using System;
using System.Collections.Generic;

namespace Emart_DotNet.DTOs
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? DeliveryType { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? EpointsUsed { get; set; }
        public int? EpointsEarned { get; set; }
        
        // Customer info
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        
        // Address info
        public int? AddressId { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        
        // Store info (if store pickup)
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? StoreCity { get; set; }
        
        // Order items
        [System.Text.Json.Serialization.JsonPropertyName("items")]
        public List<OrderItemDTO>? OrderItems { get; set; }
    }

    public class OrderItemDTO
    {
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Subtotal { get; set; }
        public string? ImageUrl { get; set; }
    }
}
