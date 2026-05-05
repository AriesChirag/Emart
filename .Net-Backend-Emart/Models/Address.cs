using System;
// System namespace - provides fundamental types and base functionality
// Used for common data types and utilities

using System.Collections.Generic;
// System.Collections.Generic namespace
// Provides generic collection types like List<T>, Dictionary<T,K>, etc.
// Used here for ICollection<T> navigation properties

namespace Emart_DotNet.Models;
// Namespace - groups all database entity models together
// File-scoped namespace (C# 10+) - everything in file is in this namespace

public partial class Address
// Address model - represents database table "Address"
// "partial" keyword = this class can be split across multiple files
// Used by Entity Framework for auto-generated code
// Example: EF generates some methods, developer writes others
// In a DbContext, this model maps to a DbSet<Address> table
{
    public int AddressId { get; set; }
    // Primary Key (PK) for Address table
    // int = unique identifier (auto-increment)
    // { get; set; } = auto-property (syntactic sugar for private backing field)
    // Every address record must have unique AddressId
    // Foreign keys in Order and Customer tables reference this AddressId

    public string? City { get; set; }
    // City name (e.g., "Mumbai", "Bangalore")
    // string? = nullable string (can be null)
    // ? means value is optional - user might not provide city
    // Database column: nullable VARCHAR/NVARCHAR
    // In EF: [Column(TypeName = "varchar(max)")] by default
    // Validation: should validate city name length, format in service layer

    public string? Country { get; set; }
    // Country name (e.g., "India", "USA")
    // Nullable - optional field
    // Could also store country code (e.g., "IN", "US") if needed
    // Database column: nullable VARCHAR
    // Used for shipping, tax calculations, restrictions

    public string? HouseNumber { get; set; }
    // House/Building number (e.g., "123", "Building A")
    // Nullable - optional
    // Combined with other fields (Town, City, Country) forms complete address
    // Database column: nullable VARCHAR
    // Example address: "HouseNumber 42, Town Pune, City Pune, State Maharashtra, Country India"

    public string? Landmark { get; set; }
    // Nearby landmark for delivery person reference (e.g., "Near Park", "Next to Hospital")
    // Nullable - optional field
    // Helpful for delivery when exact house number/GPS isn't clear
    // Database column: nullable VARCHAR
    // Not mandatory but improves delivery accuracy

    public string? Pincode { get; set; }
    // Postal/ZIP code (e.g., "411001")
    // Nullable - optional
    // Could be string to support formats: "411001", "411001-1234", "SW1A 1AA" (UK), etc.
    // If stored as int, can't handle international formats
    // Used for location validation, shipping zones, delivery partner routing
    // Database column: nullable VARCHAR
    // Should add validation: length check, format validation in service layer

    public string? State { get; set; }
    // State/Province (e.g., "Maharashtra", "California")
    // Nullable - optional
    // Used for tax calculations, shipping restrictions (liquor delivery to dry states)
    // Database column: nullable VARCHAR
    // Example: Address in Maharashtra state

    public string? Town { get; set; }
    // Town/Area within city (e.g., "Pune", "Bandra", "Andheri")
    // Nullable - optional
    // More specific than City, less specific than House Number
    // Database column: nullable VARCHAR
    // Example: Town "Pune" within City "Pune", State "Maharashtra"

    public int? UserId { get; set; }
    // Foreign Key (FK) referencing User/Customer
    // int? = nullable integer
    // Nullable FK = address might not be assigned to specific user (temporary/draft address)
    // Database column: nullable INT with FK constraint to Customer table
    // Relationship: one User can have many Addresses (1-to-Many)
    // Example: UserId = 5 → points to Customer with Id = 5
    // This field establishes relationship with Customer model

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    // Navigation property - loads related Customer records
    // ICollection<T> = list-like collection that can be added/removed
    // virtual = EF can override this property for lazy loading
    // = new List<Customer>() = initializes empty list (prevents null reference)
    // This represents: which Customers use this Address?
    // One Address can be used by multiple Customers
    // Database: no new column for this - relationship defined by FK
    // Lazy loading: if accessed, EF queries customers linked to this address
    // Eager loading: use .Include(a => a.Customers) in query
    // Example: Address with id=1 is used by Customer ids: 2, 3, 5 (multiple customers same address)
    // Inverse navigation of: Customer.AddressId FK

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    // Navigation property - loads related Order records
    // ICollection<T> = list-like collection
    // virtual = EF can override for lazy loading
    // = new List<Order>() = initializes empty list
    // This represents: which Orders used this Address for delivery?
    // One Address can be used for multiple Orders (shipping)
    // Database: no new column - relationship defined by FK in Order table
    // Example: Address with id=1 received Orders: Order#101, Order#102, Order#103
    // Each Order has AddressId pointing to this Address
    // Lazy loading: if accessed, EF queries orders shipped to this address
    // Eager loading: use .Include(a => a.Orders) in query

    public virtual Customer? User { get; set; }
    // Navigation property - loads the Customer who owns this Address
    // Customer? = nullable (address might not be linked to user)
    // virtual = EF can override for lazy loading
    // This represents: which Customer owns this Address?
    // Relationship direction: Address.User → references Customer
    // Example: Address with UserId=5 has User = Customer(id=5)
    // Database: UserId FK column is foreign key, User property is navigation
    // Lazy loading: if accessed, EF queries Customer with matching id
    // Eager loading: use .Include(a => a.User) in query
    // This is inverse of: Customer.Addresses (if Customer has collection of Addresses)
}
// End of Address class