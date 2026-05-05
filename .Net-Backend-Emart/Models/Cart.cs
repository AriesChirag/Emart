using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public decimal? CouponDiscount { get; set; }

    public int? EarnedEpoints { get; set; }

    public decimal? EpointDiscount { get; set; }

    public decimal? FinalPayableAmount { get; set; }

    public decimal? PlatformFee { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? TotalMrp { get; set; }

    public int? UsedEpoints { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public virtual Customer? User { get; set; }
}
