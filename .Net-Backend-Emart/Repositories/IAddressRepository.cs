using Emart_DotNet.Models;  
namespace Emart_DotNet.Repositories
{
    public interface IAddressRepository
    {
        Task<Address> SaveAsync(Address address);
        Task<List<Address>> FindByUserIdAsync(int userId);
        Task DeleteAsync(int addressId);
        Task<Address?> FindByIdAsync(int addressId);
    }
}