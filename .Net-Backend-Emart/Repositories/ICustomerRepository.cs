using Emart_DotNet.Models;

namespace Emart_DotNet.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> FindByUserIdAsync(int userId);

        Task<Customer?> FindByEmailAsync(string email);
        
        Task SaveAsync(Customer customer);
    }
}
