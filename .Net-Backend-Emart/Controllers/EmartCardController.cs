using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emart_DotNet.Controllers
{
    [Route("api/emart-card")]
    [ApiController]
    public class EmartCardController : ControllerBase
    {
        private readonly IEmartCardService _emartCardService;

        public EmartCardController(IEmartCardService emartCardService)
        {
            _emartCardService = emartCardService;
        }

        [HttpPost("apply")]
        public async Task<ActionResult<EmartCard>> ApplyForCard([FromBody] ApplyEmartCardRequest request)
        {
            try
            {
                var card = await _emartCardService.ApplyForCardAsync(request);
                return Ok(card);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("use-epoints")]
        public async Task<ActionResult<string>> UseEpoints([FromQuery] int userId, [FromQuery] int pointsUsed)
        {
            try
            {
                await _emartCardService.UseEpointsAsync(userId, pointsUsed);
                return Ok("E-points updated successfully");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("details/{userId}")]
        public async Task<ActionResult<EmartCardDTO>> GetCardDetails(int userId)
        {
            var card = await _emartCardService.GetCardDetailsAsync(userId);
            return Ok(card);
        }
    }
}
