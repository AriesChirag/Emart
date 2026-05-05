using System.Text.Json.Serialization;

namespace Emart_DotNet.DTOs
{
    public class ProductOfferInventoryDTO
    {
        [JsonPropertyName("product_Name")]
        public string ProductName { get; set; }

        [JsonPropertyName("discount_Offer")]
        public string DiscountOffer { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
