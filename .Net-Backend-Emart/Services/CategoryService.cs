using Emart_DotNet.Models;
using Emart_DotNet.Repositories;

namespace Emart_DotNet.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _repository.GetAllCategoriesAsync();
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            var allCategories = await _repository.GetAllCategoriesAsync();
            return allCategories.Where(c => c.ParentCategoryId == null);
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            return await _repository.GetChildCategoriesAsync(parentId);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _repository.GetCategoryByIdAsync(id);
        }
    }
}
