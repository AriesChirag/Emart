using Emart_DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
    }
}
