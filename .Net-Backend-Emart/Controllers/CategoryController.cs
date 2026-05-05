using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emart_DotNet.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;

        public CategoryController(ICategoryService categoryService, ISubCategoryService subCategoryService)
        {
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
        }

        // List Categories (only Parents)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> ListCategories()
        {
            var categories = await _categoryService.GetParentCategoriesAsync();
            return Ok(categories);
        }

        // View sub-categories by category id
        [HttpGet("{categoryId}/subcategories")]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories(int categoryId)
        {
            var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
            return Ok(subCategories);
        }

        // View Child Categories by Parent ID
        [HttpGet("{parentId}/children")]
        public async Task<ActionResult<IEnumerable<Category>>> GetChildCategories(int parentId)
        {
            // Verify parent exists first, similar to Java
            var parent = await _categoryService.GetCategoryByIdAsync(parentId);
            if (parent == null)
            {
                return NotFound(new { message = "Parent Category not found" }); // Matching Java exception message style roughly
            }

            var childCategories = await _categoryService.GetChildCategoriesAsync(parentId);
            return Ok(childCategories);
        }
    }
}
