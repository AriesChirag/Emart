using Emart_DotNet.Models;
// Import Address model - entity class representing Address table
// This model is used for type safety and EF mapping

using Microsoft.EntityFrameworkCore;
// Entity Framework Core - ORM (Object-Relational Mapping)
// Provides DbContext, DbSet<T>, LINQ queries, change tracking, SaveChangesAsync()
// Simplifies database operations - no manual SQL needed

using System.Collections.Generic;
// System.Collections.Generic namespace
// Provides List<T> and other generic collections
// Used to return lists of addresses

using System.Linq;
// System.Linq namespace - Language Integrated Query
// Provides LINQ methods: Where(), Select(), ToListAsync(), etc.
// Allows writing SQL-like queries in C#

using System.Threading.Tasks;
// System.Threading.Tasks namespace
// Provides Task, Task<T> for async operations
// Used for async/await pattern

namespace Emart_DotNet.Repositories
// Namespace - groups all repository classes together
// Repository layer = Data Access Layer (DAL)
// Separates database logic from business logic
{


    public class AddressRepository : IAddressRepository
    // AddressRepository = concrete implementation of IAddressRepository interface
    // Implements Repository Pattern
    // Repository Pattern = abstraction layer for data access
    // Benefits: easy to test (mock interface), loose coupling (depend on interface)
    // Can swap database without changing business logic
    {
        private readonly AppDbContext _context;
        // DbContext = connection to database
        // private readonly = only in this class, assigned once in constructor
        // AppDbContext represents database with DbSet properties (Addresses, Customers, Orders, etc.)
        // Used to execute queries, track changes, save changes

        public AddressRepository(AppDbContext context)
        // Constructor - dependency injection of DbContext
        // AppDbContext is injected by DI container
        // Allows testing: can pass mock DbContext for unit tests
        {
            _context = context;
            // Assign injected context to field
        }

        public async Task<Address> SaveAsync(Address address)
        // SaveAsync = create new address OR update existing address
        // async Task<Address> = returns Address asynchronously
        // "Save" = insert if new, update if existing (upsert pattern)
        // Parameter: address object to save
        {
            if (address.AddressId == 0)
            // Check if address is new or existing
            // AddressId == 0 means NEW record (not yet in database)
            // In database, auto-increment starts at 1
            // So 0 always indicates new entity
            {
                _context.Addresses.Add(address);
                // Add method: marks entity as "Added" in change tracker
                // Change tracker = EF's memory of what changed
                // This doesn't execute SQL yet - just marks for insertion
            }
            else
            // AddressId != 0 means EXISTING record (already in database)
            {
                _context.Addresses.Update(address);
                // Update method: marks entity as "Modified" in change tracker
                // EF will generate UPDATE SQL for changed properties
                // Only changed properties are included in SQL UPDATE
            }
            await _context.SaveChangesAsync();
            // SaveChangesAsync = execute all pending changes to database
            // Sends SQL INSERT or UPDATE command to database
            // await = wait for database operation to complete (non-blocking)
            // Throws exception if constraint violated, FK not found, etc.
            // Transaction: all changes in one SaveChangesAsync = atomic (all-or-nothing)
            return address;
            // Return the saved address object
            // For new address: now has AddressId assigned from database
            // For updated address: contains current state from database
        }

        public async Task<List<Address>> FindByUserIdAsync(int userId)
        // FindByUserIdAsync = fetch all addresses for specific user
        // Parameter: userId to filter by
        // Returns: List<Address> asynchronously
        // Used when user views their saved addresses
        {
            return await _context.Addresses
            // _context.Addresses = DbSet<Address> (represents Addresses table)
            // Can be queried like LINQ query

            .Where(a => a.UserId == userId)
            // Where = LINQ filter method
            // a => a.UserId == userId = lambda expression (anonymous function)
            // Translates to SQL: WHERE UserId = @userId
            // Filters addresses where UserId matches parameter
            // Example: if userId=5, returns all addresses where UserId=5

            .ToListAsync();
            // ToListAsync = execute query and return as List<Address>
            // Async version of ToList()
            // await = wait for database query to complete
            // Without ToListAsync, query is not executed (deferred execution)
            // .AsNoTracking() could be added here if updates not needed (better performance)
        }

        public async Task DeleteAsync(int addressId)
        // DeleteAsync = delete address by ID
        // Parameter: addressId to delete
        // Note: this is HARD DELETE (permanent removal)
        // Alternative: SOFT DELETE (mark IsDeleted=true, keep data)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            // FindAsync = fetch single record by primary key
            // Async version of Find()
            // Queries database: SELECT * FROM Addresses WHERE AddressId = @addressId
            // Returns Address object or null if not found

            if (address != null)
            // Null check - ensure address exists before deleting
            // If null (address not found), skip deletion
            // Prevents exception from trying to remove null entity
            {
                _context.Addresses.Remove(address);
                // Remove method: marks entity as "Deleted" in change tracker
                // Doesn't execute SQL yet - just marks for removal
                // EF will generate DELETE SQL

                await _context.SaveChangesAsync();
                // SaveChangesAsync = execute deletion in database
                // Sends: DELETE FROM Addresses WHERE AddressId = @addressId
                // await = wait for database operation
                // Throws exception if FK constraint violated (addresses still in use by orders)
            }
            // If address not found: method completes silently (no error)
            // Could improve: return bool (success/failure) to caller
        }

        public async Task<Address?> FindByIdAsync(int addressId)
        // FindByIdAsync = fetch single address by ID
        // Parameter: addressId to find
        // Returns: Address object or null (Address?)
        // ? means nullable - might return null if not found
        // Used to view/edit specific address
        {
            return await _context.Addresses.FindAsync(addressId);
            // FindAsync = EF's built-in method to query by primary key
            // Async version of Find(key)
            // Translates to: SELECT * FROM Addresses WHERE AddressId = @addressId LIMIT 1
            // await = wait for database query to complete
            // Returns Address if found, null if not found
            // Checks change tracker first (if already loaded) before querying DB
            // More efficient than .Where(a => a.AddressId == addressId).FirstOrDefaultAsync()
        }
    }
    // End of AddressRepository class
}
// End of namespace