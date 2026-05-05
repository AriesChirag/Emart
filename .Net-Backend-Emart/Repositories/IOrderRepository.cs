using Emart_DotNet.Models;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Emart_DotNet.Repositories
{
  
    public interface IOrderRepository
    {
        Task<Order> SaveAsync(Order order);
        Task<Order?> FindByIdAsync(int orderId);
        Task<List<Order>> FindByCustomerUserIdAsync(int userId);
    }
}