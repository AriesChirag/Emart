using Emart_DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public interface ISubCategoryRepository
    {
        Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId);
    }
}
