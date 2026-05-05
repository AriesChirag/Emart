using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public interface IPaymentRepository
    {
        Payment Save(Payment payment);
        Payment? FindByOrderId(int orderId);
        Task<Payment?> FindByOrderIdAsync(int orderId);
        Task<Payment> SaveAsync(Payment payment);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Payment Save(Payment payment)
        {
            if (payment.PaymentId == 0)
            {
                _context.Payments.Add(payment);
            }
            else
            {
                _context.Payments.Update(payment);
            }
            _context.SaveChanges();
            return payment;
        }

        public Payment? FindByOrderId(int orderId)
        {
            return _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
        }

        public async Task<Payment?> FindByOrderIdAsync(int orderId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<Payment> SaveAsync(Payment payment)
        {
             if (payment.PaymentId == 0)
            {
                _context.Payments.Add(payment);
            }
            else
            {
                _context.Payments.Update(payment);
            }
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
