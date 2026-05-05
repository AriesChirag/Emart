using Emart_DotNet.Models;              // Customer model - DB entity
                                        // Represents user/customer in database
using Emart_DotNet.Repositories;       // ICustomerRepository interface
                                       // Data access layer - handles all DB operations
                                       // Abstracts database complexity from service layer
using Emart_DotNet.Utilities.Helpers;  // PasswordHelper - password hashing/verification
                                       // Security: passwords never stored in plain text
using System;                           // System namespace - Exception, basic types
using System.Threading.Tasks;           // async/await support for non-blocking operations

namespace Emart_DotNet.Services        // Services namespace - business logic layer
{
    public class UserService : IUserService
    // Implements IUserService interface
    // Service layer = business logic + data validation
    // Controller → Service → Repository → Database
    // Separation of concerns: each layer has specific responsibility
    {
        private readonly ICustomerRepository _customerRepository;
        // Injected repository - handles all database operations
        // Interface-based: can swap implementations without changing service
        // Readonly: assigned once in constructor, ensures immutability
        // Why use repository? Decouples database code from business logic
        // Easy to mock for unit tests

        private readonly PasswordHelper _passwordHelper;
        // Utility for password hashing and verification
        // Hashing = one-way encryption (cannot reverse)
        // Verification = compares password input with stored hash
        // Security: passwords never stored plain text in database

        public UserService(ICustomerRepository customerRepository, PasswordHelper passwordHelper)
        // Constructor - dependency injection
        // Parameters provided by DI container (configured in Startup/Program.cs)
        // Allows: easy testing (mock dependencies), loose coupling
        {
            _customerRepository = customerRepository;  // Assign repository
            _passwordHelper = passwordHelper;          // Assign password helper
        }

        // ===================== LOGIN LOGIC =====================

        public async Task<Customer> LoginAsync(string email, string password)
        // async = non-blocking, returns immediately
        // Task<Customer> = returns Customer object asynchronously
        // Parameters: email (username), password (plain text from frontend)
        // Returns: Customer object if successful
        // Throws: Exception if user not found or password wrong
        {
            // Step 1: Find user by email
            // Query database for user with matching email
            var user = await _customerRepository.FindByEmailAsync(email);
            // await = wait for async DB operation to complete
            // If multiple users with same email: DB constraint should prevent this

            if (user == null)
            // User doesn't exist in database
            // Security note: don't say "user doesn't exist" (info leak)
            // Generic message better: "Invalid email or password"
            {
                throw new Exception("User not found");
                // Exception caught by controller, returns 400 BadRequest
            }

            // Step 2: Verify password
            // Compare input password with stored hash in database
            // Allow plain text for migration/testing if hash check fails (or just strict check)
            // This is a FALLBACK mechanism - should be removed in production

            // First attempt: use hash verification (secure method)
            if (!_passwordHelper.VerifyPassword(password, user.Password))
            // VerifyPassword logic:
            // - Take input password, hash it with stored salt
            // - Compare generated hash with stored hash
            // - If match = correct password, if no match = wrong password
            {
                // If hash verification fails, fallback to plain text check
                // This allows legacy passwords or testing (SECURITY RISK - remove in production)
                if (user.Password != password)
                // Direct string comparison - INSECURE
                // Only for migration from plain text to hashed passwords
                // Should be removed once migration complete
                {
                    throw new Exception("Invalid credentials");
                    // Generic message - doesn't reveal if user exists
                }
            }

            // Step 3: Return user if all checks pass
            return user;
            // Controller receives customer object
            // Controller then generates JWT token from this user
        }

        // ===================== REGISTER LOGIC =====================

        public async Task<Customer> RegisterUserAsync(Customer customer)
        // async = non-blocking operation
        // Parameter: customer object with email, password, fullName, etc.
        // Returns: saved Customer object (now with ID from database)
        // Throws: Exception if email already exists or other validation fails
        {
            // Step 1: Check if email already in use
            // Prevent duplicate email registrations (emails should be unique)
            if (await _customerRepository.FindByEmailAsync(customer.Email) != null)
            // Database query: find user with same email
            // If found (not null) = email already exists
            {
                throw new Exception("Email already in use");
                // Controller catches this, returns 400 BadRequest with error message
                // User gets: "Email already in use"
            }

            // Step 2: Hash password before storing
            // SECURITY: never store plain text passwords
            // Hash = one-way encryption, cannot be reversed
            if (!string.IsNullOrEmpty(customer.Password))
            // Null/empty check: ensure password exists before hashing
            // Some systems allow null passwords (OAuth users), so this check is important
            {
                customer.Password = _passwordHelper.HashPassword(customer.Password);
                // Takes plain text password, generates hash with salt
                // Salt = random data added to password before hashing
                // Makes identical passwords produce different hashes (security)
                // Stored in database: hash, not plain text
            }

            // Step 3: Set default values for new user
            customer.Role = "ROLE_USER"; // Default role
                                         // Role-based access control: ROLE_USER, ROLE_ADMIN, etc.
                                         // Used by [Authorize(Roles = "ROLE_ADMIN")] in controllers

            customer.AuthProvider = "LOCAL";
            // How user authenticated: LOCAL (email/password), GOOGLE, FACEBOOK, etc.
            // Used to determine which auth methods are allowed for user
            // Example: if GOOGLE, user can't login with password

            customer.ProfileCompleted = 1;
            // Boolean flag: has user completed full profile?
            // 1 = true (profile complete), 0 = false (incomplete)
            // Some systems require full profile before using certain features

            // Step 4: Save to database
            await _customerRepository.SaveAsync(customer);
            // Save/insert new user record
            // Database auto-generates ID for user
            // After save, customer object has ID

            // Step 5: Return saved customer
            return customer;
            // Controller receives customer with ID
            // Controller generates JWT token from this user
        }

        // ===================== GOOGLE LOGIN LOGIC =====================

        public async Task<Customer> ProcessGoogleLoginAsync(string email, string fullName)
        // Handles Google OAuth login
        // Parameters: email and fullName from Google
        // Returns: Customer object (existing or newly created)
        // Throws: Exception if email linked to LOCAL account
        {
            // Step 1: Check if user already exists
            // Google login: if email exists, use existing account
            var user = await _customerRepository.FindByEmailAsync(email);
            // Query database for user with this email

            if (user != null)
            // User already exists in system
            {
                // Step 1a: Prevent mixing authentication methods
                // Check if existing account is LOCAL (email/password)
                if (user.AuthProvider == "LOCAL")
                // User registered with email/password previously
                // Cannot convert to Google login (security issue)
                // User must login with email/password or create new Google account
                {
                    throw new Exception("This email is registered with a password. Please login using email/password.");
                    // Prevents account takeover: someone using same email for Google
                }

                // Step 1b: Return existing Google user
                return user;
                // User can login immediately
            }

            // Step 2: Create new Google user (first-time login)
            // User doesn't exist, create account with Google data
            user = new Customer
            // Initialize new Customer object with Google data
            {
                Email = email,
                // Email from Google

                FullName = fullName,
                // Name from Google (optional, user might complete later)

                Role = "ROLE_USER",
                // Default role for all new users

                Password = null,
                // No password for Google users (authenticated via Google)
                // User can add password later if they want local login too

                AuthProvider = "GOOGLE",
                // Mark as Google authentication
                // Prevents password login

                Mobile = "",
                // Empty - user fills this during profile completion

                Epoints = 0,
                // Starting points/rewards - 0 for new users

                ProfileCompleted = 0 // False
                // Profile not complete - user needs to fill details
                // Email + name from Google, but address/phone missing
            };

            // Step 3: Save new Google user to database
            await _customerRepository.SaveAsync(user);
            // Inserts new user record
            // Database assigns ID

            // Step 4: Return new user
            return user;
            // Controller gets new user object
            // Controller might handle token differently (ask frontend to complete profile)
        }

        // ===================== COMPLETE REGISTRATION LOGIC =====================

        public async Task<Customer> CompleteRegistrationAsync(int userId, Customer customerUpdates)
        // Completes Google user profile with missing details
        // Parameters: userId (which user), customerUpdates (new details)
        // Returns: updated Customer object
        // Throws: Exception if user not found or not a Google user
        // Used in multi-step registration: user logs in with Google, then fills details
        {
            // Step 1: Find user by ID
            var user = await _customerRepository.FindByUserIdAsync(userId);
            // Query database for user matching ID

            if (user == null)
            // User doesn't exist
            {
                throw new Exception("User not found");
            }

            // Step 2: Verify this is a Google user
            // Only Google users need to "complete" registration
            // LOCAL users complete registration immediately on signup
            if (user.AuthProvider != "GOOGLE")
            // Not a Google user - this operation only for Google users
            {
                throw new Exception("Registration completion is only for Google users");
            }

            // Step 3: Update profile fields (only if provided)
            // Null/empty check before updating - don't overwrite with empty values
            if (!string.IsNullOrEmpty(customerUpdates.FullName)) user.FullName = customerUpdates.FullName;
            // Update full name if provided (might be incomplete from Google)

            if (!string.IsNullOrEmpty(customerUpdates.Mobile)) user.Mobile = customerUpdates.Mobile;
            // Add phone number (not provided by Google)

            if (customerUpdates.BirthDate.HasValue) user.BirthDate = customerUpdates.BirthDate;
            // BirthDate is nullable (might not be provided)
            // HasValue checks if actual date was provided
            // Nullable types allow optional fields

            if (!string.IsNullOrEmpty(customerUpdates.Interests)) user.Interests = customerUpdates.Interests;
            // User interests/preferences

            if (customerUpdates.PromotionalEmail.HasValue) user.PromotionalEmail = customerUpdates.PromotionalEmail;
            // Checkbox: opt-in for marketing emails
            // Nullable bool - null = not answered, true/false = answered

            // Step 4: Handle address
            // Address logic - If a new address is provided or ID linked
            if (customerUpdates.AddressId != null && customerUpdates.AddressId != 0)
            // AddressId provided and not zero (0 = default/no address)
            // Zero check: sometimes 0 means "no address selected"
            {
                user.AddressId = customerUpdates.AddressId;
                // Link user to saved address
                // Address might be pre-saved, user just selects it
            }

            // Step 5: Mark profile as completed
            // Mark profile as complete - user finished filling details
            user.ProfileCompleted = 1; // True
            // Now user has: email, name, phone, address, etc.
            // Can access all features

            // Step 6: Save updated profile
            await _customerRepository.SaveAsync(user);
            // Updates existing user record with new values

            // Step 7: Return updated user
            return user;
            // Controller receives updated customer
            // Controller generates fresh JWT token with updated info
        }

        // ===================== GET USER BY ID =====================

        public async Task<Customer> GetUserByIdAsync(int userId)
        // Fetch user profile by ID
        // Parameters: userId (which user)
        // Returns: Customer object
        // Throws: Exception if user not found
        // Used by: View Profile endpoint
        {
            // Query database for user by ID
            var user = await _customerRepository.FindByUserIdAsync(userId);
            // ID should be unique in database (primary key)

            if (user == null) throw new Exception("User not found");
            // If no user found, throw exception
            // Controller catches this, returns 404 NotFound

            return user;
            // Return user object
            // Controller converts to DTO before sending to frontend
        }

        // ===================== UPDATE USER PROFILE =====================

        public async Task<Customer> UpdateUserAsync(int userId, Customer customer)
        // Updates user profile (email/password login users)
        // Parameters: userId (which user), customer (new data)
        // Returns: updated Customer object
        // Throws: Exception if user not found
        // Used by: Update Profile endpoint
        {
            // Step 1: Find existing user
            // Get current user data from database
            var existing = await _customerRepository.FindByUserIdAsync(userId);
            // Query by ID (unique identifier)

            if (existing == null) throw new Exception("User not found");
            // User doesn't exist

            // Step 2: Update fields from request
            // Update provided fields with new values
            existing.FullName = customer.FullName;
            // Update full name

            existing.Mobile = customer.Mobile;
            // Update phone number

            existing.BirthDate = customer.BirthDate;
            // Update date of birth

            existing.Interests = customer.Interests;
            // Update interests/preferences

            // Step 3: Update address if provided
            // Only update if address ID is provided
            if (customer.AddressId != null) existing.AddressId = customer.AddressId;
            // Null check: don't set address if not provided
            // Allows partial updates (only update provided fields)

            // Step 4: Save updated profile
            await _customerRepository.SaveAsync(existing);
            // Update existing user record in database
            // All changes saved together (one DB operation)

            // Step 5: Return updated user
            return existing;
            // Controller converts to DTO and returns to frontend
        }
    }
}