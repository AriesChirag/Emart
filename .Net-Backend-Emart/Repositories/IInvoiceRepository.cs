using Emart_DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<bool> DeleteInvoiceAsync(int invoiceId);
    }
}
