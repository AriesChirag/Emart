using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Emart_DotNet.Utilities.Helpers;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly PasswordHelper _passwordHelper;
        private readonly JwtHelper _jwtHelper;

        public AdminService(IAdminRepository adminRepository, PasswordHelper passwordHelper, JwtHelper jwtHelper)
        {
            _adminRepository = adminRepository;
            _passwordHelper = passwordHelper;
            _jwtHelper = jwtHelper;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var admin = await _adminRepository.FindByEmailAsync(email);
            if (admin == null)
            {
                throw new Exception("Admin not found");
            }

            // Check password - support both hashed and plain text for backwards compatibility
            bool passwordValid = false;
            try
            {
                passwordValid = _passwordHelper.VerifyPassword(password, admin.Password);
            }
            catch
            {
                // If hash verification fails, try plain text comparison
                passwordValid = (admin.Password == password);
            }

            if (!passwordValid)
            {
                throw new Exception("Invalid credentials");
            }

            // Check if admin is active (ulong 1 = true, 0 = false)
            if (admin.Active == 0)
            {
                throw new Exception("Admin inactive");
            }

            // Generate JWT token for admin
            return _jwtHelper.GenerateAdminToken(admin);
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            var admin = await _adminRepository.FindByIdAsync(id);
            if (admin == null)
            {
                throw new Exception("Admin not found");
            }
            return admin;
        }
    }
}
