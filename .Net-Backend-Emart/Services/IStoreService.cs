using Emart_DotNet.Models;

namespace Emart_DotNet.Services
{
    public interface IStoreService
    {
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<Store?> GetStoreByIdAsync(int storeId);
        Task<IEnumerable<Store>> GetStoresByCityAsync(string city);
    }
}
