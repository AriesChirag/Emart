using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(
            [FromQuery] int userId,
            [FromQuery] int productId,
            [FromQuery] int quantity,
            [FromQuery] string purchaseType = "NORMAL",
            [FromQuery] int epointsUsed = 0)
        {
            try
            {
                var item = await _cartService.AddToCartAsync(userId, productId, quantity, purchaseType, epointsUsed);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromQuery] int cartItemId, [FromQuery] int quantity)
        {
            try
            {
                var item = await _cartService.UpdateQuantityAsync(cartItemId, quantity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(cartItemId);
                return Ok("Item removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetCartSummary(int userId)
        {
            try
            {
                var summary = await _cartService.GetCartSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                await _cartService.ClearCartByUserAsync(userId);
                return Ok("Cart cleared");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
 
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> ViewCart(int userId)
        {

            
            return Ok(await _cartService.GetCartSummaryAsync(userId)); // Fallback or strict?
        }
    }
}
