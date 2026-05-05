using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{

    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly AppDbContext _context; 

        public InvoiceService(IOrderRepository orderRepo, IInvoiceRepository invoiceRepo, AppDbContext context)
        {
            _orderRepo = orderRepo;
            _invoiceRepo = invoiceRepo;
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<Invoice> CreateInvoiceForOrderAsync(int orderId)
        {
             var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

             if (order == null) throw new Exception("Order not found");

             // Check if exists
             var existing = await _context.Invoices.FirstOrDefaultAsync(i => i.OrderId == orderId);
             if (existing != null) return existing;

             // Logic to calculate Tax/Discount if not stored on Order
             decimal total = order.TotalAmount ?? 0;
             decimal subtotal = order.OrderItems.Sum(i => (decimal)(i.Subtotal ?? 0));
             decimal tax = total * 0.18m; // Simplified assumption or calculate properly
             // If we have strict fields in Order for these, use them. 
             // For now creating a basic Invoice record.
             
                // Helper to format address
                string addressStr = order.Address != null 
                    ? $"{order.Address.HouseNumber}, {order.Address.Landmark}, {order.Address.Town}, {order.Address.City}, {order.Address.State} - {order.Address.Pincode}"
                    : "";

                var invoice = new Invoice
                {
                    OrderId = orderId,
                    OrderDate = order.OrderDate ?? DateTime.Now,
                    TotalAmount = total,
                    TaxAmount = tax,
                    DiscountAmount = 0,
                    UserId = order.UserId,
                    EpointsUsed = order.EpointsUsed ?? 0,
                    EpointsEarned = order.EpointsEarned ?? 0,
                    DeliveryType = order.DeliveryType,
                    EpointsBalance = order.User?.Epoints ?? 0,
                    ShippingAddress = addressStr,
                    BillingAddress = addressStr // Assuming same for now
                };
             
             _context.Invoices.Add(invoice);
             await _context.SaveChangesAsync();
             return invoice;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int orderId)
        {
            // Eager load everything needed for PDF
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) throw new Exception("Order not found");

            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.OrderId == orderId);
            if (invoice == null) 
            {
                // Auto-create locally if missing (Optional)
                invoice = await CreateInvoiceForOrderAsync(orderId);
            }

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();

            void ComposeHeader(IContainer container)
            {
                var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
                
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("EMART INVOICE").Style(titleStyle);
                    });

                    row.RelativeItem().Background(Colors.Orange.Medium).Padding(10).Column(column =>
                    {
                        column.Item().AlignCenter().Text("TOTAL").FontSize(14).SemiBold();
                        column.Item().AlignCenter().Text($"Rs. {invoice.TotalAmount:F2}").FontSize(14).SemiBold();
                    });
                });
            }

            void ComposeContent(IContainer container)
            {
                container.PaddingVertical(20).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Sold By:").Bold();
                            col.Item().Text("EMART Pvt Ltd");
                            col.Item().Text("India");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Bill To:").Bold();
                            col.Item().Text($"{order.User?.FullName}");
                            col.Item().Text(order.User?.Email);
                            if (order.Address != null)
                            {
                                col.Item().Text($"{order.Address.HouseNumber}, {order.Address.Town}, {order.Address.City}, {order.Address.State} - {order.Address.Pincode}");
                            }
                        });
                    });

                    column.Item().PaddingVertical(10);
                    
                    // Meta Data
                    column.Item().Row(row => 
                    {
                        row.RelativeItem().Text(t => { t.Span("Invoice No: ").Bold(); t.Span($"{invoice.InvoiceId}"); });
                        row.RelativeItem().Text(t => { t.Span("Order ID: ").Bold(); t.Span($"{invoice.OrderId}"); });
                        row.RelativeItem().Text(t => { t.Span("Date: ").Bold(); t.Span($"{order.OrderDate:dd-MMM-yyyy}"); });
                    });
                    
                    column.Item().PaddingVertical(10);

                    // Table
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Product").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Qty").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Price").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Amount").SemiBold();
                        });

                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Product?.ProductName ?? "Item");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text($"{item.Quantity}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text($"Rs. {item.Price}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text($"Rs. {item.Subtotal}");
                        }
                    });

                    column.Item().PaddingVertical(10);

                    // Summary
                    column.Item().AlignRight().Table(table => 
                    {
                        table.ColumnsDefinition(columns => 
                        {
                            columns.ConstantColumn(100);
                            columns.ConstantColumn(100);
                        });
                        
                        // We don't have subtotal stored in Invoice, calculating or using Total
                        // Java had getDiscountAmount, getTaxAmount
                        
                        table.Cell().Padding(2).Text("Subtotal").Bold();
                        table.Cell().Padding(2).AlignRight().Text($"Rs. {order.OrderItems.Sum(x => x.Subtotal):F2}");
                        
                        table.Cell().Padding(2).Text("Discount").Bold();
                        table.Cell().Padding(2).AlignRight().Text($"- Rs. {invoice.DiscountAmount:F2}");
                        
                        table.Cell().Padding(2).Text("GST (18%)").Bold();
                        table.Cell().Padding(2).AlignRight().Text($"Rs. {(order.TotalAmount * 0.18m):F2}");
                        
                        table.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("TOTAL").Bold();
                        table.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"Rs. {invoice.TotalAmount:F2}").Bold();
                    });

                    column.Item().PaddingVertical(10);
                    
                    // E-Points
                     column.Item().Row(row => 
                    {
                        row.RelativeItem().Text(t => { t.Span("E-Points Used: ").Bold(); t.Span($"{invoice.EpointsUsed}"); });
                        // Balance might require Customer lookup but we have Earned on Invoice
                        row.RelativeItem().Text(t => { t.Span("E-Points Earned: ").Bold(); t.Span($"{invoice.EpointsEarned}"); });
                    });
                });
            }

            void ComposeFooter(IContainer container)
            {
                container.Column(column =>
                {
                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    column.Item().PaddingTop(5).Text("NOTES:").Bold();
                    column.Item().Text("1. Amount includes applicable taxes.");
                    column.Item().Text("2. This is a system generated invoice.");
                });
            }
        }
        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            return await _invoiceRepo.CreateInvoiceAsync(invoice);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepo.GetAllInvoicesAsync();
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            return await _invoiceRepo.DeleteInvoiceAsync(invoiceId);
        }

        public async Task<Invoice?> GetLatestInvoiceAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefaultAsync();
        }

        public async Task<byte[]> GenerateInvoicePdfByInvoiceIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null) throw new Exception("Invoice not found");

            // Reuse the existing PDF generation logic by orderId
            return await GenerateInvoicePdfAsync(invoice.OrderId ?? 0);
        }
    }
}

