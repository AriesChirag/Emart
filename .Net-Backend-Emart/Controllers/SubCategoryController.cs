using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emart_DotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategoriesByCategoryId(int categoryId)
        {
            var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
            return Ok(subCategories);
        }
    }
}
