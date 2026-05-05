using Emart_DotNet.DTOs;             // DTOs (Data Transfer Objects)
                                     // Used to send filtered/safe data to frontend
                                     // Don't expose password hashes, internal IDs, sensitive DB fields
using Emart_DotNet.Models;           // Customer model - represents DB entity
                                     // Contains all properties including sensitive data
                                     // Maps directly to database table structure
using Emart_DotNet.Services;         // IUserService interface - business logic layer
                                     // Handles DB operations, validations, business rules
                                     // Using interface allows dependency injection & easy testing
using Emart_DotNet.Utilities.Helpers; // JwtHelper - generates JWT tokens
                                      // JWT = JSON Web Token for stateless authentication
                                      // Token contains user info (encrypted) and is used for future requests
using Microsoft.AspNetCore.Mvc;      // ASP.NET Core API framework
                                     // Provides: ControllerBase, Ok(), BadRequest(), NotFound(), etc.
                                     // Handles HTTP request/response, model binding, validation
using System.Threading.Tasks;        // async/await support
                                     // Makes methods non-blocking - important for performance
                                     // One thread can handle multiple requests = better scalability

namespace Emart_DotNet.Controllers   // Namespace - groups all controllers together
{
    [ApiController]
    // Marks this class as an API controller
    // Enables automatic model validation, error responses, parameter binding
    // Without this attribute, ASP.NET doesn't know it's an API controller

    [Route("api/users")]
    // Base route for all endpoints in this controller
    // All URLs in this controller will start with /api/users
    // Final URLs: /api/users/login, /api/users/register, /api/users/{userId}, etc.

