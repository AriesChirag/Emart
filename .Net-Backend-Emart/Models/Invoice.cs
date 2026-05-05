using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emart_DotNet.Models;

[Table("customer_invoice")]
public partial class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("billing_address")]
    [StringLength(255)]
    public string? BillingAddress { get; set; }

    [Column("delivery_type")]
    public DeliveryType? DeliveryType { get; set; }

    [Column("discount_amount")]
    public decimal? DiscountAmount { get; set; }

    [Column("epoints_balance")]
    public int EpointsBalance { get; set; }

    [Column("epoints_earned")]
    public int EpointsEarned { get; set; }

    [Column("epoints_used")]
    public int EpointsUsed { get; set; }

    [Column("order_date")]
    public DateTime? OrderDate { get; set; }

    [Column("shipping_address")]
    [StringLength(255)]
    public string? ShippingAddress { get; set; }

    [Column("tax_amount")]
    public decimal? TaxAmount { get; set; }

    [Column("total_amount")]
    public decimal? TotalAmount { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Invoices")]
    public virtual Order? Order { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Invoices")]
    public virtual Customer? User { get; set; }
}
