using Emart_DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId);
    }
}
