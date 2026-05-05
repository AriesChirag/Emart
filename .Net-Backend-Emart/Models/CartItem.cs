using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? EpointsUsed { get; set; }

    public string? PurchaseType { get; set; }

    public int? Quantity { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? CartId { get; set; }

    public int? ProductId { get; set; }

    [JsonIgnore]
    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
