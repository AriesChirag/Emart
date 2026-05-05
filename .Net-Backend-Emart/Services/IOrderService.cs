using Emart_DotNet.Models;
using Emart_DotNet.DTOs;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Emart_DotNet.Services
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(PlaceOrderRequestDTO req);
        
        Task<Order> UpdateOrderStatusAsync(int orderId, string status);
        
        Task<List<OrderResponseDTO>> GetOrdersByUserIdAsync(int userId);
        
        Task<OrderResponseDTO> GetOrderByIdAsync(int orderId);
    }
}