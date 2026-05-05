using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DeliveryType? DeliveryType { get; set; }
    
    public int? EpointsEarned { get; set; }

    public int? EpointsUsed { get; set; }

    public DateTime? OrderDate { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }
   
    public PaymentStatus? PaymentStatus { get; set; }
    
    public OrderStatus Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? AddressId { get; set; }

    public int? CartId { get; set; }

    public int? UserId { get; set; }

    public int? StoreId { get; set; }

    [JsonIgnore]
    public virtual Address? Address { get; set; }

    [JsonIgnore]
    public virtual Cart? Cart { get; set; }

    [InverseProperty("Order")]
    [JsonIgnore]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();



    [JsonIgnore]
    public virtual Store? Store { get; set; }

    [JsonIgnore]
    public virtual Customer? User { get; set; }
}
