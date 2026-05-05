using AnalyticsEngineApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsEngineApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AnalyticsService _service;

    public ReportsController(AnalyticsService service)
    {
        _service = service;
    }

    [HttpGet("product-offers-inventory")]
    public IActionResult ProductOffersInventory()
    {
        return Ok(_service.GetProductOffersInventory());
    }
}
