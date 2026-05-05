using Emart_DotNet.Models;

namespace Emart_DotNet.DTOs
{
    public class PlaceOrderRequestDTO
    {
        public int UserId { get; set; }
        public string DeliveryType { get; set; } // Using String to match Enum conversion or use Enum
        public string PaymentMethod { get; set; } // Using string for easy mapping

        public int? AddressId { get; set; } // for HOME_DELIVERY
        public int? StoreId { get; set; } // for STORE_PICKUP
    }
}
