using IlisanCommerce.Models;

namespace IlisanCommerce.Services.Reports
{
    public interface IReportService
    {
        Task<OrderSummaryReport> GetOrderSummaryReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<SalesReport> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<OrderStatusCount>> GetOrderStatusCountsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<DailySales>> GetDailySalesAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TopProduct>> GetTopProductsAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int months = 12);
    }

    public class OrderSummaryReport
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public decimal WeekRevenue { get; set; }
        public int WeekOrders { get; set; }
        public decimal MonthRevenue { get; set; }
        public int MonthOrders { get; set; }
    }

    public class SalesReport
    {
        public decimal TotalSales { get; set; }
        public int TotalOrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySales> DailySales { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
    }

    public class OrderStatusCount
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }

    public class DailySales
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}
