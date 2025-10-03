using IlisanCommerce.Models;
using IlisanCommerce.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace IlisanCommerce.Services.Pdf
{
    /// <summary>
    /// Professional PDF generation service using QuestPDF
    /// Follows SOLID principles with modular design
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PdfService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string _logoPath;
        private readonly CompanyInfo _companyInfo;

        public PdfService(ApplicationDbContext context, ILogger<PdfService> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _logoPath = Path.Combine(_environment.WebRootPath, "images", "logo", "ilisan-logo.png");
            
            // Initialize company information
            _companyInfo = new CompanyInfo
            {
                Name = "ILISAN Savunma Sanayi",
                Address = "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş",
                Phone = "+90 (850) 532 5237",
                Email = "info@ilisan.com.tr",
                Website = "https://ilisan.com.tr",
                TaxNumber = "1234567890"
            };

            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region Invoice PDF Generation

        public async Task<byte[]> GenerateInvoicePdfAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Generating invoice PDF for order {OrderId}", order.Id);

                // Load order with all necessary data
                var fullOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (fullOrder == null)
                {
                    throw new ArgumentException($"Order {order.Id} not found");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(c => ComposeInvoiceHeader(c, fullOrder));
                        page.Content().Element(c => ComposeInvoiceContent(c, fullOrder));
                        page.Footer().Element(c => ComposeInvoiceFooter(c, fullOrder));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                
                _logger.LogInformation("Invoice PDF generated successfully for order {OrderId}, size: {Size} bytes", 
                    order.Id, pdfBytes.Length);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<byte[]> GenerateOrderConfirmationPdfAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Generating order confirmation PDF for order {OrderId}", order.Id);

                var fullOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (fullOrder == null)
                {
                    throw new ArgumentException($"Order {order.Id} not found");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(c => ComposeOrderConfirmationHeader(c, fullOrder));
                        page.Content().Element(c => ComposeOrderConfirmationContent(c, fullOrder));
                        page.Footer().Element(c => ComposeOrderConfirmationFooter(c, fullOrder));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating order confirmation PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<byte[]> GenerateProformInvoicePdfAsync(Order order)
        {
            // Similar to invoice but marked as "PROFORM"
            var invoicePdf = await GenerateInvoicePdfAsync(order);
            // Add watermark "PROFORM" - implementation would modify the document
            return invoicePdf;
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(Order order)
        {
            // Simplified receipt format
            return await GenerateInvoicePdfAsync(order);
        }

        public async Task<string> SaveInvoicePdfAsync(Order order, string directoryPath = "invoices")
        {
            try
            {
                var pdfBytes = await GenerateInvoicePdfAsync(order);
                var fileName = $"invoice_{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var fullPath = Path.Combine(_environment.WebRootPath, "documents", directoryPath);
                
                Directory.CreateDirectory(fullPath);
                var filePath = Path.Combine(fullPath, fileName);
                
                await File.WriteAllBytesAsync(filePath, pdfBytes);
                
                _logger.LogInformation("Invoice PDF saved: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving invoice PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        #endregion

        #region Shipping Labels

        public async Task<byte[]> GenerateShippingLabelPdfAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Generating shipping label PDF for order {OrderId}", order.Id);

                var fullOrder = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (fullOrder == null)
                {
                    throw new ArgumentException($"Order {order.Id} not found");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                        page.Content().Element(c => ComposeShippingLabel(c, fullOrder));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating shipping label PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<byte[]> GeneratePackingSlipPdfAsync(Order order)
        {
            try
            {
                var fullOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (fullOrder == null)
                {
                    throw new ArgumentException($"Order {order.Id} not found");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(c => ComposePackingSlipHeader(c, fullOrder));
                        page.Content().Element(c => ComposePackingSlipContent(c, fullOrder));
                        page.Footer().Element(c => ComposePackingSlipFooter(c, fullOrder));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating packing slip PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<byte[]> GenerateDeliveryNotePdfAsync(Order order)
        {
            // Similar to packing slip but focused on delivery
            return await GeneratePackingSlipPdfAsync(order);
        }

        public async Task<byte[]> GenerateBulkShippingLabelsAsync(List<Order> orders)
        {
            try
            {
                _logger.LogInformation("Generating bulk shipping labels for {Count} orders", orders.Count);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Content().Element(c => ComposeBulkShippingLabels(c, orders));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bulk shipping labels");
                throw;
            }
        }

        public async Task<string> SaveShippingLabelAsync(Order order, string directoryPath = "shipping")
        {
            try
            {
                var pdfBytes = await GenerateShippingLabelPdfAsync(order);
                var fileName = $"shipping_label_{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var fullPath = Path.Combine(_environment.WebRootPath, "documents", directoryPath);
                
                Directory.CreateDirectory(fullPath);
                var filePath = Path.Combine(fullPath, fileName);
                
                await File.WriteAllBytesAsync(filePath, pdfBytes);
                
                _logger.LogInformation("Shipping label PDF saved: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving shipping label PDF for order {OrderId}", order.Id);
                throw;
            }
        }

        #endregion

        #region Reports

        public async Task<byte[]> GenerateSalesReportPdfAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Generating sales report PDF from {FromDate} to {ToDate}", fromDate, toDate);

                // Get sales data
                var orders = await _context.Orders
                    .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .ToListAsync();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(c => ComposeSalesReportHeader(c, fromDate, toDate));
                        page.Content().Element(c => ComposeSalesReportContent(c, orders));
                        page.Footer().Element(c => ComposeSalesReportFooter(c));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report PDF");
                throw;
            }
        }

        public async Task<byte[]> GenerateInventoryReportPdfAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive)
                    .ToListAsync();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(c => ComposeInventoryReportHeader(c));
                        page.Content().Element(c => ComposeInventoryReportContent(c, products));
                        page.Footer().Element(c => ComposeInventoryReportFooter(c));
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating inventory report PDF");
                throw;
            }
        }

        public async Task<byte[]> GenerateCustomerReportPdfAsync()
        {
            // Implementation for customer report
            throw new NotImplementedException("Customer report will be implemented in next phase");
        }

        public async Task<byte[]> GenerateOrderSummaryReportPdfAsync(DateTime fromDate, DateTime toDate)
        {
            // Implementation for order summary report
            return await GenerateSalesReportPdfAsync(fromDate, toDate);
        }

        public async Task<byte[]> GenerateLogReportPdfAsync(string logType, DateTime fromDate, DateTime toDate)
        {
            // Implementation for log report
            throw new NotImplementedException("Log report will be implemented in next phase");
        }

        public async Task<string> SaveReportPdfAsync(string reportType, byte[] pdfData, string directoryPath = "reports")
        {
            try
            {
                var fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var fullPath = Path.Combine(_environment.WebRootPath, "documents", directoryPath);
                
                Directory.CreateDirectory(fullPath);
                var filePath = Path.Combine(fullPath, fileName);
                
                await File.WriteAllBytesAsync(filePath, pdfData);
                
                _logger.LogInformation("Report PDF saved: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving report PDF");
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private void ComposeInvoiceHeader(IContainer container, Order order)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    // Company logo and info
                    if (File.Exists(_logoPath))
                    {
                        column.Item().Image(_logoPath).FitWidth();
                    }
                    
                    column.Item().PaddingTop(10).Text(_companyInfo.Name)
                        .FontSize(16).SemiBold();
                    column.Item().Text(_companyInfo.Address).FontSize(9);
                    column.Item().Text($"Tel: {_companyInfo.Phone}").FontSize(9);
                    column.Item().Text($"Email: {_companyInfo.Email}").FontSize(9);
                });

                row.RelativeItem().Column(column =>
                {
                    // Invoice details
                    column.Item().AlignRight().Text("FATURA")
                        .FontSize(20).SemiBold();
                    column.Item().AlignRight().Text($"Fatura No: {order.OrderNumber}").FontSize(10);
                    column.Item().AlignRight().Text($"Tarih: {order.OrderDate:dd.MM.yyyy}").FontSize(10);
                    column.Item().AlignRight().Text($"Durum: {GetOrderStatusText(order.Status)}").FontSize(10);
                });
            });
        }

        private void ComposeInvoiceContent(IContainer container, Order order)
        {
            container.PaddingVertical(20).Column(column =>
            {
                // Customer information
                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Text("MÜŞTERİ BİLGİLERİ")
                    .SemiBold();
                
                column.Item().Padding(10).Text($"{order.CustomerName}")
                    .SemiBold();
                column.Item().PaddingHorizontal(10).Text($"Email: {order.CustomerEmail}");
                column.Item().PaddingHorizontal(10).Text($"Telefon: {order.ShippingPhone ?? "N/A"}");
                
                if (order.ShippingAddressText != null)
                {
                    column.Item().PaddingHorizontal(10).Text($"Adres: {order.ShippingAddressText??""}, {order.ShippingCity??""}");
                }

                // Order items table
                column.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30); // #
                        columns.RelativeColumn(3); // Product
                        columns.ConstantColumn(60); // Quantity
                        columns.ConstantColumn(80); // Unit Price
                        columns.ConstantColumn(80); // Total
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("#");
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ürün");
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Adet").AlignCenter();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Birim Fiyat").AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Toplam").AlignRight();
                    });

                    // Items
                    int itemIndex = 1;
                    foreach (var item in order.OrderItems)
                    {
                        table.Cell().Padding(5).Text(itemIndex.ToString());
                        table.Cell().Padding(5).Text(item.Product?.Name ?? "Unknown Product");
                        table.Cell().Padding(5).Text(item.Quantity.ToString()).AlignCenter();
                        table.Cell().Padding(5).Text(item.UnitPrice.ToString("C2", new CultureInfo("tr-TR"))).AlignRight();
                        table.Cell().Padding(5).Text((item.Quantity * item.UnitPrice).ToString("C2", new CultureInfo("tr-TR"))).AlignRight();
                        itemIndex++;
                    }
                });

                // Totals
                column.Item().PaddingTop(20).AlignRight().Column(totalsColumn =>
                {
                    var subtotal = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
                    var shipping = order.ShippingCost;
                    var total = order.TotalAmount;

                    totalsColumn.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text("Ara Toplam:");
                        row.ConstantItem(80).Text(subtotal.ToString("C2", new CultureInfo("tr-TR"))).AlignRight();
                    });

                    if (shipping > 0)
                    {
                        totalsColumn.Item().Row(row =>
                        {
                            row.ConstantItem(100).Text("Kargo:");
                            row.ConstantItem(80).Text(shipping.ToString("C2", new CultureInfo("tr-TR"))).AlignRight();
                        });
                    }

                    totalsColumn.Item().BorderTop(1).PaddingTop(5).Row(row =>
                    {
                        row.ConstantItem(100).Text("TOPLAM:").SemiBold();
                        row.ConstantItem(80).Text(total.ToString("C2", new CultureInfo("tr-TR")))
                            .SemiBold().AlignRight();
                    });
                });
            });
        }

        private void ComposeInvoiceFooter(IContainer container, Order order)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Bu fatura ").FontSize(8);
                text.Span(_companyInfo.Name).FontSize(8).SemiBold();
                text.Span(" tarafından elektronik ortamda oluşturulmuştur.").FontSize(8);
            });
        }

        // Additional compose methods for other document types...
        private void ComposeOrderConfirmationHeader(IContainer container, Order order)
        {
            // Similar to invoice header but with "SİPARİŞ ONAY" title
            ComposeInvoiceHeader(container, order);
        }

        private void ComposeOrderConfirmationContent(IContainer container, Order order)
        {
            // Similar to invoice content but with confirmation-specific messaging
            ComposeInvoiceContent(container, order);
        }

        private void ComposeOrderConfirmationFooter(IContainer container, Order order)
        {
            container.AlignCenter().Text("Siparişiniz alınmıştır. Teşekkür ederiz!").FontSize(10);
        }

        private void ComposeShippingLabel(IContainer container, Order order)
        {
            container.Column(column =>
            {
                // Shipping label content
                column.Item().Text("KARGO ETİKETİ").FontSize(16).SemiBold().AlignCenter();
                column.Item().PaddingTop(10).Text($"Sipariş No: {order.OrderNumber}").FontSize(12);
                column.Item().Text($"Tarih: {order.OrderDate:dd.MM.yyyy}").FontSize(12);
                
                // Customer address
                column.Item().PaddingTop(20).Text("GÖNDERİLECEK ADRES:").SemiBold();
                column.Item().Text(order.CustomerName).FontSize(12);
                column.Item().Text(order.ShippingAddressText != null ? $"{order.ShippingAddressText??""}, {order.ShippingCity??""}" : "Adres belirtilmemiş").FontSize(12);
                column.Item().Text($"Tel: {order.ShippingAddressText?? "N/A"}").FontSize(12);

                // Barcode placeholder
                column.Item().PaddingTop(30).AlignCenter().Text($"||||| {order.OrderNumber} |||||")
                    .FontFamily("Courier New").FontSize(14);
            });
        }

        private void ComposePackingSlipHeader(IContainer container, Order order)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("PAKET LİSTESİ").FontSize(18).SemiBold();
                row.RelativeItem().AlignRight().Text($"Sipariş: {order.OrderNumber}").FontSize(12);
            });
        }

        private void ComposePackingSlipContent(IContainer container, Order order)
        {
            container.PaddingTop(20).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Product
                    columns.ConstantColumn(60); // Quantity
                    columns.ConstantColumn(100); // Status
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ürün");
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Adet").AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum").AlignCenter();
                });

                foreach (var item in order.OrderItems)
                {
                    table.Cell().Padding(5).Text(item.Product?.Name ?? "Unknown Product");
                    table.Cell().Padding(5).Text(item.Quantity.ToString()).AlignCenter();
                    table.Cell().Padding(5).Text("[ ]").AlignCenter(); // Checkbox for packing
                }
            });
        }

        private void ComposePackingSlipFooter(IContainer container, Order order)
        {
            container.PaddingTop(20).Text("Paketleme Notları: ____________________").FontSize(10);
        }

        private void ComposeBulkShippingLabels(IContainer container, List<Order> orders)
        {
            container.Column(column =>
            {
                int labelsPerPage = 4;
                for (int i = 0; i < orders.Count; i += labelsPerPage)
                {
                    var pageOrders = orders.Skip(i).Take(labelsPerPage).ToList();
                    
                    foreach (var order in pageOrders)
                    {
                        column.Item().BorderBottom(1).PaddingVertical(10).Element(c => ComposeShippingLabel(c, order));
                    }
                    
                    if (i + labelsPerPage < orders.Count)
                    {
                        column.Item().PageBreak();
                    }
                }
            });
        }

        private void ComposeSalesReportHeader(IContainer container, DateTime fromDate, DateTime toDate)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("SATIŞ RAPORU").FontSize(18).SemiBold();
                row.RelativeItem().AlignRight().Text($"{fromDate:dd.MM.yyyy} - {toDate:dd.MM.yyyy}").FontSize(12);
            });
        }

        private void ComposeSalesReportContent(IContainer container, List<Order> orders)
        {
            container.PaddingTop(20).Column(column =>
            {
                // Summary statistics
                var totalOrders = orders.Count;
                var totalRevenue = orders.Sum(o => o.TotalAmount);
                var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Toplam Sipariş: {totalOrders}");
                    row.RelativeItem().Text($"Toplam Gelir: {totalRevenue:C2}");
                    row.RelativeItem().Text($"Ortalama Sepet: {avgOrderValue:C2}");
                });

                // Orders table
                column.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(100); // Order Number
                        columns.ConstantColumn(80); // Date
                        columns.RelativeColumn(2); // Customer
                        columns.ConstantColumn(80); // Total
                        columns.ConstantColumn(80); // Status
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sipariş No");
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tarih");
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Müşteri");
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Toplam").AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Durum");
                    });

                    foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                    {
                        table.Cell().Padding(5).Text(order.OrderNumber);
                        table.Cell().Padding(5).Text(order.OrderDate.ToString("dd.MM.yyyy"));
                        table.Cell().Padding(5).Text(order.CustomerName);
                        table.Cell().Padding(5).Text(order.TotalAmount.ToString("C2")).AlignRight();
                        table.Cell().Padding(5).Text(GetOrderStatusText(order.Status));
                    }
                });
            });
        }

        private void ComposeSalesReportFooter(IContainer container)
        {
            container.AlignCenter().Text($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8);
        }

        private void ComposeInventoryReportHeader(IContainer container)
        {
            container.Text("STOK RAPORU").FontSize(18).SemiBold();
        }

        private void ComposeInventoryReportContent(IContainer container, List<Product> products)
        {
            container.PaddingTop(20).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80); // Code
                    columns.RelativeColumn(3); // Name
                    columns.RelativeColumn(2); // Category
                    columns.ConstantColumn(60); // Stock
                    columns.ConstantColumn(60); // Min Stock
                    columns.ConstantColumn(80); // Price
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kod");
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ürün Adı");
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kategori");
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Stok").AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Min Stok").AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Fiyat").AlignRight();
                });

                foreach (var product in products.OrderBy(p => p.Name))
                {
                    var isLowStock = product.StockQuantity <= product.MinStockLevel;
                    
                    table.Cell().Padding(5).Text(product.ProductCode);
                    table.Cell().Padding(5).Text(product.Name);
                    table.Cell().Padding(5).Text(product.Category?.Name ?? "");
                    table.Cell().Padding(5).Text(product.StockQuantity.ToString())
                        .AlignCenter().FontColor(isLowStock ? Colors.Red.Medium : Colors.Black);
                    table.Cell().Padding(5).Text(product.MinStockLevel.ToString()).AlignCenter();
                    table.Cell().Padding(5).Text(product.Price.ToString("C2")).AlignRight();
                }
            });
        }

        private void ComposeInventoryReportFooter(IContainer container)
        {
            container.AlignCenter().Text($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8);
        }

        private string GetOrderStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Beklemede",
                OrderStatus.Confirmed => "Onaylandı",
                OrderStatus.Processing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoya Verildi",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Cancelled => "İptal Edildi",
                OrderStatus.Returned => "İade Edildi",
                _ => status.ToString()
            };
        }

        #endregion
    }

    /// <summary>
    /// Company information for PDF documents
    /// </summary>
    public class CompanyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
    }
}
