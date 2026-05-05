using Emart_DotNet.Models;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public interface IPaymentService
    {
        Payment ProcessPayment(Payment payment);
        Task<Payment> ProcessPaymentAsync(Payment payment);
        
        PaymentStatus GetPaymentStatus(int orderId);
        Task<PaymentStatus> GetPaymentStatusAsync(int orderId);
        
        PaymentMethod GetPaymentMethod(int orderId);
        Task<PaymentMethod> GetPaymentMethodAsync(int orderId);
        
        // Razorpay
        string CreateRazorpayOrder(int orderId);
        string CreateRazorpayOrder(double amount);
        Payment VerifyRazorpayPayment(int orderId, Payment paymentDetails); 
        
        // COD
        Payment CreateCashOnDeliveryPayment(int orderId);
    }
}