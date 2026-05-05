using Emart_DotNet.DTOs;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/order")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequestDTO req)
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(req);
                
                // Simple log: Order placed | UserId | Date | Time | Amount
                _logger.LogInformation("ORDER PLACED | OrderId: {OrderId} | UserId: {UserId} | Date: {Date} | Time: {Time} | Amount: {Amount}", 
                    order.OrderId, 
                    req.UserId, 
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    DateTime.Now.ToString("HH:mm:ss"),
                    order.TotalAmount);
                
                return Ok(order);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null 
                    ? $"{ex.Message} -> {ex.InnerException.Message}" 
                    : ex.Message;
                _logger.LogError("ORDER FAILED | UserId: {UserId} | Error: {Error}", req.UserId, message);
                return BadRequest(message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


