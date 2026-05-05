using System.Collections.Generic;
using System.Threading.Tasks;
using Emart_DotNet.DTOs;

namespace Emart_DotNet.Services
{
    public interface IAdminDashboardService
    {
        Task<Dictionary<string, object>> GetDashboardStatsAsync();
        Task<PagedResult<OrderResponseDTO>> GetAllOrdersAsync(int page, int size);
        Task<OrderResponseDTO> UpdateOrderStatusAsync(int orderId, string status);
    }

    public class PagedResult<T>
    {
        public List<T> Content { get; set; } = new List<T>();
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
