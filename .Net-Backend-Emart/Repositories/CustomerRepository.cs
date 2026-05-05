using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> FindByUserIdAsync(int userId)
        {
            return await _context.Customers
                .Include(c => c.CardHolder)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Customer?> FindByEmailAsync(string email)
        {
            return await _context.Customers
                .Include(c => c.CardHolder)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task SaveAsync(Customer customer)
        {
            if (_context.Entry(customer).State == EntityState.Detached)
            {
                _context.Customers.Add(customer);
            }

            await _context.SaveChangesAsync();
        }
    }
}
