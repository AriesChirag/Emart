// ==================== IMPORTS ====================

using Emart_DotNet.Models;           // Customer model - represents database entity
                                     // Contains all customer properties (Id, Email, Password, FullName, etc.)

using System.Threading.Tasks;        // async/await support
                                     // Task = represents asynchronous operation
                                     // Task<T> = represents async operation that returns value of type T
                                     // Makes database operations non-blocking

// ==================== NAMESPACE ====================

namespace Emart_DotNet.Services      // Services namespace - contains business logic layer
                                     // Separates business logic from API controller logic
                                     // Keeps code organized and maintainable
{
    // ==================== INTERFACE DEFINITION ====================

    public interface IUserService
    // Interface = contract/specification of what methods a service MUST implement
    // Why use interface instead of concrete class?
    // 1. ABSTRACTION - depends on what service does, not how it does it
    // 2. DEPENDENCY INJECTION - controller depends on interface, not concrete class
    // 3. TESTABILITY - can mock IUserService for unit tests
    // 4. FLEXIBILITY - can swap implementations without changing controller code
    // 5. SOLID PRINCIPLE - Dependency Inversion (depend on abstractions, not implementations)
    //
    // In Startup.cs/Program.cs, we register: services.AddScoped<IUserService, UserService>();
    // This tells ASP.NET: "when someone requests IUserService, give them UserService instance"
    {
        // ===================== METHOD 1: LOGIN =====================

        Task<Customer> LoginAsync(string email, string password);

        // Purpose: Authenticate user with email and password
        // Returns: Customer object if login successful
        // Throws: Exception if email not found OR password incorrect
        //
        // Parameters:
        // - email (string): user's email address - used to find user in database
        // - password (string): plain text password - will be compared with password hash in DB
        //
        // Why async? Database query is I/O operation (slow)
        // async makes it non-blocking - thread can handle other requests while waiting for DB
        // Performance benefit = one thread can handle 100s of requests simultaneously
        //
        // Return type: Task<Customer>
        // - Task<T> = represents async operation that will eventually return T
        // - <Customer> = will return Customer object when login succeeds
        // - Controller calls: await _userService.LoginAsync(email, password);
        //   This waits for async operation to complete and gets Customer object
        //
        // Implementation example (in UserService.cs):
        // - Find customer by email in database
        // - Get stored password hash from database
        // - Compare provided password with stored hash using BCrypt/similar
        // - If matches: return Customer object
        // - If doesn't match: throw exception "Invalid password"


        // ===================== METHOD 2: REGISTER =====================

        Task<Customer> RegisterUserAsync(Customer customer);

        // Purpose: Create new user account
        // Returns: Saved Customer object (with ID from database auto-increment)
        // Throws: Exception if email already exists OR validation fails
        //
        // Parameter:
        // - customer (Customer): customer object containing: Email, Password, FullName, etc.
        //   Note: Password is plain text here - will be hashed before saving to DB
        //
        // Why receive Customer object instead of individual parameters?
        // - Customer has multiple properties (email, password, fullName, phone, address, etc.)
        // - Easier to pass one object than 10 individual parameters
        // - Follows DTO/Model pattern for API data transfer
        //
        // Why async? Database INSERT operation is I/O operation (slow)
        // One thread can handle 100s of registration requests simultaneously
        //
        // Return type: Task<Customer>
        // - Returns saved customer with ID (database generated)
        // - Client/Controller uses this ID for future requests
        // - Customer ID = primary key in database
        //
        // Implementation example (in UserService.cs):
        // - Validate input: email format, password strength, fullName not empty
        // - Check if email already exists (duplicate check)
        // - Hash password using BCrypt (never store plain text passwords!)
        // - Insert new record in Customers table
        // - Return newly created Customer object
        // - Database auto-increment generates new ID


        // ===================== METHOD 3: GOOGLE LOGIN =====================

        Task<Customer> ProcessGoogleLoginAsync(string email, string fullName);

        // Purpose: Handle Google OAuth login
        // Returns: Customer object (existing or newly created)
        // Throws: Exception if database error occurs
        //
        // Parameters:
        // - email (string): user's email from Google OAuth
        // - fullName (string): user's name from Google OAuth
        //
        // What does this method do?
        // - Check if customer with this email already exists in database
        // - IF EXISTS: return existing customer (they've logged in before)
        // - IF NOT EXISTS: create new customer account with Google data
        // This is "seamless" login - no separate signup process
        //
        // Why async? Database query/insert operations
        //
        // Return type: Task<Customer>
        // - Always returns Customer object
        // - Either existing customer or newly created customer
        //
        // Implementation example (in UserService.cs):
        // - Query database: SELECT * FROM Customers WHERE Email = @email
        // - If found: return that customer
        // - If not found:
        //   - Create new Customer object with email and fullName
        //   - Password = null or auto-generated (OAuth doesn't use password)
        //   - Insert into database
        //   - Return newly created customer
        //
        // Why different from RegisterAsync?
        // - RegisterAsync requires customer to provide password
        // - ProcessGoogleLoginAsync doesn't need password (Google handles auth)


        // ===================== METHOD 4: COMPLETE REGISTRATION =====================

        Task<Customer> CompleteRegistrationAsync(int userId, Customer customer);

        // Purpose: Update incomplete user profile after Google login
        // Returns: Updated Customer object
        // Throws: Exception if user not found OR update fails
        //
        // Parameters:
        // - userId (int): which customer to update - primary key from database
        // - customer (Customer): new/updated customer data to save
        //   Contains: address, phone, preferences, etc.
        //
        // When is this used?
        // - User logs in via Google (gets basic info: email, name)
        // - But registration isn't complete (missing: address, phone, etc.)
        // - User fills remaining details on frontend
        // - Frontend calls this endpoint to save those details
        //
        // Why async? Database UPDATE operation
        //
        // Return type: Task<Customer>
        // - Returns updated customer object from database
        // - Contains all data including new additions
        //
        // Implementation example (in UserService.cs):
        // - Find customer by userId
        // - If not found: throw exception "User not found"
        // - Update customer properties: address, phone, preferences, etc.
        // - Save to database: UPDATE Customers SET ... WHERE Id = @userId
        // - Return updated customer


        // ===================== METHOD 5: GET USER BY ID =====================

        Task<Customer> GetUserByIdAsync(int userId);

        // Purpose: Fetch user profile by ID
        // Returns: Customer object
        // Throws: Exception if user not found
        //
        // Parameter:
        // - userId (int): customer ID - primary key in database
        //
        // What happens?
        // - Query database: SELECT * FROM Customers WHERE Id = @userId
        // - If found: return Customer object
        // - If not found: throw exception "User not found"
        //
        // Why async? Database query operation (I/O operation)
        //
        // Return type: Task<Customer>
        // - Returns single Customer object
        //
        // Used for: "View Profile" endpoint in controller
        // - Controller calls this to get user data
        // - Converts to DTO and returns to client
        //
        // Implementation example (in UserService.cs):
        // - Query: SELECT * FROM Customers WHERE Id = userId
        // - If found: return customer
        // - If not found: throw new Exception("Customer not found");


        // ===================== METHOD 6: UPDATE USER =====================

        Task<Customer> UpdateUserAsync(int userId, Customer customer);

        // Purpose: Update user profile information
        // Returns: Updated Customer object
        // Throws: Exception if user not found OR update fails
        //
        // Parameters:
        // - userId (int): which customer to update - primary key
        // - customer (Customer): new customer data with updated values
        //   Contains: fullName, phone, address, etc. (updated values)
        //
        // What happens?
        // - Find customer by userId
        // - If not found: throw exception
        // - Update all properties: fullName, phone, address, etc.
        // - Save changes to database: UPDATE Customers SET ... WHERE Id = @userId
        // - Return updated customer object
        //
        // Why async? Database UPDATE operation (I/O)
        //
        // Return type: Task<Customer>
        // - Returns updated customer object
        // - Client receives confirmation with updated values
        //
        // Used for: "Update Profile" endpoint in controller
        // - User edits their profile
        // - Controller calls this to save changes
        //
        // Implementation example (in UserService.cs):
        // - Check if userId exists in database
        // - If not: throw exception "User not found"
        // - Update properties: customer.FullName = customer.FullName, etc.
        // - Execute: UPDATE Customers SET FullName = @fullName, Phone = @phone WHERE Id = @userId
        // - Return updated customer
        //
        // Security Note: Controller should verify userId = current logged-in user
        // (Should have [Authorize] attribute to prevent unauthorized updates)
    }
    // End of IUserService interface
}
// End of namespace