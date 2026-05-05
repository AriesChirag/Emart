using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(int storeId)
        {
            return await _context.Stores.FindAsync(storeId);
        }

        public async Task<Store?> FindByIdAsync(int storeId)
        {
            return await _context.Stores.FindAsync(storeId);
        }

        public async Task<IEnumerable<Store>> GetStoresByCityAsync(string city)
        {
            return await _context.Stores
                .Where(s => s.City == city)
                .ToListAsync();
        }
    }
}
