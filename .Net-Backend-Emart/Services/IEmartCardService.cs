using Emart_DotNet.DTOs;
using Emart_DotNet.Models;

namespace Emart_DotNet.Services
{
    public interface IEmartCardService
    {
        Task<EmartCard> ApplyForCardAsync(ApplyEmartCardRequest request);
        
        Task UseEpointsAsync(int userId, int pointsUsed);
        
        Task<EmartCardDTO?> GetCardDetailsAsync(int userId);
    }
}
