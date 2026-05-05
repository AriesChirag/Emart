using Emart_DotNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class ProductUploadController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductUploadController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            try
            {
                await _productService.UploadProductsAsync(file);
                return Ok("Products uploaded successfully");
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
