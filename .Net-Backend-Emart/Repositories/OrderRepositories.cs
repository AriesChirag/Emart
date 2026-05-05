using Emart_DotNet.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Emart_DotNet.Repositories
{

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> SaveAsync(Order order)
        {
            if (order.OrderId == 0)
                _context.Orders.Add(order);
            else
                _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> FindByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Invoices) // Changed from CustomerInvoices (check model)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .Include(o => o.Store)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> FindByCustomerUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .Include(o => o.Store)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }

    // OrderItem Repository
    public interface IOrderItemRepository
    {
        Task<OrderItem> SaveAsync(OrderItem orderItem);
    }

    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;
        public OrderItemRepository(AppDbContext context) { _context = context; }

        public async Task<OrderItem> SaveAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem); 
            await _context.SaveChangesAsync();
            return orderItem;
        }
    }
}
