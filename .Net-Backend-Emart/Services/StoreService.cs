using Emart_DotNet.Models;
using Emart_DotNet.Repositories;

namespace Emart_DotNet.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repository;

        public StoreService(IStoreRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _repository.GetAllStoresAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(int storeId)
        {
            return await _repository.GetStoreByIdAsync(storeId);
        }

        public async Task<IEnumerable<Store>> GetStoresByCityAsync(string city)
        {
            return await _repository.GetStoresByCityAsync(city);
        }
    }
}
