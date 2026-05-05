using Emart_DotNet.Models;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin?> FindByEmailAsync(string email);
        Task<Admin?> FindByIdAsync(int id);
        Task<Admin> SaveAsync(Admin admin);
    }
}
