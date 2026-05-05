using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using System.Linq;

namespace Emart_DotNet.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceDTO ToDTO(Invoice invoice)
        {
            if (invoice == null) return null;

            var dto = new InvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                OrderId = invoice.OrderId ?? 0,
                OrderDate = invoice.OrderDate,
                TotalAmount = invoice.TotalAmount,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                EpointsUsed = invoice.EpointsUsed,
                EpointsEarned = invoice.EpointsEarned,
                
                CustomerName = invoice.User?.FullName,
                CustomerEmail = invoice.User?.Email
            };

            // Map Address
            if (invoice.Order?.Address != null)
            {
                var addr = invoice.Order.Address;
                dto.ShippingAddress = $"{addr.HouseNumber}, {addr.Town}, {addr.City}, {addr.State} - {addr.Pincode}";
            }
            else
            {
                dto.ShippingAddress = invoice.ShippingAddress; // Fallback to flat field if populated
            }

            // Map Items
            if (invoice.Order?.OrderItems != null)
            {
                dto.Items = invoice.Order.OrderItems.Select(item => new OrderItemDTO
                {
                    ProductName = item.Product?.ProductName ?? "Unknown Product",
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Subtotal = item.Subtotal,
                    ImageUrl = item.Product?.ProductImageUrl
                }).ToList();
            }

            return dto;
        }
    }
}
