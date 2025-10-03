using IlisanCommerce.Models;

namespace IlisanCommerce.Services.Pdf
{
    /// <summary>
    /// Professional PDF generation service interface
    /// Follows Interface Segregation Principle - separated by document types
    /// </summary>
    public interface IPdfService : IInvoicePdfService, IShippingLabelService, IReportPdfService
    {
        // Combined interface for dependency injection convenience
    }

    /// <summary>
    /// Invoice and order document generation
    /// </summary>
    public interface IInvoicePdfService
    {
        Task<byte[]> GenerateInvoicePdfAsync(Order order);
        Task<byte[]> GenerateOrderConfirmationPdfAsync(Order order);
        Task<byte[]> GenerateProformInvoicePdfAsync(Order order);
        Task<byte[]> GenerateReceiptPdfAsync(Order order);
        Task<string> SaveInvoicePdfAsync(Order order, string directoryPath = "invoices");
    }

    /// <summary>
    /// Shipping and logistics documents
    /// </summary>
    public interface IShippingLabelService
    {
        Task<byte[]> GenerateShippingLabelPdfAsync(Order order);
        Task<byte[]> GeneratePackingSlipPdfAsync(Order order);
        Task<byte[]> GenerateDeliveryNotePdfAsync(Order order);
        Task<byte[]> GenerateBulkShippingLabelsAsync(List<Order> orders);
        Task<string> SaveShippingLabelAsync(Order order, string directoryPath = "shipping");
    }

    /// <summary>
    /// Business reports and analytics
    /// </summary>
    public interface IReportPdfService
    {
        Task<byte[]> GenerateSalesReportPdfAsync(DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateInventoryReportPdfAsync();
        Task<byte[]> GenerateCustomerReportPdfAsync();
        Task<byte[]> GenerateOrderSummaryReportPdfAsync(DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateLogReportPdfAsync(string logType, DateTime fromDate, DateTime toDate);
        Task<string> SaveReportPdfAsync(string reportType, byte[] pdfData, string directoryPath = "reports");
    }

    /// <summary>
    /// PDF generation options and settings
    /// </summary>
    public class PdfGenerationOptions
    {
        public bool IncludeLogo { get; set; } = true;
        public bool IncludeWatermark { get; set; } = false;
        public string WatermarkText { get; set; } = "DRAFT";
        public PdfPageSize PageSize { get; set; } = PdfPageSize.A4;
        public bool IncludePageNumbers { get; set; } = true;
        public string? CustomFooterText { get; set; }
        public Dictionary<string, object> CustomData { get; set; } = new();
    }

    public enum PdfPageSize
    {
        A4,
        A3,
        Letter,
        Legal
    }

    /// <summary>
    /// PDF generation result with metadata
    /// </summary>
    public class PdfGenerationResult
    {
        public byte[] PdfData { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/pdf";
        public long FileSizeBytes { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string GeneratedBy { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
