using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;

namespace Emart_DotNet.Services
{
    public class EmartCardService : IEmartCardService
    {
        private readonly IEmartCardRepository _emartCardRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly AppDbContext _context;

        public EmartCardService(
            IEmartCardRepository emartCardRepository,
            ICustomerRepository customerRepository,
            AppDbContext context)
        {
            _emartCardRepository = emartCardRepository;
            _customerRepository = customerRepository;
            _context = context;
        }

        public async Task<EmartCard> ApplyForCardAsync(ApplyEmartCardRequest request)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            if (await _emartCardRepository.ExistsByUserIdAsync(request.UserId))
                throw new Exception("User already has an eMart Card");

            var customer = await _customerRepository.FindByUserIdAsync(request.UserId)
                ?? throw new Exception("User not found");

            // Create card with ALL required fields from request (matching Java)
            var card = new EmartCard
            {
                UserId = request.UserId,
                AnnualIncome = (double)request.AnnualIncome,
                PanCard = request.PanCard,
                BankDetails = request.BankDetails,
                Occupation = request.Occupation,
                EducationQualification = request.EducationQualification,
                PurchaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5)),
                TotalEpointsUsed = 0,
                Status = "ACTIVE"
            };

            await _emartCardRepository.SaveAsync(card);

            // Update Customer Profile & Points
            customer.CardHolder = card;
            customer.Epoints += 100; // Bonus Points

            // Update optional fields if provided
            if (request.BirthDate.HasValue)
            {
                customer.BirthDate = request.BirthDate;
            }
            if (!string.IsNullOrEmpty(request.Interests))
            {
                customer.Interests = request.Interests;
            }

            await _customerRepository.SaveAsync(customer);

            await tx.CommitAsync();
            return card;
        }

        public async Task UseEpointsAsync(int userId, int pointsUsed)
        {
            var card = await _emartCardRepository.FindByUserIdAsync(userId)
                ?? throw new Exception("eMart Card not found");

            if (card.Status != "ACTIVE")
            {
                throw new Exception("Card is not active");
            }

            card.TotalEpointsUsed = (card.TotalEpointsUsed ?? 0) + pointsUsed;
            await _emartCardRepository.SaveAsync(card);
        }

        public async Task<EmartCardDTO?> GetCardDetailsAsync(int userId)
        {
            var card = await _emartCardRepository.FindByUserIdAsync(userId);
            if (card == null) return null;

            var customer = await _customerRepository.FindByUserIdAsync(userId)
                ?? throw new Exception("User not found");

            return new EmartCardDTO
            {
                CardId = card.CardId,
                UserId = card.UserId,
                FullName = customer.FullName,
                PurchaseDate = card.PurchaseDate,
                ExpiryDate = card.ExpiryDate,
                Status = card.Status
            };
        }
    }
}
