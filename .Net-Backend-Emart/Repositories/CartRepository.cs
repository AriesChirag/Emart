using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{

    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> FindByCustomerAsync(Customer customer)
        {
             return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == customer.UserId);
        }

        public async Task<Cart> SaveAsync(Cart cart)
        {
            if (cart.CartId == 0)
            {
                _context.Carts.Add(cart);
            }
            else
            {
                _context.Carts.Update(cart);
            }
            await _context.SaveChangesAsync();
            return cart;
        }
    }
}
