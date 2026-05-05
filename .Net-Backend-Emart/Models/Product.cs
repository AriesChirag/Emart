using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

public partial class Product
{
    [JsonPropertyName("id")]
    public int ProductId { get; set; }

    [JsonPropertyName("availableQuantity")]
    public int? AvailableQuantity { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("discountPercent")]
    public decimal? DiscountPercent { get; set; }

    [JsonPropertyName("ecardPrice")]
    public decimal? EcardPrice { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ProductImageUrl { get; set; }

    [JsonPropertyName("name")]
    public string ProductName { get; set; } = null!;

    [JsonPropertyName("normalPrice")]
    public decimal NormalPrice { get; set; }

    [JsonPropertyName("storeId")]
    public int? StoreId { get; set; }

    [JsonPropertyName("subCategoryId")]
    public int? SubcategoryId { get; set; }

    [JsonIgnore]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [JsonPropertyName("subCategory")]
    public virtual SubCategory? Subcategory { get; set; }
}
