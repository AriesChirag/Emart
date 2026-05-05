using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emart_DotNet.Models;

public partial class Customer
{
    public int UserId { get; set; }

    public string AuthProvider { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string Email { get; set; } = null!;

    public int? Epoints { get; set; }

    public string FullName { get; set; } = null!;

    public string? Interests { get; set; }

    public string? MembershipNumber { get; set; }

    public string? Mobile { get; set; }

    public string? Password { get; set; }

    public ulong? ProfileCompleted { get; set; }

    public ulong? PromotionalEmail { get; set; }

    public string? Role { get; set; }

    public int? AddressId { get; set; }

    public int? CardHolderId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual EmartCard? CardHolder { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("User")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
