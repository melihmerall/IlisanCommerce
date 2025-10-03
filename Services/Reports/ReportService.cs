using IlisanCommerce.Data;
using IlisanCommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace IlisanCommerce.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderSummaryReport> GetOrderSummaryReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);

            // Base query
            var ordersQuery = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

            var orders = await ordersQuery.ToListAsync();

            // Calculate metrics
            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
            var completedOrders = orders.Count(o => o.Status == OrderStatus.Completed);
            var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);

            // Today's metrics
            var todayOrders = orders.Where(o => o.OrderDate.Date == today).ToList();
            var todayRevenue = todayOrders.Sum(o => o.TotalAmount);
            var todayOrderCount = todayOrders.Count;

            // This week's metrics
            var weekOrders = orders.Where(o => o.OrderDate.Date >= weekStart).ToList();
            var weekRevenue = weekOrders.Sum(o => o.TotalAmount);
            var weekOrderCount = weekOrders.Count;

            // This month's metrics
            var monthOrders = orders.Where(o => o.OrderDate.Date >= monthStart).ToList();
            var monthRevenue = monthOrders.Sum(o => o.TotalAmount);
            var monthOrderCount = monthOrders.Count;

            return new OrderSummaryReport
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                TodayRevenue = todayRevenue,
                TodayOrders = todayOrderCount,
                WeekRevenue = weekRevenue,
                WeekOrders = weekOrderCount,
                MonthRevenue = monthRevenue,
                MonthOrders = monthOrderCount
            };
        }

        public async Task<SalesReport> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

            var orders = await ordersQuery
                .Where(o => o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            var totalSales = orders.Sum(o => o.TotalAmount);
            var totalOrderCount = orders.Count;
            var averageOrderValue = totalOrderCount > 0 ? totalSales / totalOrderCount : 0;

            var dailySales = await GetDailySalesAsync(startDate, endDate);
            var topProducts = await GetTopProductsAsync(10, startDate, endDate);

            return new SalesReport
            {
                TotalSales = totalSales,
                TotalOrderCount = totalOrderCount,
                AverageOrderValue = averageOrderValue,
                DailySales = dailySales,
                TopProducts = topProducts
            };
        }

        public async Task<List<OrderStatusCount>> GetOrderStatusCountsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

            var statusCounts = await ordersQuery
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusCount
                {
                    Status = g.Key,
                    Count = g.Count(),
                    StatusText = GetStatusText(g.Key)
                })
                .ToListAsync();

            return statusCounts;
        }

        public async Task<List<DailySales>> GetDailySalesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var dailySales = await _context.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .Where(o => o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySales
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            return dailySales;
        }

        public async Task<List<TopProduct>> GetTopProductsAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);

            var topProducts = await ordersQuery
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopProduct
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(count)
                .ToListAsync();

            return topProducts ?? new List<TopProduct>();
        }

        public async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int months = 12)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var monthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate)
                .Where(o => o.Status != OrderStatus.Cancelled)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new MonthlyRevenue
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.GetMonthName(g.Key.Month),
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToListAsync();

            return monthlyRevenue;
        }

        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Bekliyor",
                OrderStatus.Confirmed => "Onaylandı",
                OrderStatus.Processing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoda",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Completed => "Tamamlandı",
                OrderStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmiyor"
            };
        }
    }
}
