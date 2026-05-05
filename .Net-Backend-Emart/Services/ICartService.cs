using Emart_DotNet.Models;
using Emart_DotNet.DTOs;

using System.Threading.Tasks;


namespace Emart_DotNet.Services
{
    public interface ICartService
    {
        Task<CartItem> AddToCartAsync(int userId, int productId, int quantity, string purchaseType, int epointsUsed);
        
        Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity);
        
        Task RemoveFromCartAsync(int cartItemId);
        
        Task<CartResponseDTO> GetCartSummaryAsync(int userId);
        
        Task ClearCartByUserAsync(int userId);
    }
}