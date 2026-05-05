using Emart_DotNet.Services;
using Emart_DotNet.Utilities.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("oauth2/authorization")] // Matches Frontend URL structure
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("callback")]
        [Route("/api/auth/google-callback")] // Internal callback route
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                // Fallback or error handling
                 return Redirect("http://localhost:5173/login?error=GoogleAuthFailed");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                 return Redirect("http://localhost:5173/login?error=EmailNotFound");
            }

            try 
            {
                var user = await _userService.ProcessGoogleLoginAsync(email, name ?? "Google User");
                var token = _jwtHelper.GenerateToken(user);
                
                // Redirect to Frontend with Token
                return Redirect($"http://localhost:5173/login?token={token}");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:5173/login?error={Uri.EscapeDataString(ex.Message)}");
            }
        }
    }
}
