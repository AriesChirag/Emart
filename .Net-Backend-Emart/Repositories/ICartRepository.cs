using Emart_DotNet.Models;

using System.Threading.Tasks;


namespace Emart_DotNet.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByCustomerAsync(int userId);
        Task<Cart> SaveAsync(Cart cart);
        Task<Cart?> FindByCustomerAsync(Customer customer);
    }
}