using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IlisanCommerce.Services.Reports;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Models.Constants;
using IlisanCommerce.Models;
using System.Security.Claims;

namespace IlisanCommerce.Controllers.Admin
{
    [Authorize]
    [Route("admin/reports")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILoggingService _loggingService;

        public ReportsController(IReportService reportService, ILoggingService loggingService)
        {
            _reportService = reportService;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get default report (last 30 days)
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now;

                var orderSummary = await _reportService.GetOrderSummaryReportAsync(startDate, endDate);
                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);
                var statusCounts = await _reportService.GetOrderStatusCountsAsync(startDate, endDate);
                var dailySales = await _reportService.GetDailySalesAsync(startDate, endDate);
                var topProducts = await _reportService.GetTopProductsAsync(10, startDate, endDate);
                var monthlyRevenue = await _reportService.GetMonthlyRevenueAsync(12);

                var model = new ReportsViewModel
                {
                    OrderSummary = orderSummary,
                    SalesReport = salesReport,
                    StatusCounts = statusCounts,
                    DailySales = dailySales,
                    TopProducts = topProducts,
                    MonthlyRevenue = monthlyRevenue,
                    StartDate = startDate,
                    EndDate = endDate
                };

                // Log admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read,
                    EntityTypes.System,
                    0,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: "Admin viewed reports dashboard"
                );

                ViewData["Title"] = "Raporlar ve Analitik";
                ViewData["Description"] = "Satış raporları ve istatistikler";

                return View("~/Views/Admin/Reports/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.Index",
                    "Raporlar yüklenirken hata oluştu",
                    ex,
                    new Dictionary<string, object>()
                );

                TempData["Error"] = "Raporlar yüklenirken bir hata oluştu.";
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterReports(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Default to last 30 days if no dates provided
                startDate ??= DateTime.Now.AddDays(-30);
                endDate ??= DateTime.Now;

                var orderSummary = await _reportService.GetOrderSummaryReportAsync(startDate, endDate);
                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);
                var statusCounts = await _reportService.GetOrderStatusCountsAsync(startDate, endDate);
                var dailySales = await _reportService.GetDailySalesAsync(startDate, endDate);
                var topProducts = await _reportService.GetTopProductsAsync(10, startDate, endDate);
                var monthlyRevenue = await _reportService.GetMonthlyRevenueAsync(12);

                var model = new ReportsViewModel
                {
                    OrderSummary = orderSummary,
                    SalesReport = salesReport,
                    StatusCounts = statusCounts,
                    DailySales = dailySales,
                    TopProducts = topProducts,
                    MonthlyRevenue = monthlyRevenue,
                    StartDate = startDate.Value,
                    EndDate = endDate.Value
                };

                // Log admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read,
                    EntityTypes.System,
                    0,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin filtered reports from {startDate:dd.MM.yyyy} to {endDate:dd.MM.yyyy}"
                );

                ViewData["Title"] = "Raporlar ve Analitik";
                ViewData["Description"] = "Satış raporları ve istatistikler";

                return View("~/Views/Admin/Reports/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.FilterReports",
                    "Rapor filtreleme sırasında hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["startDate"] = startDate?.ToString() ?? "", ["endDate"] = endDate?.ToString() ?? "" }
                );

                TempData["Error"] = "Raporlar filtrelenirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet("api/daily-sales")]
        public async Task<IActionResult> GetDailySalesData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var dailySales = await _reportService.GetDailySalesAsync(startDate, endDate);
                return Json(dailySales);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.GetDailySalesData",
                    "Günlük satış verileri alınırken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["startDate"] = startDate?.ToString() ?? "", ["endDate"] = endDate?.ToString() ?? "" }
                );

                return Json(new { error = "Veriler yüklenirken hata oluştu" });
            }
        }

        [HttpGet("api/order-status")]
        public async Task<IActionResult> GetOrderStatusData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var statusCounts = await _reportService.GetOrderStatusCountsAsync(startDate, endDate);
                return Json(statusCounts);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.GetOrderStatusData",
                    "Sipariş durumu verileri alınırken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["startDate"] = startDate?.ToString() ?? "", ["endDate"] = endDate?.ToString() ?? "" }
                );

                return Json(new { error = "Veriler yüklenirken hata oluştu" });
            }
        }

        [HttpGet("api/monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenueData(int months = 12)
        {
            try
            {
                var monthlyRevenue = await _reportService.GetMonthlyRevenueAsync(months);
                return Json(monthlyRevenue);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.GetMonthlyRevenueData",
                    "Aylık gelir verileri alınırken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["months"] = months }
                );

                return Json(new { error = "Veriler yüklenirken hata oluştu" });
            }
        }

        [HttpGet("GenerateSalesReportPdf")]
        [Route("admin/GenerateSalesReportPdf")]
        public async Task<IActionResult> GenerateSalesReportPdf(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Default to last 30 days if no dates provided
                startDate ??= DateTime.Now.AddDays(-30);
                endDate ??= DateTime.Now;

                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);
                var orderSummary = await _reportService.GetOrderSummaryReportAsync(startDate, endDate);

                // Create PDF content
                var pdfContent = GenerateSalesReportPdfContent(salesReport, orderSummary, startDate.Value, endDate.Value);
                var pdfBytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);

                // Log activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Export,
                    EntityTypes.System,
                    0,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Sales report PDF generated for {startDate:dd.MM.yyyy} to {endDate:dd.MM.yyyy}"
                );

                return File(pdfBytes, "application/pdf", $"satis_raporu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "ReportsController.GenerateSalesReportPdf",
                    "PDF rapor oluşturulurken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["startDate"] = startDate?.ToString() ?? "", ["endDate"] = endDate?.ToString() ?? "" }
                );

                TempData["Error"] = "PDF raporu oluşturulurken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        private string GenerateSalesReportPdfContent(SalesReport salesReport, OrderSummaryReport orderSummary, DateTime startDate, DateTime endDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Satış Raporu</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .summary {{ margin-bottom: 20px; }}
        .summary-item {{ display: inline-block; margin: 10px; padding: 15px; border: 1px solid #ddd; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ padding: 10px; text-align: left; border: 1px solid #ddd; }}
        th {{ background-color: #f5f5f5; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>ILISAN Satış Raporu</h1>
        <h3>{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}</h3>
    </div>
    
    <div class='summary'>
        <div class='summary-item'>
            <h4>Toplam Satış</h4>
            <p>{salesReport.TotalSales:C}</p>
        </div>
        <div class='summary-item'>
            <h4>Sipariş Sayısı</h4>
            <p>{orderSummary.TotalOrders}</p>
        </div>
        <div class='summary-item'>
            <h4>Ortalama Sipariş</h4>
            <p>{salesReport.AverageOrderValue:C}</p>
        </div>
    </div>
    
    <p>Rapor oluşturulma tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
</body>
</html>";
        }
    }

}
