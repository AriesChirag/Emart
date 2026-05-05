using System;
using System.Collections.Generic;

namespace Emart_DotNet.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public double? Amount { get; set; }

    public PaymentMethod? Mode { get; set; }
    public DateTime? PaymentDate { get; set; }

    public string? RazorpayOrderId { get; set; }

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpaySignature { get; set; }

    public PaymentStatus? Status { get; set; }
    public string? TransactionId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
