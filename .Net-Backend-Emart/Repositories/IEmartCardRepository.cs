using Emart_DotNet.Models;

namespace Emart_DotNet.Repositories
{
    public interface IEmartCardRepository
    {
        Task<EmartCard?> FindByUserIdAsync(int userId);
        
        Task<bool> ExistsByUserIdAsync(int userId);
        
        Task SaveAsync(EmartCard card);
    }
}
