using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services;
using IlisanCommerce.Services.Pdf;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Models.Constants;
using System.Security.Claims;
using System.IO.Compression;
namespace IlisanCommerce.Controllers.Admin
{
    [Authorize]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IShippingService _shippingService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPdfService _pdfService;
        private readonly ILoggingService _loggingService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SettingsService _settingsService;
        public AdminController(ApplicationDbContext context, IShippingService shippingService,
            IFileUploadService fileUploadService, IPdfService pdfService,
            ILoggingService loggingService, IEmailService emailService, ILogger<AdminController> logger, IWebHostEnvironment webHostEnvironment,
            SettingsService settingsService)
        {
            _context = context;
            _shippingService = shippingService;
            _fileUploadService = fileUploadService;
            _pdfService = pdfService;
            _loggingService = loggingService;
            _emailService = emailService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _settingsService = settingsService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get current date ranges for comparison
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfLastMonth = startOfMonth.AddMonths(-1);
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                // Basic counts
                var totalProducts = await _context.Products.CountAsync();
                var totalOrders = await _context.Orders.CountAsync();
                var totalUsers = await _context.Users.CountAsync();
                var pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
                // Revenue calculations
                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                var monthlyRevenue = await _context.Orders
                    .Where(o => (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                               && o.OrderDate >= startOfMonth)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                var lastMonthRevenue = await _context.Orders
                    .Where(o => (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                               && o.OrderDate >= startOfLastMonth && o.OrderDate < startOfMonth)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                // Growth calculations
                var revenueGrowth = lastMonthRevenue > 0
                    ? Math.Round(((monthlyRevenue - lastMonthRevenue) / lastMonthRevenue) * 100, 1)
                    : 0;
                // Order statistics
                var thisMonthOrders = await _context.Orders.CountAsync(o => o.OrderDate >= startOfMonth);
                var lastMonthOrders = await _context.Orders
                    .CountAsync(o => o.OrderDate >= startOfLastMonth && o.OrderDate < startOfMonth);
                var orderGrowth = lastMonthOrders > 0
                    ? Math.Round(((double)(thisMonthOrders - lastMonthOrders) / lastMonthOrders) * 100, 1)
                    : 0;
                // Customer statistics
                var newCustomersThisMonth = await _context.Users.CountAsync(u => u.CreatedDate >= startOfMonth);
                var activeCustomersThisMonth = await _context.Users
                    .CountAsync(u => _context.Orders.Any(o => o.UserId == u.Id && o.OrderDate >= startOfMonth));
                // Stock alerts
                var lowStockProducts = await _context.Products.CountAsync(p => p.StockQuantity <= p.MinStockLevel);
                var criticalStockProducts = await _context.Products.CountAsync(p => p.StockQuantity <= 5);
                // Performance metrics
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
                var conversionRate = totalUsers > 0 ? Math.Round((double)totalOrders / totalUsers * 100, 1) : 0;
                // Recent performance data (last 7 days)
                var weeklyStats = new List<object>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    var dayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == date);
                    var dayRevenue = await _context.Orders
                        .Where(o => o.OrderDate.Date == date && (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered))
                        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                    weeklyStats.Add(new { Date = date.ToString("dd/MM"), Orders = dayOrders, Revenue = dayRevenue });
                }
                var stats = new
                {
                    TotalProducts = totalProducts,
                    TotalOrders = totalOrders,
                    TotalUsers = totalUsers,
                    PendingOrders = pendingOrders,
                    TotalRevenue = totalRevenue,
                    MonthlyRevenue = monthlyRevenue,
                    LowStockProducts = lowStockProducts,
                    CriticalStockProducts = criticalStockProducts,
                    AverageOrderValue = averageOrderValue,
                    ConversionRate = conversionRate,
                    // Growth metrics
                    RevenueGrowth = revenueGrowth,
                    OrderGrowth = orderGrowth,
                    // Monthly stats
                    NewCustomersThisMonth = newCustomersThisMonth,
                    OrdersThisMonth = thisMonthOrders,
                    ActiveCustomersThisMonth = activeCustomersThisMonth,
                    // Weekly performance
                    WeeklyStats = weeklyStats
                };
                ViewBag.Stats = stats;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "Panel yüklenirken hata oluştu.";
                // Fallback empty stats
                ViewBag.Stats = new
                {
                    TotalProducts = 0,
                    TotalOrders = 0,
                    TotalUsers = 0,
                    PendingOrders = 0,
                    TotalRevenue = 0m,
                    MonthlyRevenue = 0m,
                    LowStockProducts = 0,
                    CriticalStockProducts = 0,
                    AverageOrderValue = 0m,
                    ConversionRate = 0.0,
                    RevenueGrowth = 0.0,
                    OrderGrowth = 0.0,
                    NewCustomersThisMonth = 0,
                    OrdersThisMonth = 0,
                    ActiveCustomersThisMonth = 0,
                    WeeklyStats = new List<object>()
                };
                return View();
            }
        }
        [HttpGet("products")]
        public async Task<IActionResult> Products(int page = 1, int pageSize = 20, string search = "",
            int? categoryId = null, bool? inStock = null, string sortBy = "CreatedDate", string sortOrder = "desc")
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .AsQueryable();
                // Advanced search - Gelişmiş arama
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Name.Contains(search) ||
                                           p.ProductCode.Contains(search) ||
                                           p.Description.Contains(search) ||
                                           p.Category.Name.Contains(search));
                }
                // Category filter - Kategori filtresi
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }
                // Stock filter - Stok filtresi
                if (inStock.HasValue)
                {
                    if (inStock.Value)
                    {
                        query = query.Where(p => p.StockQuantity > 0);
                    }
                    else
                    {
                        query = query.Where(p => p.StockQuantity == 0);
                    }
                }
                // Sorting - Sıralama
                query = sortBy.ToLower() switch
                {
                    "name" => sortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "price" => sortOrder == "asc" ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "stock" => sortOrder == "asc" ? query.OrderBy(p => p.StockQuantity) : query.OrderByDescending(p => p.StockQuantity),
                    "category" => sortOrder == "asc" ? query.OrderBy(p => p.Category.Name) : query.OrderByDescending(p => p.Category.Name),
                    _ => sortOrder == "asc" ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate)
                };
                var totalCount = await query.CountAsync();
                var products = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                // Get statistics for dashboard cards
                var totalProducts = await _context.Products.CountAsync();
                var lowStockProducts = await _context.Products.CountAsync(p => p.StockQuantity <= p.MinStockLevel);
                var outOfStockProducts = await _context.Products.CountAsync(p => p.StockQuantity == 0);
                var activeProducts = await _context.Products.CountAsync(p => p.IsActive);
                // Get categories for filter dropdown
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new { c.Id, c.Name })
                    .ToListAsync();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                ViewBag.TotalCount = totalCount;
                ViewBag.Search = search;
                ViewBag.PageSize = pageSize;
                ViewBag.CategoryId = categoryId;
                ViewBag.InStock = inStock;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.Categories = categories;
                // Statistics
                ViewBag.TotalProducts = totalProducts;
                ViewBag.LowStockProducts = lowStockProducts;
                ViewBag.OutOfStockProducts = outOfStockProducts;
                ViewBag.ActiveProducts = activeProducts;
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["Error"] = "Ürünler yüklenirken hata oluştu.";
                return View(new List<Product>());
            }
        }
        [HttpGet("products/create")]
        public async Task<IActionResult> CreateProduct()
        {
            await LoadCategoriesForViewBag();
            LoadCertificateTypesForViewBag();
            var model = new Product
            {
                ProductVariants = new List<ProductVariant>(),
                IsActive = true,
                IsFeatured = false,
                IsSpecialProduct = false,
                StockQuantity = 0,
                MinStockLevel = 10
            };
            return View(model);
        }
        [HttpPost("products/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, List<IFormFile>? productImages, int mainImageIndex = 0)
        {
            var submittedVariants = product.ProductVariants?.ToList() ?? new List<ProductVariant>();
            product.ProductVariants = new List<ProductVariant>();
            try
            {
                // Fix checkbox validation issues - ensure boolean values are properly handled
                ModelState.Remove("Category");
                if (product.CategoryId <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Kategori seçimi zorunludur");
                }
                if (ModelState.IsValid)
                {
                    product.CreatedDate = DateTime.Now;
                    // Slug işleme - eğer kullanıcı slug girmediyse otomatik oluştur
                    if (string.IsNullOrWhiteSpace(product.Slug))
                    {
                        product.Slug = GenerateSlug(product.Name);
                    }
                    else
                    {
                        // Kullanıcının girdiği slug'ı temizle
                        product.Slug = GenerateSlug(product.Slug);
                    }
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    await SyncVariantsAfterCreateAsync(product, submittedVariants);
                    // Professional multi-image upload handling
                    if (productImages != null && productImages.Any())
                    {
                        var validImages = productImages.Where(f => f != null && _fileUploadService.IsValidImageFile(f)).ToList();
                        if (!validImages.Any())
                        {
                            TempData["Warning"] = "Ürün oluşturuldu ancak hiç geçerli resim bulunamadı.";
                            return RedirectToAction("EditProduct", new { id = product.Id });
                        }
                        for (int i = 0; i < validImages.Count; i++)
                        {
                            var file = validImages[i];
                            try
                            {
                                var imagePath = await _fileUploadService.UploadImageAsync(file, "products", 800, 600);
                                var productImage = new ProductImage
                                {
                                    ProductId = product.Id,
                                    ImagePath = imagePath,
                                    AltText = $"{product.Name} - Resim {i + 1}",
                                    IsMainImage = (i == mainImageIndex && mainImageIndex < validImages.Count),
                                    SortOrder = i
                                };
                                _context.ProductImages.Add(productImage);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to upload image {Index} for product {ProductId}", i, product.Id);
                            }
                        }
                        // Ensure at least one image is marked as main
                        if (!await _context.ProductImages.AnyAsync(pi => pi.ProductId == product.Id && pi.IsMainImage))
                        {
                            var firstImage = await _context.ProductImages
                                .Where(pi => pi.ProductId == product.Id)
                                .OrderBy(pi => pi.SortOrder)
                                .FirstOrDefaultAsync();
                            if (firstImage != null)
                            {
                                firstImage.IsMainImage = true;
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ürün başarıyla oluşturuldu.";
                    return RedirectToAction("Products");
                }
                await LoadCategoriesForViewBag();
                LoadCertificateTypesForViewBag();
                product.ProductVariants = submittedVariants;
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                TempData["Error"] = "Ürün oluşturulurken hata oluştu.";
                await LoadCategoriesForViewBag();
                LoadCertificateTypesForViewBag();
                product.ProductVariants = submittedVariants;
                return View(product);
            }
        }
        [HttpGet("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSpecifications)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                TempData["Error"] = "Ürün bulunamadı.";
                return RedirectToAction("Products");
            }
            await LoadCategoriesForViewBag();
            LoadCertificateTypesForViewBag();
            return View(product);
        }
        [HttpPost("products/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, List<IFormFile>? newImages, int mainImageIndex = 0)
        {
            var submittedVariants = product.ProductVariants?.ToList() ?? new List<ProductVariant>();
            product.ProductVariants = new List<ProductVariant>();
            try
            {
                if (id != product.Id)
                {
                    return NotFound();
                }
                // Fix checkbox validation issues - ensure boolean values are properly handled
                ModelState.Remove("Category");
                if (product.CategoryId <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Kategori seçimi zorunludur");
                }
                if (ModelState.IsValid)
                {
                    product.UpdatedDate = DateTime.Now;
                    // Slug işleme - eğer kullanıcı slug girmediyse otomatik oluştur
                    if (string.IsNullOrWhiteSpace(product.Slug))
                    {
                        product.Slug = GenerateSlug(product.Name);
                    }
                    else
                    {
                        // Kullanıcının girdiği slug'ı temizle
                        product.Slug = GenerateSlug(product.Slug);
                    }
                    // Ürünü güncelle
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    await SyncVariantsAfterEditAsync(product.Id, product.ProductCode, product.MinStockLevel, submittedVariants);
                    // Professional multi-image upload handling for edit
                    if (newImages != null && newImages.Any())
                    {
                        var validImages = newImages.Where(f => f != null && _fileUploadService.IsValidImageFile(f)).ToList();
                        if (validImages.Any())
                        {
                            var currentImageCount = await _context.ProductImages
                                .Where(pi => pi.ProductId == product.Id)
                                .CountAsync();
                            for (int i = 0; i < validImages.Count; i++)
                            {
                                var file = validImages[i];
                                try
                                {
                                    var imagePath = await _fileUploadService.UploadImageAsync(file, "products", 800, 600);
                                    var productImage = new ProductImage
                                    {
                                        ProductId = product.Id,
                                        ImagePath = imagePath,
                                        AltText = $"{product.Name} - Resim {currentImageCount + i + 1}",
                                        IsMainImage = false, // Will be set below if needed
                                        SortOrder = currentImageCount + i
                                    };
                                    _context.ProductImages.Add(productImage);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Failed to upload image {Index} for product {ProductId}", i, product.Id);
                                }
                            }
                            await _context.SaveChangesAsync();
                            // Set main image if specified
                            if (mainImageIndex >= 0)
                            {
                                var allImages = await _context.ProductImages
                                    .Where(pi => pi.ProductId == product.Id)
                                    .OrderBy(pi => pi.SortOrder)
                                    .ToListAsync();
                                if (mainImageIndex < allImages.Count)
                                {
                                    // Clear existing main images
                                    foreach (var img in allImages)
                                    {
                                        img.IsMainImage = false;
                                    }
                                    // Set new main image
                                    allImages[mainImageIndex].IsMainImage = true;
                                    await _context.SaveChangesAsync();
                                }
                            }
                            TempData["Success"] = $"Ürün güncellendi. {validImages.Count} yeni resim eklendi.";
                        }
                        else
                        {
                            TempData["Warning"] = "Ürün güncellendi ancak hiç geçerli resim bulunamadı.";
                        }
                    }
                    else
                    {
                        TempData["Success"] = "Ürün başarıyla güncellendi.";
                    }
                    return RedirectToAction("Products");
                }
                await LoadCategoriesForViewBag();
                LoadCertificateTypesForViewBag();
                product.ProductVariants = submittedVariants;
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                TempData["Error"] = "Ürün güncellenirken hata oluştu.";
                await LoadCategoriesForViewBag();
                LoadCertificateTypesForViewBag();
                product.ProductVariants = submittedVariants;
                return View(product);
            }
        }
        [HttpGet("shipping-rates")]
        
        [HttpPost("products/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct([FromForm] int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return Json(new { success = false, message = "Ürün bulunamadı." });

                // Önce ilişkili CartItems kayıtlarını sil
                var relatedCartItems = _context.CartItems.Where(ci => ci.ProductId == productId);
                _context.CartItems.RemoveRange(relatedCartItems);

                // Sonra ürünü sil
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ürün ve ilişkili kayıtlar başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product and related data");
                return Json(new { success = false, message = "Silme işlemi sırasında hata oluştu." });
            }
        }
        public async Task<IActionResult> ShippingRates()
        {
            var shippingRates = await _shippingService.GetActiveShippingRatesAsync();
            return View(shippingRates);
        }
        [HttpGet("shipping-rates/create")]
        public IActionResult CreateShippingRate()
        {
            return View();
        }
        [HttpPost("shipping-rates/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShippingRate(ShippingRate shippingRate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    shippingRate.CreatedDate = DateTime.Now;
                    _context.ShippingRates.Add(shippingRate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kargo fiyatlandırması başarıyla oluşturuldu.";
                    return RedirectToAction("ShippingRates");
                }
                return View(shippingRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shipping rate");
                TempData["Error"] = "Kargo fiyatlandırması oluşturulurken hata oluştu.";
                return View(shippingRate);
            }
        }
        [HttpGet("shipping-rates/edit/{id}")]
        public async Task<IActionResult> EditShippingRate(int id)
        {
            var shippingRate = await _context.ShippingRates.FindAsync(id);
            if (shippingRate == null)
            {
                TempData["Error"] = "Kargo fiyatlandırması bulunamadı.";
                return RedirectToAction("ShippingRates");
            }
            return View(shippingRate);
        }
        [HttpPost("shipping-rates/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShippingRate(int id, ShippingRate shippingRate)
        {
            try
            {
                if (id != shippingRate.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    shippingRate.UpdatedDate = DateTime.Now;
                    _context.Update(shippingRate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kargo fiyatlandırması başarıyla güncellendi.";
                    return RedirectToAction("ShippingRates");
                }
                return View(shippingRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shipping rate {ShippingRateId}", id);
                TempData["Error"] = "Kargo fiyatlandırması güncellenirken hata oluştu.";
                return View(shippingRate);
            }
        }
        [HttpPost("shipping-rates/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShippingRate(int id)
        {
            try
            {
                var shippingRate = await _context.ShippingRates.FindAsync(id);
                if (shippingRate != null)
                {
                    _context.ShippingRates.Remove(shippingRate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kargo fiyatlandırması başarıyla silindi.";
                }
                else
                {
                    TempData["Error"] = "Kargo fiyatlandırması bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shipping rate {ShippingRateId}", id);
                TempData["Error"] = "Kargo fiyatlandırması silinirken hata oluştu.";
            }
            return RedirectToAction("ShippingRates");
        }
        [HttpGet("orders")]
        public async Task<IActionResult> Orders(int page = 1, int pageSize = 20, OrderStatus? status = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .AsQueryable();
                if (status.HasValue)
                {
                    query = query.Where(o => o.Status == status.Value);
                }
                var totalCount = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                ViewBag.Status = status;
                ViewBag.PageSize = pageSize;
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders: {Message}", ex.Message);
                TempData["Error"] = $"Siparişler yüklenirken hata oluştu: {ex.Message}";
                return View(new List<Order>());
            }
        }
        // Slider Management
        [HttpGet("sliders")]
        public async Task<IActionResult> Sliders()
        {
            try
            {
                var sliders = await _context.Sliders
                    .OrderBy(s => s.SortOrder)
                    .ThenByDescending(s => s.CreatedDate)
                    .ToListAsync();
                return View(sliders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sliders");
                TempData["Error"] = "Slider'lar yüklenirken hata oluştu.";
                return View(new List<Slider>());
            }
        }
        [HttpGet("sliders/create")]
        public IActionResult CreateSlider()
        {
            return View();
        }
        [HttpPost("sliders/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSlider(Slider slider, IFormFile? imageFile)
        {
            try
            {
                // Remove ImageUrl from model validation since it will be provided via file upload
                ModelState.Remove(nameof(Slider.ImageUrl));
                // Image is required for new sliders
                if (imageFile == null || !_fileUploadService.IsValidImageFile(imageFile))
                {
                    ModelState.AddModelError("ImageFile", "Slider için resim gereklidir. Lütfen geçerli bir resim dosyası yükleyin.");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Resim yükle
                        var imagePath = await _fileUploadService.UploadImageAsync(imageFile!, "sliders", 1920, 800);
                        slider.ImageUrl = imagePath;
                        slider.CreatedDate = DateTime.Now;
                        _context.Sliders.Add(slider);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("New slider created successfully. ID: {SliderId}, Title: {Title}, ImagePath: {ImagePath}",
                            slider.Id, slider.Title, imagePath);
                        TempData["Success"] = "Slider başarıyla oluşturuldu.";
                        return RedirectToAction("Sliders");
                    }
                    catch (Exception uploadEx)
                    {
                        _logger.LogError(uploadEx, "Error uploading slider image");
                        ModelState.AddModelError("ImageFile", "Resim yüklenirken hata oluştu: " + uploadEx.Message);
                    }
                }
                return View(slider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating slider");
                TempData["Error"] = "Slider oluşturulurken hata oluştu: " + ex.Message;
                return View(slider);
            }
        }
        [HttpGet("sliders/edit/{id}")]
        public async Task<IActionResult> EditSlider(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                TempData["Error"] = "Slider bulunamadı.";
                return RedirectToAction("Sliders");
            }
            return View(slider);
        }
        [HttpPost("sliders/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSlider(int id, Slider slider, IFormFile? imageFile)
        {
            try
            {
                if (id != slider.Id)
                {
                    return NotFound();
                }
                // Remove ImageUrl from model validation since it might be provided via file upload
                ModelState.Remove(nameof(Slider.ImageUrl));
                var existingSlider = await _context.Sliders.FindAsync(id);
                if (existingSlider == null)
                {
                    TempData["Error"] = "Slider bulunamadı.";
                    return RedirectToAction("Sliders");
                }
                // Check if we need an image (either existing or new upload)
                var hasExistingImage = !string.IsNullOrEmpty(existingSlider.ImageUrl);
                var hasNewImage = imageFile != null && _fileUploadService.IsValidImageFile(imageFile);
                if (!hasExistingImage && !hasNewImage)
                {
                    ModelState.AddModelError("ImageFile", "Slider için resim gereklidir. Lütfen bir resim yükleyin.");
                }
                else if (imageFile != null && !_fileUploadService.IsValidImageFile(imageFile))
                {
                    ModelState.AddModelError("ImageFile", "Geçersiz dosya formatı. Sadece resim dosyaları kabul edilir.");
                }
                if (ModelState.IsValid)
                {
                    // Yeni resim yüklendiyse
                    if (hasNewImage)
                    {
                        try
                        {
                            // Yeni resmi yükle
                            var imagePath = await _fileUploadService.UploadImageAsync(imageFile!, "sliders", 1920, 800);
                            // Eski resmi sil (sadece başarılı yükleme sonrası)
                            if (hasExistingImage)
                            {
                                await _fileUploadService.DeleteImageAsync(existingSlider.ImageUrl);
                            }
                            existingSlider.ImageUrl = imagePath;
                            _logger.LogInformation("Slider image updated for ID {SliderId}. New path: {ImagePath}", id, imagePath);
                        }
                        catch (Exception uploadEx)
                        {
                            _logger.LogError(uploadEx, "Error uploading slider image for ID {SliderId}", id);
                            ModelState.AddModelError("ImageFile", "Resim yüklenirken hata oluştu: " + uploadEx.Message);
                            return View(slider);
                        }
                    }
                    // Diğer alanları güncelle
                    existingSlider.Title = slider.Title;
                    existingSlider.Subtitle = slider.Subtitle;
                    existingSlider.ButtonText = slider.ButtonText;
                    existingSlider.ButtonUrl = slider.ButtonUrl;
                    existingSlider.Description = slider.Description;
                    existingSlider.IsActive = slider.IsActive;
                    existingSlider.SortOrder = slider.SortOrder;
                    existingSlider.UpdatedDate = DateTime.Now;
                    _context.Update(existingSlider);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Slider updated successfully. ID: {SliderId}, Title: {Title}", id, slider.Title);
                    TempData["Success"] = "Slider başarıyla güncellendi.";
                    return RedirectToAction("Sliders");
                }
                // Model doğrulama hatası varsa, mevcut slider verilerini view'e gönder
                slider.ImageUrl = existingSlider.ImageUrl; // Mevcut resmi koru
                return View(slider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating slider {SliderId}", id);
                TempData["Error"] = "Slider güncellenirken hata oluştu: " + ex.Message;
                return View(slider);
            }
        }
        [HttpPost("sliders/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            try
            {
                var slider = await _context.Sliders.FindAsync(id);
                if (slider != null)
                {
                    // Resmi sil
                    if (!string.IsNullOrEmpty(slider.ImageUrl))
                    {
                        await _fileUploadService.DeleteImageAsync(slider.ImageUrl);
                    }
                    _context.Sliders.Remove(slider);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Slider başarıyla silindi.";
                }
                else
                {
                    TempData["Error"] = "Slider bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting slider {SliderId}", id);
                TempData["Error"] = "Slider silinirken hata oluştu.";
            }
            return RedirectToAction("Sliders");
        }
        // Product Image Management
        [HttpPost("products/images/{imageId}/set-main")]
        public async Task<IActionResult> SetMainProductImage(int imageId)
        {
            try
            {
                var productImage = await _context.ProductImages
                    .FirstOrDefaultAsync(pi => pi.Id == imageId);
                if (productImage == null)
                {
                    return Json(new { success = false, message = "Resim bulunamadı." });
                }
                // Diğer ana resimleri kaldır
                var existingMainImages = await _context.ProductImages
                    .Where(pi => pi.ProductId == productImage.ProductId && pi.IsMainImage)
                    .ToListAsync();
                foreach (var img in existingMainImages)
                {
                    img.IsMainImage = false;
                }
                // Bu resmi ana resim yap
                productImage.IsMainImage = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Ana resim başarıyla ayarlandı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting main image {ImageId}", imageId);
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }
        [HttpPost("products/images/{imageId}/delete")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            try
            {
                var productImage = await _context.ProductImages
                    .FirstOrDefaultAsync(pi => pi.Id == imageId);
                if (productImage == null)
                {
                    return Json(new { success = false, message = "Resim bulunamadı." });
                }
                // Dosyayı fiziksel olarak sil
                try
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, productImage.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not delete physical file for image {ImageId}", imageId);
                }
                // Veritabanından sil
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Resim başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }
        [HttpPost("products/{productId}/images/upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProductImage(int productId, IFormFile imageFile, bool isMainImage = false)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }
                if (imageFile == null || !_fileUploadService.IsValidImageFile(imageFile))
                {
                    return Json(new { success = false, message = "Geçersiz dosya formatı." });
                }
                var imagePath = await _fileUploadService.UploadImageAsync(imageFile, "products", 800, 600);
                // Eğer ana resim olarak işaretleniyorsa, diğer ana resimleri false yap
                if (isMainImage)
                {
                    var existingMainImages = await _context.ProductImages
                        .Where(pi => pi.ProductId == productId && pi.IsMainImage)
                        .ToListAsync();
                    foreach (var img in existingMainImages)
                    {
                        img.IsMainImage = false;
                    }
                }
                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImagePath = imagePath,
                    AltText = product.Name,
                    IsMainImage = isMainImage,
                    SortOrder = await _context.ProductImages.CountAsync(pi => pi.ProductId == productId)
                };
                _context.ProductImages.Add(productImage);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Resim başarıyla yüklendi.",
                    imageId = productImage.Id,
                    imagePath = imagePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading product image");
                return Json(new { success = false, message = "Resim yüklenirken hata oluştu." });
            }
        }
        private async Task LoadCategoriesForViewBag()
        {
            // Provide the full category entities so views can build a hierarchical select reliably
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.ParentCategoryId.HasValue ? 1 : 0)
                .ThenBy(c => c.ParentCategoryId)
                .ThenBy(c => c.Name)
                .ToListAsync();
            ViewBag.Categories = categories;
        }
        private void LoadCertificateTypesForViewBag()
        {
            // Simple certificate types - can be made dynamic later via settings
            ViewBag.CertificateTypes = new List<string>
            {
                "NATO STANAG 2920",
                "TSE K 1055",
                "CE Marking",
                "ISO 9001",
                "ISO 14001",
                "STANAG 4569",
                "NIJ 0101.06",
                "EN 13432",
                "MIL-STD-810G",
                "AQAP-2110",
                "Diğer"
            };
        }
        private string GenerateSlug(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            // Türkçe karakterleri değiştir
            var slug = name.ToLowerInvariant()
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u");
            // Özel karakterleri temizle
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = slug.Trim('-');
            return slug;
        }
        #region PDF Export Methods
        /// <summary>
        /// Generate and download invoice PDF for specific order
        /// </summary>
        [HttpGet("orders/{id}/invoice")]
        public async Task<IActionResult> GenerateInvoicePdf(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    TempData["Error"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Orders");
                }
                var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(order);
                var fileName = $"fatura_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf";
                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Export,
                    EntityTypes.Order,
                    order.Id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin generated invoice PDF for order {order.OrderNumber}"
                );
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice PDF for order {OrderId}", id);
                await _loggingService.LogErrorAsync(ex, "AdminController.GenerateInvoicePdf");
                TempData["Error"] = "Fatura PDF oluşturulurken hata oluştu.";
                return RedirectToAction("Orders");
            }
        }
        /// <summary>
        /// Generate and download shipping label PDF for specific order
        /// </summary>
        [HttpGet("orders/{id}/shipping-label")]
        public async Task<IActionResult> GenerateShippingLabelPdf(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    TempData["Error"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Orders");
                }
                var pdfBytes = await _pdfService.GenerateShippingLabelPdfAsync(order);
                var fileName = $"kargo_etiketi_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf";
                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Export,
                    EntityTypes.Order,
                    order.Id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin generated shipping label PDF for order {order.OrderNumber}"
                );
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating shipping label PDF for order {OrderId}", id);
                await _loggingService.LogErrorAsync(ex, "AdminController.GenerateShippingLabelPdf");
                TempData["Error"] = "Kargo etiketi PDF oluşturulurken hata oluştu.";
                return RedirectToAction("Orders");
            }
        }
        /// <summary>
        /// Order detail page with PDF export options
        /// </summary>
        [HttpGet("orders/{id}/details")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(o => o.User)
                    // OrderStatusHistories removed - using simple status tracking
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    TempData["Error"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Orders");
                }
                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read,
                    EntityTypes.Order,
                    order.Id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Admin viewed order details for {order.OrderNumber}"
                );
                ViewData["Title"] = $"Sipariş Detayı - {order.OrderNumber}";
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details for order {OrderId}", id);
                await _loggingService.LogErrorAsync(ex, "AdminController.OrderDetails");
                TempData["Error"] = "Sipariş detayları yüklenirken hata oluştu.";
                return RedirectToAction("Orders");
            }
        }
        /// <summary>
        /// Bulk order status update
        /// </summary>
        [HttpPost("orders/bulk-status-update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkStatusUpdate([FromForm] int[] orderIds, [FromForm] OrderStatus newStatus, [FromForm] string? comment = null)
        {
            try
            {
                if (orderIds == null || orderIds.Length == 0)
                {
                    return Json(new { success = false, message = "Lütfen en az bir sipariş seçin." });
                }
                var orders = await _context.Orders
                    .Where(o => orderIds.Contains(o.Id))
                    .ToListAsync();
                if (!orders.Any())
                {
                    return Json(new { success = false, message = "Seçili siparişler bulunamadı." });
                }
                var updatedCount = 0;
                foreach (var order in orders)
                {
                    var oldStatus = order.Status;
                    order.Status = newStatus;
                    // UpdatedDate property removed - using Order.OrderDate instead
                    // Status history tracking simplified - log the change instead
                    updatedCount++;
                }
                await _context.SaveChangesAsync();
                // Log bulk activity
                await _loggingService.LogBulkOperationAsync(
                    "BulkStatusUpdate",
                    EntityTypes.Order,
                    orderIds,
                    User.Identity?.Name,
                    User.Identity?.Name
                );
                return Json(new
                {
                    success = true,
                    message = $"{updatedCount} sipariş durumu '{GetOrderStatusText(newStatus)}' olarak güncellendi."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk status update");
                await _loggingService.LogErrorAsync(ex, "AdminController.BulkStatusUpdate");
                return Json(new { success = false, message = "Toplu güncelleme sırasında hata oluştu." });
            }
        }
        /// <summary>
        /// Bulk order deletion
        /// </summary>
        [HttpPost("orders/bulk-delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteOrders([FromForm] int[] orderIds)
        {
            try
            {
                if (orderIds == null || orderIds.Length == 0)
                {
                    return Json(new { success = false, message = "Lütfen en az bir sipariş seçin." });
                }
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    // OrderStatusHistories removed - using simple status tracking
                    .Where(o => orderIds.Contains(o.Id))
                    .ToListAsync();
                if (!orders.Any())
                {
                    return Json(new { success = false, message = "Seçili siparişler bulunamadı." });
                }
                // Remove related data first
                foreach (var order in orders)
                {
                    _context.OrderItems.RemoveRange(order.OrderItems);
                    // OrderStatusHistories removed - no need to delete related records
                }
                _context.Orders.RemoveRange(orders);
                await _context.SaveChangesAsync();
                // Log bulk activity
                await _loggingService.LogBulkOperationAsync(
                    "BulkDelete",
                    EntityTypes.Order,
                    orderIds,
                    User.Identity?.Name,
                    User.Identity?.Name
                );
                return Json(new
                {
                    success = true,
                    message = $"{orders.Count} sipariş başarıyla silindi."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk order deletion");
                await _loggingService.LogErrorAsync(ex, "AdminController.BulkDeleteOrders");
                return Json(new { success = false, message = "Toplu silme sırasında hata oluştu." });
            }
        }
        /// <summary>
        /// Generate bulk invoices as ZIP file
        /// </summary>
        [HttpPost("orders/bulk-invoices")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBulkInvoices([FromForm] int[] orderIds)
        {
            try
            {
                if (orderIds == null || orderIds.Length == 0)
                {
                    return Json(new { success = false, message = "Lütfen en az bir sipariş seçin." });
                }
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .Where(o => orderIds.Contains(o.Id))
                    .ToListAsync();
                if (!orders.Any())
                {
                    return Json(new { success = false, message = "Seçili siparişler bulunamadı." });
                }
                // Create temporary directory for PDFs
                var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);
                try
                {
                    // Generate individual invoice PDFs
                    var pdfFiles = new List<string>();
                    foreach (var order in orders)
                    {
                        var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(order);
                        var fileName = $"fatura_{order.OrderNumber}.pdf";
                        var filePath = Path.Combine(tempDir, fileName);
                        await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                        pdfFiles.Add(filePath);
                    }
                    // Create ZIP file
                    var zipFileName = $"toplu_faturalar_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                    var zipPath = Path.Combine(tempDir, zipFileName);
                    using (var zip = new ZipArchive(System.IO.File.Create(zipPath), ZipArchiveMode.Create))
                    {
                        foreach (var pdfFile in pdfFiles)
                        {
                            var entry = zip.CreateEntry(Path.GetFileName(pdfFile));
                            using (var entryStream = entry.Open())
                            using (var fileStream = System.IO.File.OpenRead(pdfFile))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                    var zipBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
                    // Log bulk activity
                    await _loggingService.LogBulkOperationAsync(
                        "BulkInvoiceGeneration",
                        EntityTypes.Order,
                        orderIds,
                        User.Identity?.Name,
                        User.Identity?.Name
                    );
                    // Clean up temp directory
                    Directory.Delete(tempDir, true);
                    return File(zipBytes, "application/zip", zipFileName);
                }
                catch
                {
                    // Clean up temp directory in case of error
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bulk invoices");
                await _loggingService.LogErrorAsync(ex, "AdminController.GenerateBulkInvoices");
                return Json(new { success = false, message = "Toplu fatura oluşturma sırasında hata oluştu." });
            }
        }
        /// <summary>
        /// Get order status text in Turkish
        /// </summary>
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
        #region Site Settings
        // GET: Admin/SiteSettings
        [HttpGet("site-settings")]
        public async Task<IActionResult> SiteSettings()
        {
            try
            {
                // Check if any settings exist, if not create default ones
                var settingsCount = await _context.SiteSettings.CountAsync();
                if (settingsCount == 0)
                {
                    await CreateDefaultSiteSettings();
                }
                var settings = await _context.SiteSettings
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Category)
                    .ThenBy(s => s.SortOrder)
                    .ToListAsync();
                ViewData["Title"] = "Site Ayarları";
                ViewData["Description"] = "Sistem ayarları ve yapılandırma";
                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading site settings: {Message}", ex.Message);
                TempData["Error"] = $"Site ayarları yüklenirken hata oluştu: {ex.Message}";
                return View(new List<SiteSetting>());
            }
        }
        // POST: Admin/UpdateSiteSettings
        [HttpPost("site-settings/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSiteSettings(Dictionary<string, string> settings)
        {
            try
            {
                var updatedCount = 0;
                foreach (var kvp in settings)
                {
                    var setting = await _context.SiteSettings
                        .FirstOrDefaultAsync(s => s.Key == kvp.Key);
                    if (setting != null && setting.Value != kvp.Value)
                    {
                        setting.Value = kvp.Value;
                        setting.UpdatedDate = DateTime.Now;
                        updatedCount++;
                    }
                }
                if (updatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    // Cache'i temizle
                    _settingsService.ClearCache();
                    // Log admin activity
                    await _loggingService.LogActivityAsync(
                        ActivityActions.Update,
                        EntityTypes.SiteSetting,
                        null,
                        User.Identity?.Name,
                        User.Identity?.Name,
                        description: $"Site settings updated - {updatedCount} settings changed"
                    );
                    TempData["Success"] = $"{updatedCount} ayar başarıyla güncellendi.";
                }
                else
                {
                    TempData["Info"] = "Hiçbir ayar değiştirilmedi.";
                }
                return RedirectToAction("SiteSettings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site settings");
                TempData["Error"] = "Ayarlar güncellenirken hata oluştu: " + ex.Message;
                return RedirectToAction("SiteSettings");
            }
        }
        // POST: Admin/ResetSiteSettings
        [HttpPost("site-settings/reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetSiteSettings(string category)
        {
            try
            {
                // Default values for different categories
                var defaultValues = GetDefaultSiteSettingValues();
                var settingsToReset = await _context.SiteSettings
                    .Where(s => s.Category == category)
                    .ToListAsync();
                var resetCount = 0;
                foreach (var setting in settingsToReset)
                {
                    if (defaultValues.ContainsKey(setting.Key))
                    {
                        setting.Value = defaultValues[setting.Key];
                        setting.UpdatedDate = DateTime.Now;
                        resetCount++;
                    }
                }
                if (resetCount > 0)
                {
                    await _context.SaveChangesAsync();
                    // Cache'i temizle
                    _settingsService.ClearCache();
                    // Log admin activity
                    await _loggingService.LogActivityAsync(
                        ActivityActions.Update,
                        EntityTypes.SiteSetting,
                        null,
                        User.Identity?.Name,
                        User.Identity?.Name,
                        description: $"Site settings reset for category: {category} - {resetCount} settings"
                    );
                    TempData["Success"] = $"{category} kategorisindeki {resetCount} ayar varsayılan değerlere sıfırlandı.";
                }
                return RedirectToAction("SiteSettings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting site settings for category {Category}", category);
                TempData["Error"] = "Ayarlar sıfırlanırken hata oluştu: " + ex.Message;
                return RedirectToAction("SiteSettings");
            }
        }
        private async Task CreateDefaultSiteSettings()
        {
            var defaultSettings = new List<SiteSetting>
            {
                // General Settings
                new SiteSetting { Key = "SiteName", Value = "ILISAN Commerce", Category = "General", DisplayName = "Site Adı", DataType = "text", SortOrder = 1, IsActive = true },
                new SiteSetting { Key = "SiteTitle", Value = "ILISAN Savunma Sanayi E-Ticaret", Category = "General", DisplayName = "Site Başlığı", DataType = "text", SortOrder = 2, IsActive = true },
                new SiteSetting { Key = "SiteDescription", Value = "Türkiye'nin güvenilir savunma sanayi e-ticaret platformu", Category = "General", DisplayName = "Site Açıklaması", DataType = "textarea", SortOrder = 3, IsActive = true },
                new SiteSetting { Key = "SiteKeywords", Value = "savunma sanayi, e-ticaret, güvenlik, askeri malzeme", Category = "General", DisplayName = "Site Anahtar Kelimeleri", DataType = "textarea", SortOrder = 4, IsActive = true },
                new SiteSetting { Key = "FooterText", Value = "© 2024 ILISAN Commerce. Tüm hakları saklıdır.", Category = "General", DisplayName = "Footer Metni", DataType = "text", SortOrder = 5, IsActive = true },
                // Contact Settings
                new SiteSetting { Key = "ContactEmail", Value = "info@ilisan.com.tr", Category = "Contact", DisplayName = "İletişim E-postası", DataType = "email", SortOrder = 1, IsActive = true },
                new SiteSetting { Key = "ContactPhone", Value = "+90 (850) 532 5237", Category = "Contact", DisplayName = "İletişim Telefonu", DataType = "text", SortOrder = 2, IsActive = true },
                new SiteSetting { Key = "ContactAddress", Value = "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş", Category = "Contact", DisplayName = "İletişim Adresi", DataType = "textarea", SortOrder = 3, IsActive = true },
                // Social Media Settings
                new SiteSetting { Key = "FacebookUrl", Value = "", Category = "Social", DisplayName = "Facebook URL", DataType = "url", SortOrder = 1, IsActive = true },
                new SiteSetting { Key = "TwitterUrl", Value = "", Category = "Social", DisplayName = "Twitter URL", DataType = "url", SortOrder = 2, IsActive = true },
                new SiteSetting { Key = "InstagramUrl", Value = "", Category = "Social", DisplayName = "Instagram URL", DataType = "url", SortOrder = 3, IsActive = true },
                new SiteSetting { Key = "LinkedInUrl", Value = "", Category = "Social", DisplayName = "LinkedIn URL", DataType = "url", SortOrder = 4, IsActive = true },
                // Features Settings
                new SiteSetting { Key = "EnableUserRegistration", Value = "true", Category = "Features", DisplayName = "Kullanıcı Kaydına İzin Ver", DataType = "checkbox", SortOrder = 1, IsActive = true },
                new SiteSetting { Key = "EnableGuestCheckout", Value = "true", Category = "Features", DisplayName = "Misafir Alışverişine İzin Ver", DataType = "checkbox", SortOrder = 2, IsActive = true },
                new SiteSetting { Key = "ShowProductStock", Value = "true", Category = "Features", DisplayName = "Ürün Stok Durumunu Göster", DataType = "checkbox", SortOrder = 3, IsActive = true },
                new SiteSetting { Key = "EnableProductReviews", Value = "true", Category = "Features", DisplayName = "Ürün Yorumlarına İzin Ver", DataType = "checkbox", SortOrder = 4, IsActive = true }
            };
            _context.SiteSettings.AddRange(defaultSettings);
            await _context.SaveChangesAsync();
        }
        private Dictionary<string, string> GetDefaultSiteSettingValues()
        {
            return new Dictionary<string, string>
            {
                // Site General
                {"SiteName", "ILISAN Commerce"},
                {"SiteTitle", "ILISAN Savunma Sanayi E-Ticaret"},
                {"SiteDescription", "Türkiye'nin güvenilir savunma sanayi e-ticaret platformu"},
                {"MaintenanceMode", "false"},
                // Contact
                {"ContactEmail", "info@ilisan.com.tr"},
                {"ContactPhone", "+90 (850) 532 5237"},
                {"ContactAddress", "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş"},
                // Features
                {"EnableUserRegistration", "true"},
                {"EnableGuestCheckout", "true"}
            };
        }
        /// <summary>
        /// Update order status
        /// </summary>
        [HttpPost("orders/update-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status, PaymentStatus paymentStatus, ShippingStatus shippingStatus, string? trackingNumber = null, string? comment = null)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Orders");
                }
                var oldStatus = order.Status;
                var oldPaymentStatus = order.PaymentStatus;
                var oldShippingStatus = order.ShippingStatus;
                order.Status = status;
                order.PaymentStatus = paymentStatus;
                order.ShippingStatus = shippingStatus;
                if (!string.IsNullOrEmpty(trackingNumber))
                {
                    order.TrackingNumber = trackingNumber;
                }
                // UpdatedDate property'si yok, sadece log'da güncellenme zamanını tutuyoruz
                await _context.SaveChangesAsync();
                // Log admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Update,
                    EntityTypes.Order,
                    order.Id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    description: $"Order status updated: {oldStatus} -> {status}, Payment: {oldPaymentStatus} -> {paymentStatus}, Shipping: {oldShippingStatus} -> {shippingStatus}"
                );
                TempData["Success"] = "Sipariş durumu başarıyla güncellendi.";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                await _loggingService.LogErrorAsync(ex, "AdminController.UpdateOrderStatus");
                TempData["Error"] = "Sipariş durumu güncellenirken hata oluştu.";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
        }
        #endregion
        #region Test Email
        // GET: Admin/TestEmail
        [HttpGet("test-email")]
        public IActionResult TestEmail()
        {
            ViewData["Title"] = "Test Email Gönder";
            return View();
        }
        // POST: Admin/TestEmail
        [HttpPost("test-email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestEmail(TestEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool emailSent = false;
                    string emailContent = "";
                    var recipientName = model.RecipientName ?? "Test Musteri";
                    var nameParts = recipientName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : recipientName;
                    var lastName = nameParts.Length > 1 ? nameParts[1] : "Tester";
                    switch (model.EmailType)
                    {
                        case "welcome":
                            // Test welcome email
                            var testUser = new User
                            {
                                FirstName = model.RecipientName ?? "Test",
                                Email = model.RecipientEmail
                            };
                            emailSent = await _emailService.SendNewUserWelcomeAsync(testUser);
                            emailContent = "Hoş geldin email template'i gönderildi.";
                            break;
                                                case "order_confirmation":
                            // Test order confirmation
                            var testOrder = new Order
                            {
                                OrderNumber = "TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                GuestFirstName = firstName,
                                GuestLastName = lastName,
                                GuestEmail = model.RecipientEmail,
                                SubTotal = 150.00m,
                                TotalAmount = 150.00m,
                                ShippingCost = 0,
                                TaxAmount = 0,
                                OrderDate = DateTime.Now
                            };
                            emailSent = await _emailService.SendOrderConfirmationAsync(testOrder);
                            emailContent = "Siparis onay email template'i gonderildi.";
                            break;
                        case "password_reset":
                            // Test password reset
                            emailSent = await _emailService.SendPasswordResetEmailAsync(
                                model.RecipientEmail,
                                "test-reset-token-123",
                                model.RecipientName ?? "Test Kullanıcı");
                            emailContent = "Şifre sıfırlama email template'i gönderildi.";
                            break;
                                                case "shipping_notification":
                            // Test shipping notification
                            var testShippingOrder = new Order
                            {
                                OrderNumber = "TEST-SHIP-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                GuestFirstName = firstName,
                                GuestLastName = lastName,
                                GuestEmail = model.RecipientEmail,
                                SubTotal = 250.00m,
                                TotalAmount = 250.00m,
                                ShippingCost = 0,
                                TaxAmount = 0,
                                ShippingAddressText = "Test Address",
                                ShippingCity = "Test City",
                                OrderDate = DateTime.Now
                            };
                            emailSent = await _emailService.SendShippingNotificationAsync(
                                testShippingOrder,
                                "TRK123456789",
                                "Aras Kargo");
                            emailContent = "Kargo bildirimi email template'i gonderildi.";
                            break;
                                                case "delivery_confirmation":
                            // Test delivery confirmation
                            var testDeliveryOrder = new Order
                            {
                                OrderNumber = "TEST-DEL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                GuestFirstName = firstName,
                                GuestLastName = lastName,
                                GuestEmail = model.RecipientEmail,
                                SubTotal = 300.00m,
                                TotalAmount = 300.00m,
                                ShippingCost = 0,
                                TaxAmount = 0,
                                ShippingAddressText = "Test Address",
                                ShippingCity = "Test City",
                                OrderDate = DateTime.Now
                            };
                            emailSent = await _emailService.SendDeliveryConfirmationAsync(testDeliveryOrder);
                            emailContent = "Teslimat onayi email template'i gonderildi.";
                            break;
                                                case "order_cancellation":
                            // Test order cancellation
                            var testCancelOrder = new Order
                            {
                                OrderNumber = "TEST-CANCEL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                GuestFirstName = firstName,
                                GuestLastName = lastName,
                                GuestEmail = model.RecipientEmail,
                                SubTotal = 100.00m,
                                TotalAmount = 100.00m,
                                ShippingCost = 0,
                                TaxAmount = 0,
                                OrderDate = DateTime.Now
                            };
                            emailSent = await _emailService.SendOrderCancellationAsync(testCancelOrder, "Test iptal sebebi");
                            emailContent = "Siparis iptali email template'i gonderildi.";
                            break;
                        case "contact_form":
                            // Test contact form
                            emailSent = await _emailService.SendContactFormAsync(
                                model.RecipientName ?? "Test Gönderen",
                                model.RecipientEmail,
                                "+90 555 123 45 67",
                                "Test Konu",
                                "Bu bir test mesajıdır. Email template sistemi çalışıyor.");
                            emailContent = "İletişim formu email template'i gönderildi.";
                            break;
                        case "custom":
                            // Custom email
                            emailSent = await _emailService.SendEmailAsync(
                                model.RecipientEmail,
                                model.Subject ?? "Test Email",
                                model.Message ?? "Bu bir test email'idir.");
                            emailContent = "Özel email gönderildi.";
                            break;
                        default:
                            TempData["ErrorMessage"] = "Geçersiz email tipi seçildi.";
                            return View(model);
                    }
                    if (emailSent)
                    {
                        TempData["SuccessMessage"] = $"✅ {emailContent} {model.RecipientEmail} adresine başarıyla gönderildi.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"❌ Email gönderilemedi. Lütfen SMTP ayarlarını kontrol edin.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Test email gönderilirken hata oluştu: {Email}", model.RecipientEmail);
                    TempData["ErrorMessage"] = $"❌ Email gönderilirken hata oluştu: {ex.Message}";
                }
            }
            return View(model);
        }
        #endregion
        private static string GenerateVariantSku(string productCode, ProductVariant variant, int fallbackIndex)
        {
            var baseCode = string.IsNullOrWhiteSpace(productCode) ? "ILISAN" : productCode.Trim().ToUpperInvariant();
            baseCode = baseCode.Replace(" ", "-");
            var descriptor = !string.IsNullOrWhiteSpace(variant.Color)
                ? variant.Color
                : !string.IsNullOrWhiteSpace(variant.Size)
                    ? variant.Size
                    : variant.VariantName;
            if (string.IsNullOrWhiteSpace(descriptor))
            {
                descriptor = $"VAR{fallbackIndex + 1}";
            }
            var normalized = new string(descriptor.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                normalized = $"VAR{fallbackIndex + 1}";
            }
            if (normalized.Length > 8)
            {
                normalized = normalized.Substring(0, 8);
            }
            return $"{baseCode}-{normalized}";
        }
        private static void NormalizeVariantDefaults(IList<ProductVariant> variants)
        {
            if (variants == null || variants.Count == 0)
            {
                return;
            }
            var activeVariants = variants.Where(v => !v.IsDeleted && v.IsActive).ToList();
            if (!activeVariants.Any())
            {
                return;
            }
            var defaultVariant = activeVariants.FirstOrDefault(v => v.IsDefault);
            if (defaultVariant == null)
            {
                defaultVariant = activeVariants.First();
                defaultVariant.IsDefault = true;
            }
            foreach (var variant in activeVariants)
            {
                if (!ReferenceEquals(variant, defaultVariant))
                {
                    variant.IsDefault = false;
                }
            }
        }
        private static List<ProductVariant> PrepareVariantModels(string productCode, int minStockLevel, IEnumerable<ProductVariant>? submittedVariants)
        {
            var prepared = new List<ProductVariant>();
            if (submittedVariants == null)
            {
                return prepared;
            }
            var index = 0;
            foreach (var variant in submittedVariants)
            {
                if (variant == null)
                {
                    continue;
                }
                if (variant.IsDeleted)
                {
                    continue;
                }
                var hasMeaningfulData = !string.IsNullOrWhiteSpace(variant.VariantName)
                    || !string.IsNullOrWhiteSpace(variant.SKU)
                    || !string.IsNullOrWhiteSpace(variant.Color)
                    || !string.IsNullOrWhiteSpace(variant.Size);
                if (!hasMeaningfulData)
                {
                    continue;
                }
                var copy = new ProductVariant
                {
                    Id = variant.Id,
                    VariantName = variant.VariantName?.Trim() ?? string.Empty,
                    SKU = variant.SKU?.Trim(),
                    PriceAdjustment = variant.PriceAdjustment,
                    StockQuantity = variant.StockQuantity,
                    MinStockLevel = variant.MinStockLevel <= 0 ? minStockLevel : variant.MinStockLevel,
                    Color = variant.Color,
                    ColorCode = variant.ColorCode,
                    Size = variant.Size,
                    Material = variant.Material,
                    Weight = variant.Weight,
                    Dimensions = variant.Dimensions,
                    Desi = variant.Desi,
                    IsDefault = variant.IsDefault,
                    IsActive = variant.IsActive || variant.Id == 0,
                    SortOrder = variant.SortOrder,
                    IsDeleted = false
                };
                if (string.IsNullOrWhiteSpace(copy.SKU))
                {
                    copy.SKU = GenerateVariantSku(productCode, copy, index);
                }
                prepared.Add(copy);
                index++;
            }
            NormalizeVariantDefaults(prepared);
            for (int i = 0; i < prepared.Count; i++)
            {
                prepared[i].SortOrder = i;
                if (prepared[i].CreatedDate == default)
                {
                    prepared[i].CreatedDate = DateTime.Now;
                }
            }
            return prepared;
        }
        private async Task SyncVariantsAfterCreateAsync(Product product, List<ProductVariant> submittedVariants)
        {
            if (submittedVariants == null || submittedVariants.Count == 0)
            {
                return;
            }
            var prepared = PrepareVariantModels(product.ProductCode, product.MinStockLevel, submittedVariants);
            if (prepared.Count == 0)
            {
                return;
            }
            foreach (var variant in prepared)
            {
                variant.ProductId = product.Id;
                variant.Product = null;
                variant.CreatedDate = DateTime.Now;
                variant.UpdatedDate = null;
            }
            _context.ProductVariants.AddRange(prepared);
            await _context.SaveChangesAsync();
        }
        private async Task SyncVariantsAfterEditAsync(int productId, string productCode, int minStockLevel, List<ProductVariant> submittedVariants)
        {
            var prepared = PrepareVariantModels(productCode, minStockLevel, submittedVariants);
            var deletedIds = submittedVariants?.Where(v => v != null && v.IsDeleted && v.Id > 0)
                                               .Select(v => v.Id)
                                               .ToHashSet() ?? new HashSet<int>();
            var existingVariants = await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();
            var processedExistingIds = new HashSet<int>();
            var newVariants = new List<ProductVariant>();
            foreach (var variant in prepared)
            {
                if (variant.Id > 0)
                {
                    var existing = existingVariants.FirstOrDefault(v => v.Id == variant.Id);
                    if (existing != null)
                    {
                        processedExistingIds.Add(existing.Id);
                        existing.VariantName = variant.VariantName;
                        existing.SKU = variant.SKU;
                        existing.PriceAdjustment = variant.PriceAdjustment;
                        existing.StockQuantity = variant.StockQuantity;
                        existing.MinStockLevel = variant.MinStockLevel;
                        existing.Color = variant.Color;
                        existing.ColorCode = variant.ColorCode;
                        existing.Size = variant.Size;
                        existing.Material = variant.Material;
                        existing.Weight = variant.Weight;
                        existing.Dimensions = variant.Dimensions;
                        existing.Desi = variant.Desi;
                        existing.IsDefault = variant.IsDefault;
                        existing.IsActive = variant.IsActive;
                        existing.SortOrder = variant.SortOrder;
                        existing.UpdatedDate = DateTime.Now;
                    }
                }
                else
                {
                    variant.ProductId = productId;
                    variant.Product = null;
                    variant.CreatedDate = DateTime.Now;
                    variant.UpdatedDate = null;
                    if (variant.SortOrder <= 0)
                    {
                        variant.SortOrder = existingVariants.Count + newVariants.Count;
                    }
                    newVariants.Add(variant);
                }
            }
            foreach (var id in deletedIds)
            {
                var existing = existingVariants.FirstOrDefault(v => v.Id == id);
                if (existing != null)
                {
                    existing.IsActive = false;
                    existing.IsDefault = false;
                    existing.StockQuantity = 0;
                    existing.UpdatedDate = DateTime.Now;
                }
            }
            if (newVariants.Count > 0)
            {
                _context.ProductVariants.AddRange(newVariants);
            }
            var activeVariants = existingVariants.Where(v => v.IsActive && !deletedIds.Contains(v.Id)).ToList();
            activeVariants.AddRange(newVariants.Where(v => v.IsActive));
            NormalizeVariantDefaults(activeVariants);
            await _context.SaveChangesAsync();
        }
    }
}
