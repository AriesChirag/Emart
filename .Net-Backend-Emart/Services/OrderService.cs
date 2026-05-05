using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ICartRepository _cartRepo;
        private readonly ICartItemRepository _cartItemRepo;
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailService _emailService;


        public OrderService(
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            ICartRepository cartRepo,
            ICartItemRepository cartItemRepo,
            IOrderItemRepository orderItemRepo,
            IAddressRepository addressRepo,
            IStoreRepository storeRepo,
            IInvoiceService invoiceService,
            IEmailService emailService)
        {
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _orderItemRepo = orderItemRepo;
            _addressRepo = addressRepo;
            _storeRepo = storeRepo;
            _invoiceService = invoiceService;
            _emailService = emailService;
        }

        public async Task<Order> PlaceOrderAsync(PlaceOrderRequestDTO req)
        {
            var customer = await _customerRepo.FindByUserIdAsync(req.UserId)
                           ?? throw new Exception("Customer not found");

            var cart = await _cartRepo.FindByCustomerAsync(customer)
                       ?? throw new Exception("Cart not found");
            
            var cartItems = await _cartItemRepo.FindByCartAsync(cart.CartId);
            if (!cartItems.Any())
            {
                throw new Exception("Cart is empty");
            }
            
            // Validate e-Points sufficiency again just in case
            if ((cart.UsedEpoints ?? 0) > (customer.Epoints ?? 0))
            {
                throw new Exception("Insufficient e-points");
            }
            
            Order order = new Order
            {
                UserId = customer.UserId,
                CartId = cart.CartId,
                OrderDate = DateTime.Now,
                EpointsUsed = cart.UsedEpoints,
                EpointsEarned = cart.EarnedEpoints,
                TotalAmount = cart.FinalPayableAmount
            };

            // Status Enum Mapping
            if (Enum.TryParse<OrderStatus>("Confirmed", true, out var statusEnum))
            {
                 order.Status = statusEnum; // Default to Confirmed as per Java
            }
            else
            {
                 order.Status = OrderStatus.Confirmed; 
            }

            // Delivery Type Mapping - Handle both Java-style (HOME_DELIVERY, STORE_PICKUP) and .NET-style values
            var deliveryTypeStr = req.DeliveryType?.Replace("_", "").ToUpperInvariant();
            if (deliveryTypeStr == "HOMEDELIVERY" || req.DeliveryType?.ToUpperInvariant() == "HOME_DELIVERY")
            {
                order.DeliveryType = DeliveryType.HomeDelivery;
            }
            else if (deliveryTypeStr == "STORE" || deliveryTypeStr == "STOREPICKUP" || 
                     req.DeliveryType?.ToUpperInvariant() == "STORE_PICKUP")
            {
                order.DeliveryType = DeliveryType.Store;
            }
            else if (Enum.TryParse<DeliveryType>(req.DeliveryType, true, out var deliveryTypeEnum))
            {
                order.DeliveryType = deliveryTypeEnum;
            }
            else
            {
                // Default to HomeDelivery if parsing fails (to prevent unintended store lookups)
                order.DeliveryType = DeliveryType.HomeDelivery;
            }

            // Payment Method Mapping - Handle both Java-style (CASH, RAZORPAY) and .NET-style values
            var paymentMethodStr = req.PaymentMethod?.Replace("_", "").ToUpperInvariant();
            if (paymentMethodStr == "CASH" || paymentMethodStr == "COD")
            {
                order.PaymentMethod = PaymentMethod.Cash;
            }
            else if (paymentMethodStr == "RAZORPAY" || paymentMethodStr == "ONLINE")
            {
                order.PaymentMethod = PaymentMethod.Razorpay;
            }
            else if (Enum.TryParse<PaymentMethod>(req.PaymentMethod, true, out var pmEnum))
            {
                order.PaymentMethod = pmEnum;
            }

            // Address or Store
            if (order.DeliveryType == DeliveryType.HomeDelivery)
            {
                var address = await _addressRepo.FindByIdAsync(req.AddressId ?? 0)
                              ?? throw new Exception("Address not found");
                order.AddressId = address.AddressId;
            }
            else
            {
                var store = await _storeRepo.FindByIdAsync(req.StoreId ?? 0)
                             ?? throw new Exception("Store not found");
                order.StoreId = store.StoreId;
            }

            // Payment Status
            order.PaymentStatus = (order.PaymentMethod == PaymentMethod.Cash) 
                                  ? PaymentStatus.Pending 
                                  : PaymentStatus.Paid;

            // Save Order
            order = await _orderRepo.SaveAsync(order);

            // Create Order Items
            foreach (var ci in cartItems)
            {
                OrderItem oi = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = ci.ProductId ?? 0,
                    Quantity = ci.Quantity ?? 0,
                    Price = ci.Product?.NormalPrice,
                    Subtotal = ci.Subtotal
                };
                await _orderItemRepo.SaveAsync(oi);
            }

            // Update E-Points
            customer.Epoints = (customer.Epoints ?? 0) - (cart.UsedEpoints ?? 0) + (cart.EarnedEpoints ?? 0);
            await _customerRepo.SaveAsync(customer);
            
            // Generate Invoice
            await _invoiceService.CreateInvoiceForOrderAsync(order.OrderId);

            // Generate COD Payment (Placeholder)
            // if (order.PaymentMethod == PaymentMethod.Cash) ...

            // Clear Cart
            await _cartItemRepo.DeleteByCartIdAsync(cart.CartId);
            cart.TotalMrp = 0;
            cart.FinalPayableAmount = 0;
            cart.UsedEpoints = 0;
            cart.EpointDiscount = 0;
            cart.EarnedEpoints = 0;
            await _cartRepo.SaveAsync(cart);

             // Send Email with Invoice
             try
             {
                 var pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(order.OrderId);
                 string subject = "Order Confirmation - Order #" + order.OrderId;
                 string body = $"Dear {customer.FullName},<br/><br/>Your order has been placed successfully. Please find the attached invoice.<br/><br/>Regards,<br/>Emart Team";
                 
                 await _emailService.SendPdfAsync(customer.Email, subject, body, pdfBytes, $"Invoice_{order.OrderId}.pdf");
             }
             catch(Exception ex)
             {
                 // Log error but don't fail the order flow
                 Console.WriteLine($"Failed to send email: {ex.Message}");
             }

             return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepo.FindByIdAsync(orderId)
                        ?? throw new Exception("Order not found");
            
            if (Enum.TryParse<OrderStatus>(status, true, out var st))
            {
                order.Status = st;
            }
            return await _orderRepo.SaveAsync(order);
        }

        public async Task<List<OrderResponseDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.FindByCustomerUserIdAsync(userId);
            return orders.Select(MapToDTO).ToList();
        }

        public async Task<OrderResponseDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepo.FindByIdAsync(orderId)
                   ?? throw new Exception("Order not found");
            return MapToDTO(order);
        }

        private OrderResponseDTO MapToDTO(Order order)
        {
            if (order == null) return null;

            var dto = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus?.ToString().ToUpper(),
                PaymentMethod = order.PaymentMethod?.ToString().ToUpper(),
                DeliveryType = order.DeliveryType == DeliveryType.HomeDelivery ? "HOME_DELIVERY" : 
                               order.DeliveryType == DeliveryType.Store ? "STORE" : 
                               order.DeliveryType?.ToString().ToUpper(),
                TotalAmount = order.TotalAmount,
                EpointsUsed = order.EpointsUsed,
                EpointsEarned = order.EpointsEarned
            };

            // Customer info
            if (order.User != null)
            {
                dto.CustomerId = order.User.UserId;
                dto.CustomerName = order.User.FullName;
                dto.CustomerEmail = order.User.Email;
            }

            // Address info (for home delivery)
            if (order.Address != null)
            {
                dto.AddressId = order.Address.AddressId;
                dto.AddressLine = $"{order.Address.HouseNumber ?? ""}{(string.IsNullOrEmpty(order.Address.Landmark) ? "" : ", " + order.Address.Landmark)}";
                dto.City = order.Address.City;
                dto.State = order.Address.State;
                dto.Pincode = order.Address.Pincode;
            }

            // Store info (for store pickup)
            if (order.Store != null)
            {
                dto.StoreId = order.Store.StoreId;
                dto.StoreName = order.Store.StoreName;
                dto.StoreCity = order.Store.City;
            }

            // Order Items
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                dto.OrderItems = order.OrderItems.Select(item => new OrderItemDTO
                {
                    ProductName = item.Product?.ProductName ?? "Unknown Product",
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Subtotal = item.Subtotal
                }).ToList();
            }

            return dto;
        }
    }
}
