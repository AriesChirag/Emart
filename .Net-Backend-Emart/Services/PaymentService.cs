using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Razorpay.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ModelPayment = Emart_DotNet.Models.Payment;
using ModelOrder = Emart_DotNet.Models.Order;

namespace Emart_DotNet.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly string _keyId;
        private readonly string _keySecret;

        public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo, IConfiguration config)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _keyId = config["Razorpay:KeyId"] ?? "";
            _keySecret = config["Razorpay:KeySecret"] ?? "";
        }

        public async Task<ModelPayment> ProcessPaymentAsync(ModelPayment payment)
        {
            payment.PaymentDate = DateTime.Now;
            if (payment.Status == PaymentStatus.Pending) 
                payment.Status = PaymentStatus.Pending;
            
            return await _paymentRepo.SaveAsync(payment); 
        }

        public ModelPayment ProcessPayment(ModelPayment payment) 
        {
             return ProcessPaymentAsync(payment).Result;
        }

        public async Task<PaymentStatus> GetPaymentStatusAsync(int orderId)
        {
            var payment = await _paymentRepo.FindByOrderIdAsync(orderId);
            if (payment == null) throw new Exception("Payment not found for order");
            return payment.Status ?? PaymentStatus.Pending;
        }
        
        public PaymentStatus GetPaymentStatus(int orderId)
        {
            return GetPaymentStatusAsync(orderId).Result;
        }

        public async Task<PaymentMethod> GetPaymentMethodAsync(int orderId)
        {
            var payment = await _paymentRepo.FindByOrderIdAsync(orderId);
            if (payment == null) throw new Exception("Payment mode not found for order");
            return payment.Mode ?? PaymentMethod.Cash;
        }

        public PaymentMethod GetPaymentMethod(int orderId)
        {
            return GetPaymentMethodAsync(orderId).Result;
        }

        // Razorpay Implementation - Fixed to return proper JSON
        public string CreateRazorpayOrder(int orderId)
        {
            var order = _orderRepo.FindByIdAsync(orderId).Result;

            if (order == null) throw new Exception("Order not found");
            
            double amount = (double)(order.TotalAmount ?? 0);

            RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", (int)(amount * 100)); // paise
            options.Add("currency", "INR");
            options.Add("receipt", "receipt_" + orderId);

            Razorpay.Api.Order razorOrder = client.Order.Create(options);
            string razorOrderId = razorOrder["id"].ToString();

            // Save Payment as Pending
            ModelPayment payment = new ModelPayment
            {
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                Mode = PaymentMethod.Razorpay,
                RazorpayOrderId = razorOrderId,
                OrderId = orderId, 
            };

            _paymentRepo.SaveAsync(payment).Wait();

            // Return proper JSON with Razorpay order attributes
            var responseObj = new Dictionary<string, object>
            {
                { "id", razorOrder["id"].ToString() },
                { "entity", razorOrder["entity"]?.ToString() ?? "order" },
                { "amount", razorOrder["amount"] },
                { "amount_paid", razorOrder["amount_paid"] ?? 0 },
                { "amount_due", razorOrder["amount_due"] ?? razorOrder["amount"] },
                { "currency", razorOrder["currency"]?.ToString() ?? "INR" },
                { "receipt", razorOrder["receipt"]?.ToString() ?? "" },
                { "status", razorOrder["status"]?.ToString() ?? "created" },
                { "created_at", razorOrder["created_at"] ?? 0 }
            };
            
            return JsonSerializer.Serialize(responseObj);
        }

        public string CreateRazorpayOrder(double amount)
        {
            RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", (int)(amount * 100));
            options.Add("currency", "INR");
            options.Add("receipt", "receipt_txn_" + DateTimeOffset.Now.ToUnixTimeMilliseconds());

            Razorpay.Api.Order razorOrder = client.Order.Create(options);
            
            // Return proper JSON with Razorpay order attributes
            var responseObj = new Dictionary<string, object>
            {
                { "id", razorOrder["id"].ToString() },
                { "entity", razorOrder["entity"]?.ToString() ?? "order" },
                { "amount", razorOrder["amount"] },
                { "amount_paid", razorOrder["amount_paid"] ?? 0 },
                { "amount_due", razorOrder["amount_due"] ?? razorOrder["amount"] },
                { "currency", razorOrder["currency"]?.ToString() ?? "INR" },
                { "receipt", razorOrder["receipt"]?.ToString() ?? "" },
                { "status", razorOrder["status"]?.ToString() ?? "created" },
                { "created_at", razorOrder["created_at"] ?? 0 }
            };
            
            return JsonSerializer.Serialize(responseObj);
        }

        public ModelPayment VerifyRazorpayPayment(int orderId, ModelPayment paymentDetails)
        {
            var dbPayment = _paymentRepo.FindByOrderIdAsync(orderId).Result;
            
            if (dbPayment == null)
            {
                var order = _orderRepo.FindByIdAsync(orderId).Result;
                if (order == null) throw new Exception("Order not found for verification");

                dbPayment = new ModelPayment
                {
                    OrderId = orderId,
                    Amount = (double)(order.TotalAmount ?? 0),
                    PaymentDate = DateTime.Now,
                    Mode = PaymentMethod.Razorpay,
                    Status = PaymentStatus.Pending
                };
                dbPayment = _paymentRepo.SaveAsync(dbPayment).Result;
            }

            string data = paymentDetails.RazorpayOrderId + "|" + paymentDetails.RazorpayPaymentId;
            string generatedSignature = HmacSha256(data, _keySecret);

            if (generatedSignature == paymentDetails.RazorpaySignature)
            {
                dbPayment.Status = PaymentStatus.Paid;
                dbPayment.TransactionId = paymentDetails.RazorpayPaymentId;
                dbPayment.RazorpayPaymentId = paymentDetails.RazorpayPaymentId;
                dbPayment.RazorpaySignature = paymentDetails.RazorpaySignature;
                dbPayment.RazorpayOrderId = paymentDetails.RazorpayOrderId;
                
                return _paymentRepo.SaveAsync(dbPayment).Result;
            }
            else
            {
                dbPayment.Status = PaymentStatus.Failed;
                return _paymentRepo.SaveAsync(dbPayment).Result;
            }
        }

        public ModelPayment CreateCashOnDeliveryPayment(int orderId)
        {
             var order = _orderRepo.FindByIdAsync(orderId).Result;
             if (order == null) throw new Exception("Order not found");

             ModelPayment payment = new ModelPayment
             {
                 OrderId = orderId,
                 Amount = (double)(order.TotalAmount ?? 0),
                 PaymentDate = DateTime.Now,
                 Mode = PaymentMethod.Cash,
                 Status = PaymentStatus.Pending
             };

             return _paymentRepo.SaveAsync(payment).Result;
        }

        private string HmacSha256(string data, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
