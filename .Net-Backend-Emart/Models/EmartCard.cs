using System;
using System.Collections.Generic;

namespace Emart_DotNet.Models;

public partial class EmartCard
{
    public int CardId { get; set; }

    public double AnnualIncome { get; set; }

    public string BankDetails { get; set; } = null!;

    public string? EducationQualification { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string? Occupation { get; set; }

    public string PanCard { get; set; } = null!;

    public DateOnly PurchaseDate { get; set; }

    public string? Status { get; set; }

    public int? TotalEpointsUsed { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
