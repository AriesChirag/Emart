using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{

    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem?> FindByCartAndProductAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<List<CartItem>> FindByCartAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<CartItem> SaveAsync(CartItem cartItem)
        {
            if (cartItem.CartItemId == 0)
            {
                _context.CartItems.Add(cartItem);
            }
            else
            {
                _context.CartItems.Update(cartItem);
            }
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task DeleteAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByCartIdAsync(int cartId)
        {
            var items = await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> FindByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .ThenInclude(c => c.User) // Need User for validation
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
        }
    }
}
