using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Models.Constants;
using System.Security.Claims;

namespace IlisanCommerce.Controllers.Admin
{
    [Authorize]
    [Route("admin/customers")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ApplicationDbContext context,
            ILoggingService loggingService,
            ILogger<CustomersController> logger)
        {
            _context = context;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Search functionality
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => 
                        u.FirstName.Contains(search) ||
                        u.LastName.Contains(search) ||
                        u.Email.Contains(search) ||
                        (u.Phone != null && u.Phone.Contains(search)));
                }

                // Get total count for pagination
                var totalCustomers = await query.CountAsync();

                // Apply pagination
                var customers = await query
                    .OrderByDescending(u => u.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new CustomerViewModel
                    {
                        Id = u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.Phone ?? "",
                        RegisterDate = u.CreatedDate,
                        IsActive = u.IsActive,
                        TotalOrders = _context.Orders.Count(o => o.UserId == u.Id),
                        TotalSpent = _context.Orders
                            .Where(o => o.UserId == u.Id && o.Status != OrderStatus.Cancelled)
                            .Sum(o => (decimal?)o.TotalAmount) ?? 0m
                    })
                    .ToListAsync();

                // Create pagination info
                var paginationInfo = new PaginationInfo
                {
                    CurrentPage = page,
                    TotalItems = totalCustomers,
                    ItemsPerPage = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCustomers / pageSize)
                };

                var viewModel = new CustomersIndexViewModel
                {
                    Customers = customers,
                    Pagination = paginationInfo,
                    SearchTerm = search ?? ""
                };

                // Statistics for dashboard cards
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.ActiveCustomers = await _context.Users.CountAsync(u => u.IsActive);
                ViewBag.NewCustomersThisMonth = await _context.Users
                    .CountAsync(u => u.CreatedDate >= DateTime.Now.AddDays(-30));

                // Log activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read,
                    EntityTypes.User,
                    0,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin viewed customers list. Page: {page}, Search: {search ?? "none"}"
                );

                ViewData["Title"] = "Müşteri Yönetimi";
                ViewData["Description"] = "Kayıtlı müşterileri yönetin";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers list");
                await _loggingService.LogErrorAsync(
                    "CustomersController.Index",
                    "Müşteri listesi yüklenirken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["search"] = search ?? "", ["page"] = page }
                );

                TempData["Error"] = "Müşteri listesi yüklenirken bir hata oluştu.";
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (customer == null)
                {
                    TempData["Error"] = "Müşteri bulunamadı.";
                    return RedirectToAction("Index");
                }

                // Get customer orders
                var orders = await _context.Orders
                    .Where(o => o.UserId == id)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync();

                // Get customer addresses
                var addresses = await _context.Addresses
                    .Where(a => a.UserId == id)
                    .OrderByDescending(a => a.IsDefault)
                    .ToListAsync();

                var customerDetail = new CustomerDetailViewModel
                {
                    Customer = customer,
                    RecentOrders = orders,
                    Addresses = addresses,
                    TotalOrders = await _context.Orders.CountAsync(o => o.UserId == id),
                    TotalSpent = await _context.Orders
                        .Where(o => o.UserId == id && o.Status != OrderStatus.Cancelled)
                        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m,
                    AverageOrderValue = await _context.Orders
                        .Where(o => o.UserId == id && o.Status != OrderStatus.Cancelled)
                        .AverageAsync(o => (decimal?)o.TotalAmount) ?? 0
                };

                // Log activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read,
                    EntityTypes.User,
                    id,
                    User.Identity?.Name,
                    customer.Email,
                    description: $"Admin viewed customer details for {customer.FirstName} {customer.LastName}"
                );

                ViewData["Title"] = $"Müşteri Detayı - {customer.FirstName} {customer.LastName}";

                return View("~/Views/Admin/Customers/Details.cshtml", customerDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer details for ID {CustomerId}", id);
                await _loggingService.LogErrorAsync(
                    "CustomersController.Details",
                    "Müşteri detayları yüklenirken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["customerId"] = id }
                );

                TempData["Error"] = "Müşteri detayları yüklenirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var customer = await _context.Users.FindAsync(id);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Müşteri bulunamadı." });
                }

                customer.IsActive = !customer.IsActive;
                await _context.SaveChangesAsync();

                // Log activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Update,
                    EntityTypes.User,
                    id,
                    User.Identity?.Name,
                    customer.Email,
                    description: $"Customer status changed to {(customer.IsActive ? "Active" : "Inactive")}"
                );

                _logger.LogInformation("Customer status updated. ID: {CustomerId}, Status: {IsActive}", id, customer.IsActive);

                return Json(new { 
                    success = true, 
                    message = $"Müşteri durumu {(customer.IsActive ? "aktif" : "pasif")} olarak güncellendi.",
                    isActive = customer.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling customer status for ID {CustomerId}", id);
                await _loggingService.LogErrorAsync(
                    "CustomersController.ToggleStatus",
                    "Müşteri durumu değiştirilirken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["customerId"] = id }
                );

                return Json(new { success = false, message = "Durum değiştirilemedi." });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportCustomers()
        {
            try
            {
                var customers = await _context.Users
                    .OrderBy(u => u.FirstName)
                    .Select(u => new
                    {
                        Ad = u.FirstName,
                        Soyad = u.LastName,
                        Email = u.Email,
                        Telefon = u.Phone ?? "",
                        KayitTarihi = u.CreatedDate.ToString("dd.MM.yyyy"),
                        Durum = u.IsActive ? "Aktif" : "Pasif",
                        ToplamSiparis = _context.Orders.Count(o => o.UserId == u.Id),
                        ToplamHarcama = _context.Orders
                            .Where(o => o.UserId == u.Id && o.Status != OrderStatus.Cancelled)
                            .Sum(o => (decimal?)o.TotalAmount) ?? 0m
                    })
                    .ToListAsync();

                var csv = GenerateCsv(customers);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

                // Log activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Export,
                    EntityTypes.User,
                    0,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin exported {customers.Count} customers to CSV"
                );

                return File(bytes, "text/csv", $"musteriler_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customers");
                TempData["Error"] = "Müşteri listesi dışa aktarılamadı.";
                return RedirectToAction("Index");
            }
        }

        private string GenerateCsv<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetProperties();
            var csv = new System.Text.StringBuilder();

            // Header
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Data
            foreach (var item in data)
            {
                var values = properties.Select(p => 
                {
                    var value = p.GetValue(item)?.ToString() ?? "";
                    return value.Contains(",") ? $"\"{value}\"" : value;
                });
                csv.AppendLine(string.Join(",", values));
            }

            return csv.ToString();
        }
    }

}