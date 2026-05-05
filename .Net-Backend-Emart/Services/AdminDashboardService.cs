using Emart_DotNet.DTOs;
using Emart_DotNet.Mappers;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;

        public AdminDashboardService(AppDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
        {
            var stats = new Dictionary<string, object>();

            // Total orders
            var totalOrders = await _context.Orders.CountAsync();
            stats["totalOrders"] = totalOrders;

            // Total revenue
            var totalRevenue = await _context.Orders
                .SumAsync(o => o.TotalAmount ?? 0);
            stats["totalRevenue"] = totalRevenue;

            // Total users
            var totalUsers = await _context.Customers.CountAsync();
            stats["totalUsers"] = totalUsers;

            // Pending orders
            var pendingOrders = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending);
            stats["pendingOrders"] = pendingOrders;

            return stats;
        }

        public async Task<PagedResult<OrderResponseDTO>> GetAllOrdersAsync(int page, int size)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .OrderByDescending(o => o.OrderDate);

            var totalElements = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalElements / (double)size);

            var orders = await query
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            var dtos = orders.Select(o => OrderMapper.ToDTO(o)).ToList();

            return new PagedResult<OrderResponseDTO>
            {
                Content = dtos,
                TotalElements = totalElements,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = size
            };
        }

        public async Task<OrderResponseDTO> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            // Parse status string to enum
            if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                order.Status = orderStatus;
            }
            else
            {
                throw new Exception($"Invalid order status: {status}");
            }

            await _context.SaveChangesAsync();

            return OrderMapper.ToDTO(order);
        }
    }
}