    public class UserController : ControllerBase
    // Inherits from ControllerBase
    // ControllerBase = API only (lightweight, no View support)
    // Provides methods: Ok(), BadRequest(), NotFound(), Created(), etc.
    {
        private readonly IUserService _userService;
        // Private = only accessible within this class (encapsulation)
        // Readonly = can only be assigned once (in constructor) - ensures immutability
        // Interface-based = depends on abstraction, not concrete implementation
        // Benefits: loose coupling, easy to mock for unit tests, can swap implementations

        private readonly JwtHelper _jwtHelper;
        // Helper class for generating JWT tokens
        // JWT token is sent to client and used for future authenticated requests
        // Stateless authentication = no session stored on server = scalable

        public UserController(IUserService userService, JwtHelper jwtHelper)
        // Constructor - called when controller is instantiated
        // Parameters are INJECTED by ASP.NET's dependency injection container
        // DI container is configured in Startup.cs or Program.cs
        // Benefits: easier testing, loose coupling, configuration centralized
        {
            _userService = userService;   // Assign injected service to field
            _jwtHelper = jwtHelper;       // Assign injected helper to field
        }

        // ===================== LOGIN =====================

        [HttpPost("login")]
        // HTTP POST method - /api/users/login
        // POST used for sensitive data (password in body, not URL)
        // URL with password would be logged/exposed - security risk

        public async Task<IActionResult> Login([FromBody] LoginModel request)
        // async = method returns immediately (non-blocking)
        // Task<IActionResult> = returns an IActionResult asynchronously
        // [FromBody] = ASP.NET extracts JSON from request body and maps to LoginModel
        // Example: { "email": "user@example.com", "password": "pass123" } → LoginModel object
        // IActionResult = flexible return type (Ok(), BadRequest(), NotFound(), etc.)
        {
            try
            // Try-catch for exception handling
            // Catches any exceptions thrown by service layer
            // Returns meaningful error response instead of 500 Internal Server Error
            {
                // Call service layer to verify email and password
                // Service handles password verification, user lookup, validation
                var customer = await _userService.LoginAsync(request.Email, request.Password);
                // await = wait for async operation to complete (non-blocking)
                // If login fails, service throws exception → caught in catch block
                // If successful, returns Customer object with user details

                // Generate JWT token using customer data
                // Token contains: userId, email, role, expiration time (configured in JwtHelper)
                string token = _jwtHelper.GenerateToken(customer);
                // Token is signed - if hacker modifies it, signature fails
                // Token expires after set duration (e.g., 1 hour) - security measure
                // Stateless = server doesn't store token, just verifies signature

                // Return token wrapped in TokenResponse DTO to client
                // Ok() = HTTP 200 status code (success)
                // TokenResponse ensures consistent JSON format: { "token": "eyJhbGc..." }
                return Ok(new TokenResponse(token));
                // Client receives token, stores it (localStorage/sessionStorage/cookie)
                // Client sends token in future requests: Authorization: Bearer {token}
            }
            catch (System.Exception ex)
            // Catches ANY exception from service layer
            // Examples: wrong password, user not found, database error, validation failed
            {
                // BadRequest() = HTTP 400 status code
                // Indicates client sent invalid/bad data (wrong credentials)
                // Returns: { "message": "Invalid email or password" }
                return BadRequest(new { message = ex.Message });
                // Returns error message from exception
                // Note: Be careful - don't expose sensitive info (e.g., "user with email doesn't exist")
            }
        }

        // ===================== REGISTER =====================

        [HttpPost("register")]
        // HTTP POST - /api/users/register
        // POST used because we're creating a new resource (user)

        public async Task<IActionResult> RegisterUser([FromBody] Customer customer)
        // [FromBody] = JSON from request body maps to Customer object
        // ASP.NET automatically: deserializes JSON, validates, binds to model
        // Example: { "email": "new@example.com", "password": "pass123", "fullName": "John Doe" }
        // If JSON is invalid, ASP.NET returns 400 automatically (before method runs)
        {
            try
            {
                // Call service to save new user in database
                // Service handles: password hashing, email validation, duplicate email check
                var savedCustomer = await _userService.RegisterUserAsync(customer);
                // If email already exists → service throws exception → caught in catch
                // If successful, returns saved customer object (now has ID from DB auto-increment)

                // Generate JWT token immediately after successful registration
                // User doesn't need to login separately - token provided on registration
                string token = _jwtHelper.GenerateToken(savedCustomer);
                // Token created with new user's data (ID, email, role)

                // Return token to client
                // Ok() = HTTP 200
                // Client can use this token immediately for authenticated requests
                return Ok(new TokenResponse(token));
            }
            catch (System.Exception ex)
            // Catches exceptions from service layer
            // Examples: email already exists, validation failed, DB error, duplicate entry
            {
                // BadRequest() = HTTP 400
                // Returns error message from exception
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===================== GOOGLE LOGIN =====================

        [HttpPost("google-login")]
        // POST request - /api/users/google-login?email=user@gmail.com&fullName=John
        // Query parameters from Google OAuth flow

        public async Task<IActionResult> GoogleLogin(
            [FromQuery] string email,      // email from URL query string (?email=...)
                                           // [FromQuery] tells ASP.NET to extract from URL, not body
                                           // Google sends OAuth data as URL parameters

            [FromQuery] string fullName    // fullName from URL query string
                                           // Google provides user info after authentication
        )
        {
            try
            {
                // Process Google login - create user if first-time, or fetch existing
                // Service handles: checking if user exists, creating if not, returning user data
                var customer = await _userService.ProcessGoogleLoginAsync(email, fullName);
                // Seamless experience: user can login with Google immediately (no separate signup)
                // If user exists in DB with same email: return existing user
                // If new user: create account with Google data

                // Convert Customer entity to CustomerDTO
                // Entity = full model with all properties (including passwordHash, internal fields)
                // DTO = filtered model with only safe fields for client consumption
                return Ok(Mappers.CustomerMapper.ToDTO(customer));
                // Why convert? SECURITY - DTO excludes sensitive fields
                // Example: Model has { id, email, fullName, passwordHash, internalNotes }
                //          DTO has { id, email, fullName, avatar }
                // This endpoint returns customer data, not token (different from login/register)
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===================== COMPLETE REGISTRATION =====================

        [HttpPut("complete-registration/{userId}")]
        // PUT request - /api/users/complete-registration/5
        // {userId} is route parameter extracted from URL path
        // PUT = updating existing resource
        // Used in multi-step registration: user logs in via Google, then fills remaining details

        public async Task<IActionResult> CompleteRegistration(
            int userId,                     // userId from URL path: /.../{userId}
                                            // ASP.NET automatically converts string "5" to int 5
                                            // If invalid (e.g., "/abc") → ASP.NET returns 400 automatically

            [FromBody] Customer customer    // Additional user data from request body (JSON)
                                            // Contains: address, phone, preferences, etc.
        )
        {
            try
            {
                // Update user profile with additional details
                // Service handles: validation, authorization check, DB update
                var updatedCustomer = await _userService.CompleteRegistrationAsync(userId, customer);
                // Returns updated customer object with new data
                // Service might also send notifications, update user status, etc.

                // Generate new JWT token with updated user info
                // Old token might have incomplete data - needs to be refreshed
                string token = _jwtHelper.GenerateToken(updatedCustomer);
                // New token contains updated user data

                // Return new token to client
                // Ok() = HTTP 200
                return Ok(new TokenResponse(token));
                // Client replaces old token with new one
                // Future requests use updated token with complete user data
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===================== VIEW PROFILE =====================

        [HttpGet("{userId}")]
        // GET request - /api/users/5
        // GET = retrieving data, no side effects
        // Idempotent = calling multiple times returns same result
        // Cacheable = browser/server can cache response

        public async Task<IActionResult> ViewProfile(int userId)
        // userId from route parameter {userId}
        // No [FromBody] because GET shouldn't have request body
        {
            try
            {
                // Fetch user from database by ID
                // Service retrieves user record matching the ID
                var user = await _userService.GetUserByIdAsync(userId);
                // If user not found, service throws exception → caught in catch block
                // If found, returns Customer object

                // Convert model to DTO before sending to client
                // Security: DTO excludes sensitive fields (passwordHash, etc.)
                return Ok(Mappers.CustomerMapper.ToDTO(user));
                // Ok() = HTTP 200
                // Returns: { "id": 5, "email": "user@example.com", "fullName": "John", ... }
                // No passwordHash, no internal sensitive fields in response
            }
            catch (System.Exception ex)
            {
                // NotFound() = HTTP 404 status code
                // Indicates the resource (user) doesn't exist
                // 404 = resource not found
                // 400 = client sent bad data
                // 401 = unauthorized
                // 403 = forbidden
                return NotFound(new { message = ex.Message });
            }
        }

        // ===================== UPDATE PROFILE =====================

        [HttpPut("{userId}")]
        // PUT request - /api/users/5
        // PUT = updating existing resource
        // Should include full resource (PATCH is for partial updates)
        // Idempotent = calling multiple times with same data = same result

        public async Task<IActionResult> UpdateProfile(
            int userId,                     // userId from URL path
                                            // Specifies which user to update

            [FromBody] Customer customer    // Updated user data from request body (JSON)
                                            // Contains fields to update: fullName, phone, address, etc.
        )
        {
            try
            {
                // Update user record in database
                // Service handles: validation, authorization check (should verify userId = current user)
                var updated = await _userService.UpdateUserAsync(userId, customer);
                // Note: Controller should have [Authorize] attribute for security
                // (Verify current user can only update their own profile)
                // Returns updated customer object from DB

                // Convert to DTO and return to client
                // Security: DTO excludes sensitive fields
                return Ok(Mappers.CustomerMapper.ToDTO(updated));
                // Ok() = HTTP 200
                // Returns updated user data (client confirms update success)
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}