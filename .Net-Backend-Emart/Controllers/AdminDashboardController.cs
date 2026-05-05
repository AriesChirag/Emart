using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly IOrderService _orderService;

        public AdminDashboardController(IAdminDashboardService dashboardService, IOrderService orderService)
        {
            _dashboardService = dashboardService;
            _orderService = orderService;
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] string status)
        {
            try
            {
                var result = await _dashboardService.UpdateOrderStatusAsync(orderId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            try
            {
                var orders = await _dashboardService.GetAllOrdersAsync(page, size);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("products/upload-csv")]
        public async Task<IActionResult> UploadProductsCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                // TODO: Implement CSV processing through ProductCsvService
                // For now, return a placeholder response
                return Ok(new { success = true, message = "CSV upload endpoint ready. Implementation pending." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
