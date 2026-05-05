using Emart_DotNet.Models;

namespace Emart_DotNet.Repositories
{
    public interface IStoreRepository
    {
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<Store?> GetStoreByIdAsync(int storeId);
        Task<Store?> FindByIdAsync(int storeId);
        Task<IEnumerable<Store>> GetStoresByCityAsync(string city);
    }
}
