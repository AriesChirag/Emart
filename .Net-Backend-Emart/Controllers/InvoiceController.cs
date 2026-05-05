using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("generate/{orderId}")]
        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceForOrderAsync(orderId);
                return Ok(Mappers.InvoiceMapper.ToDTO(invoice));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("view/{invoiceId}")]
        public async Task<IActionResult> ViewInvoiceById(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return NotFound();
                }
                return Ok(Mappers.InvoiceMapper.ToDTO(invoice));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("mail/latest")]
        public async Task<IActionResult> MailLatestInvoice()
        {
            try
            {
                var invoice = await _invoiceService.GetLatestInvoiceAsync();
                if (invoice == null)
                {
                    return BadRequest("No invoice found");
                }

                // Generate PDF
                var pdfBytes = await _invoiceService.GenerateInvoicePdfByInvoiceIdAsync(invoice.InvoiceId);

                // TODO: Send email with PDF attachment
                // For now, return success message. Email service can be implemented later.
                return Ok("Invoice generated. Email service implementation pending.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("export/pdf/{invoiceId}")]
        public async Task<IActionResult> ExportInvoicePdf(int invoiceId)
        {
            try
            {
                var pdfBytes = await _invoiceService.GenerateInvoicePdfByInvoiceIdAsync(invoiceId);
                return File(pdfBytes, "application/pdf", $"invoice_{invoiceId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download/{orderId}")]
        public async Task<IActionResult> DownloadInvoice(int orderId)
        {
            try
            {
                byte[] pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(orderId);
                return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

