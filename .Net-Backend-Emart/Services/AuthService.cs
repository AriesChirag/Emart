using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Emart_DotNet.Utilities.Helpers;

namespace Emart_DotNet.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHelper _passwordHelper;

        public AuthService(ICustomerRepository customerRepository, JwtHelper jwtHelper, PasswordHelper passwordHelper)
        {
            _customerRepository = customerRepository;
            _jwtHelper = jwtHelper;
            _passwordHelper = passwordHelper;
        }

        public async Task<string?> LoginAsync(LoginModel model)
        {
            var user = await _customerRepository.FindByEmailAsync(model.Email);
            if (user == null || user.Password == null)
            {
                return null;
            }

            // In production, use VerifyPassword. For now, we assume passwords might be plain text or hashed.
            // If you are migrating, check if it's hashed. 
            // We fully switch to BCrypt here.
            if (!_passwordHelper.VerifyPassword(model.Password, user.Password))
            {
                // Fallback for plain text if needed during development/migration (Optional but dangerous in prod)
                if (user.Password != model.Password) 
                {
                    return null; 
                }
            }

            return _jwtHelper.GenerateToken(user);
        }

        public async Task<string> RegisterAsync(Customer customer, string password)
        {
            customer.Password = _passwordHelper.HashPassword(password);
            await _customerRepository.SaveAsync(customer);
            return _jwtHelper.GenerateToken(customer);
        }
    }
}
