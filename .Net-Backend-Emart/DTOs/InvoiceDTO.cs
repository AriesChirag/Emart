using System;
using System.Collections.Generic;

namespace Emart_DotNet.DTOs
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        
        public decimal? TotalAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        
        // Shipping Info
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? ShippingAddress { get; set; } // Formatted address string
        
        // Items
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        
        // E-points
        public int EpointsUsed { get; set; }
        public int EpointsEarned { get; set; }
    }
}
