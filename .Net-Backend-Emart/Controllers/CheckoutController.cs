using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emart_DotNet.Controllers
{
    [Route("api/checkout")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("delivery")]
        public async Task<IActionResult> SelectDeliveryOption([FromQuery] int userId, [FromQuery] string deliveryType)
        {
            try
            {
                var result = await _checkoutService.SelectDeliveryOptionAsync(userId, deliveryType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromQuery] int userId)
        {
            try
            {
                var result = await _checkoutService.PlaceOrderAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
