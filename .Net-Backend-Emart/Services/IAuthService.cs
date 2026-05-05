using Emart_DotNet.DTOs;
using Emart_DotNet.Models;

namespace Emart_DotNet.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginModel model);
        Task<string> RegisterAsync(Customer customer, string password); // Basic register for convenience
    }
}
