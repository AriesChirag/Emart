using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{

    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepo;

        public AddressService(IAddressRepository addressRepo)
        {
            _addressRepo = addressRepo;
        }

        public async Task<Address> AddAddressAsync(int userId, Address address)
        {
            address.UserId = userId;
            return await _addressRepo.SaveAsync(address);
        }

        public async Task<List<Address>> GetAddressesByCustomerAsync(int userId)
        {
            return await _addressRepo.FindByUserIdAsync(userId);
        }

        public async Task StartDeleteAddressAsync(int addressId) // Renamed internally if needed, but interface demands DeleteAddressAsync
        { 
             await _addressRepo.DeleteAsync(addressId);
        }
        
        // Interface implementation for Delete
        public async Task DeleteAddressAsync(int addressId)
        {
            await _addressRepo.DeleteAsync(addressId);
        }
    }
}

