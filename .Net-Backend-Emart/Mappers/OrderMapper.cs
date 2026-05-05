using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace Emart_DotNet.Mappers
{
    public static class OrderMapper
    {
        public static OrderResponseDTO ToDTO(Order order)
        {
            if (order == null) return null;

            var dto = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus?.ToString(),
                PaymentMethod = order.PaymentMethod?.ToString(),
                DeliveryType = order.DeliveryType?.ToString(),
                TotalAmount = order.TotalAmount,
                EpointsUsed = order.EpointsUsed,
                EpointsEarned = order.EpointsEarned
            };

            // Customer info (User property in .NET model)
            if (order.User != null)
            {
                dto.CustomerId = order.User.UserId;
                dto.CustomerName = order.User.FullName;
                dto.CustomerEmail = order.User.Email;
            }

            // Address info
            if (order.Address != null)
            {
                dto.AddressId = order.Address.AddressId;
                dto.AddressLine = $"{order.Address.HouseNumber}, {order.Address.Town}";
                dto.City = order.Address.City;
                dto.State = order.Address.State;
                dto.Pincode = order.Address.Pincode;
            }

            // Order items
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                dto.OrderItems = order.OrderItems.Select(item => new OrderItemDTO
                {
                    ProductName = item.Product?.ProductName,
                    ImageUrl = item.Product?.ProductImageUrl,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Subtotal = item.Subtotal
                }).ToList();
            }

            return dto;
        }

        public static List<OrderResponseDTO> ToDTOList(IEnumerable<Order> orders)
        {
            return orders?.Select(ToDTO).ToList() ?? new List<OrderResponseDTO>();
        }
    }
}
