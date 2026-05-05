using Emart_DotNet.Models;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public interface IAdminService
    {
        Task<string> LoginAsync(string email, string password);
        Task<Admin> GetAdminByIdAsync(int id);
    }
}
