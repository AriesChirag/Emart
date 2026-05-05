using Emart_DotNet.DTOs;
using Emart_DotNet.Models;

namespace Emart_DotNet.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDTO ToDTO(Customer customer)
        {
            if (customer == null) return null;

            return new CustomerDTO
            {
                UserId = customer.UserId,
                FullName = customer.FullName,
                Email = customer.Email,
                Mobile = customer.Mobile,
                Epoints = customer.Epoints ?? 0,
                BirthDate = customer.BirthDate,
                Interests = customer.Interests,
                EmartCard = customer.CardHolder != null ? new EmartCardDTO
                {
                    CardId = customer.CardHolder.CardId,
                    UserId = customer.CardHolder.UserId,
                    FullName = customer.FullName,
                    PurchaseDate = customer.CardHolder.PurchaseDate,
                    ExpiryDate = customer.CardHolder.ExpiryDate,
                    Status = customer.CardHolder.Status
                } : null
            };
        }
    }
}
