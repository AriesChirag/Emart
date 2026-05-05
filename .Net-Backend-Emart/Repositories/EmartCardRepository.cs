using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{
    public class EmartCardRepository : IEmartCardRepository
    {
        private readonly AppDbContext _context;

        public EmartCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmartCard?> FindByUserIdAsync(int userId)
        {
            return await _context.EmartCards
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            return await _context.EmartCards
                .AnyAsync(c => c.UserId == userId);
        }

        public async Task SaveAsync(EmartCard card)
        {
            if (_context.Entry(card).State == EntityState.Detached)
            {
                _context.EmartCards.Add(card);
            }

            await _context.SaveChangesAsync();
        }
    }
}
