using System;
using System.Collections.Generic;

namespace Emart_DotNet.DTOs
{
    public class CartResponseDTO
    {
        public decimal TotalMrp { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal EpointDiscount { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal FinalPayableAmount { get; set; }
        public decimal TotalAmount { get; set; }
        
        public int UsedEpoints { get; set; }
        public int EarnedEpoints { get; set; }
        public int AvailableEpoints { get; set; }
        
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }

    public class CartItemDTO
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string PurchaseType { get; set; }
        public int EpointsUsed { get; set; }
        public string ImageUrl { get; set; }
    }
}
