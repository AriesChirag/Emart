using Emart_DotNet.Models;

namespace Emart_DotNet.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<byte[]> GenerateInvoicePdfAsync(int orderId);
        Task<Invoice> CreateInvoiceForOrderAsync(int orderId);
        Task<Invoice?> GetLatestInvoiceAsync();
        Task<byte[]> GenerateInvoicePdfByInvoiceIdAsync(int invoiceId);
    }
}

