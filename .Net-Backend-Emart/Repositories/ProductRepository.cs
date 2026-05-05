using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace Emart_DotNet.Repositories
{

    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Subcategory)
                    .ThenInclude(sc => sc.Category)
                        .ThenInclude(c => c.ParentCategory)
                .ToListAsync();
        }

        public async Task<Product?> FindByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Subcategory)
                    .ThenInclude(sc => sc.Category)
                        .ThenInclude(c => c.ParentCategory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.Products
                .Include(p => p.Subcategory)
                    .ThenInclude(sc => sc.Category)
                        .ThenInclude(c => c.ParentCategory)
                .Where(p => p.ProductName.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<Product> SaveAsync(Product product)
        {
            if (product.ProductId == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                _context.Products.Update(product);
            }
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await FindByIdAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int productId)
        {
             return await _context.Products.AnyAsync(p => p.ProductId == productId);
        }
    }
}
