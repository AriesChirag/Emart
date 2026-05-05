using Emart_DotNet.Repositories;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{

    public class EPointsService : IEPointsService
    {
        private readonly ICustomerRepository _customerRepo;

        public EPointsService(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<int> CreditPointsAsync(int userId, int points)
        {
            var customer = await _customerRepo.FindByUserIdAsync(userId);
            if (customer == null) throw new Exception("User not found");

            customer.Epoints = (customer.Epoints ?? 0) + points;
            await _customerRepo.SaveAsync(customer);
            return customer.Epoints ?? 0;
        }

        public async Task<int> RedeemPointsAsync(int userId, int points)
        {
            var customer = await _customerRepo.FindByUserIdAsync(userId);
            if (customer == null) throw new Exception("User not found");

            if ((customer.Epoints ?? 0) < points)
                throw new Exception("Insufficient Epoints");

            customer.Epoints = (customer.Epoints ?? 0) - points;
            await _customerRepo.SaveAsync(customer);
            return customer.Epoints ?? 0;
        }

        public async Task<int> GetBalanceAsync(int userId)
        {
            var customer = await _customerRepo.FindByUserIdAsync(userId);
            if (customer == null) throw new Exception("User not found");
            return customer.Epoints ?? 0;
        }
    }
}
