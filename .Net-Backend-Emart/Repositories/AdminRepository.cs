using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> FindByEmailAsync(string email)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Admin?> FindByIdAsync(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin> SaveAsync(Admin admin)
        {
            if (admin.AdminId == 0)
            {
                _context.Admins.Add(admin);
            }
            else
            {
                _context.Admins.Update(admin);
            }
            await _context.SaveChangesAsync();
            return admin;
        }
    }
}
