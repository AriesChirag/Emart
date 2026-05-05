using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] Payment payment)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(payment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetStatus(int orderId)
        {
            try
            {
                var status = await _paymentService.GetPaymentStatusAsync(orderId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("method/{orderId}")]
        public async Task<IActionResult> GetPaymentMethod(int orderId)
        {
            try
            {
                var method = await _paymentService.GetPaymentMethodAsync(orderId);
                return Ok(method);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-razorpay-order/{orderId}")]
        public IActionResult CreateRazorpayOrderById(int orderId)
        {
            try
            {
                string response = _paymentService.CreateRazorpayOrder(orderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromQuery] double amount)
        {
            try
            {
                string response = _paymentService.CreateRazorpayOrder(amount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-razorpay-payment/{orderId}")]
        public IActionResult VerifyRazorpayPayment(int orderId, [FromBody] Payment paymentDetails)
        {
            try
            {
                var payment = _paymentService.VerifyRazorpayPayment(orderId, paymentDetails);
                if (payment.Status == PaymentStatus.Paid)
                {
                    return Ok(payment);
                }
                return BadRequest(new { message = "Payment Verification Failed", payment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("cod/{orderId}")]
        public IActionResult CashOnDelivery(int orderId)
        {
            try
            {
                var payment = _paymentService.CreateCashOnDeliveryPayment(orderId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

