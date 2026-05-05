using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Emart_DotNet.Models;

namespace Emart_DotNet.Converters
{
    /// <summary>
    /// Custom converters to handle Java-style enum values stored in the database.
    /// Java/Hibernate stores enums like "CONFIRMED", "HOME_DELIVERY", "CASH"
    /// .NET needs to convert these to/from PascalCase enum values.
    /// </summary>
    
    public class OrderStatusConverter : ValueConverter<OrderStatus, int>
    {
        public OrderStatusConverter() : base(
            v => ToOrdinal(v),
            v => FromOrdinal(v))
        { }

        private static int ToOrdinal(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => 0,
                OrderStatus.Confirmed => 1,
                OrderStatus.Packed => 2,
                OrderStatus.Shipped => 3,
                OrderStatus.Delivered => 4,
                OrderStatus.Cancelled => 5,
                _ => (int)status
            };
        }

        private static OrderStatus FromOrdinal(int value)
        {
            return value switch
            {
                0 => OrderStatus.Pending,
                1 => OrderStatus.Confirmed,
                2 => OrderStatus.Packed,
                3 => OrderStatus.Shipped,
                4 => OrderStatus.Delivered,
                5 => OrderStatus.Cancelled,
                _ => OrderStatus.Pending
            };
        }
    }

    public class DeliveryTypeConverter : ValueConverter<DeliveryType?, string?>
    {
        public DeliveryTypeConverter() : base(
            v => ToJavaStyle(v),
            v => FromJavaStyle(v))
        { }

        private static string? ToJavaStyle(DeliveryType? type)
        {
            if (type == null) return null;
            return type switch
            {
                DeliveryType.HomeDelivery => "HOME_DELIVERY",
                DeliveryType.Store => "STORE_PICKUP",
                _ => type.ToString()?.ToUpperInvariant()
            };
        }

        private static DeliveryType? FromJavaStyle(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            
            var normalized = value.Replace("_", "").ToUpperInvariant();
            return normalized switch
            {
                "HOMEDELIVERY" => DeliveryType.HomeDelivery,
                "STOREPICKUP" or "STORE" => DeliveryType.Store,
                _ => Enum.TryParse<DeliveryType>(value, true, out var result) ? result : null
            };
        }
    }

    public class PaymentMethodConverter : ValueConverter<PaymentMethod?, string?>
    {
        public PaymentMethodConverter() : base(
            v => ToJavaStyle(v),
            v => FromJavaStyle(v))
        { }

        private static string? ToJavaStyle(PaymentMethod? method)
        {
            if (method == null) return null;
            return method switch
            {
                PaymentMethod.Cash => "CASH",
                PaymentMethod.Razorpay => "RAZORPAY",
                _ => method.ToString()?.ToUpperInvariant()
            };
        }

        private static PaymentMethod? FromJavaStyle(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            
            return value.ToUpperInvariant() switch
            {
                "CASH" or "COD" => PaymentMethod.Cash,
                "RAZORPAY" or "ONLINE" => PaymentMethod.Razorpay,
                _ => Enum.TryParse<PaymentMethod>(value, true, out var result) ? result : null
            };
        }
    }

    public class PaymentStatusConverter : ValueConverter<PaymentStatus?, string?>
    {
        public PaymentStatusConverter() : base(
            v => ToJavaStyle(v),
            v => FromJavaStyle(v))
        { }

        private static string? ToJavaStyle(PaymentStatus? status)
        {
            if (status == null) return null;
            return status switch
            {
                PaymentStatus.Pending => "PENDING",
                PaymentStatus.Paid => "PAID",
                PaymentStatus.Failed => "FAILED",
                _ => status.ToString()?.ToUpperInvariant()
            };
        }

        private static PaymentStatus? FromJavaStyle(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            
            return value.ToUpperInvariant() switch
            {
                "PENDING" => PaymentStatus.Pending,
                "PAID" => PaymentStatus.Paid,
                "FAILED" => PaymentStatus.Failed,
                _ => Enum.TryParse<PaymentStatus>(value, true, out var result) ? result : null
            };
        }
    }
}
