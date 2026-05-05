using Emart_DotNet.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/admin/analytics")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string BASE_URL = "http://localhost:8081/api/reports";

        public AdminAnalyticsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("product-offers-inventory")]
        public async Task<IActionResult> ProductOffersInventory()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{BASE_URL}/product-offers-inventory");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<ProductOfferInventoryDTO>>(content, options);
                    return Ok(data);
                }
                return StatusCode((int)response.StatusCode, "Failed to fetch analytics data");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
