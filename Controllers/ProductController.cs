using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services;

namespace IlisanCommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly SettingsService _settingsService;

        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger, SettingsService settingsService)
        {
            _context = context;
            _logger = logger;
            _settingsService = settingsService;
        }

        // GET: Product
        public async Task<IActionResult> Index(int page = 1, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null, string? category = null, string? search = null)
        {
            var pageSize = 12;
            var query = _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Category filtering
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Slug == category);
            }

            // Search filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.ShortDescription.Contains(search));
            }

            // Price filtering
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(p => p.Price),
                "price-high" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            var totalProducts = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .ToListAsync();

            // Get current category if filtering by category
            Category? currentCategory = null;
            if (!string.IsNullOrEmpty(category))
            {
                currentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == category && c.IsActive);
            }

            var model = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategory = currentCategory,
                TotalProducts = totalProducts,
                CurrentPage = page,
                PageSize = pageSize,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SearchQuery = search,
                CategorySlug = category
            };

            ViewData["Title"] = "Ürünlerimiz";
            ViewData["Description"] = "ILISAN savunma sanayi ürünleri - Balistik yelekler, Anti-rad ürünleri, Askeri tekstil";

            return View(model);
        }

        // AJAX endpoint for filtering
        [HttpPost]
        public async Task<IActionResult> FilterProducts(int page = 1, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null, string? category = null, string? search = null)
        {
            var pageSize = 12;
            var query = _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Category filtering
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Slug == category);
            }

            // Search filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.ShortDescription.Contains(search));
            }

            // Price filtering
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(p => p.Price),
                "price-high" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            var totalProducts = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .ToListAsync();

            // Get current category if filtering by category
            Category? currentCategory = null;
            if (!string.IsNullOrEmpty(category))
            {
                currentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == category && c.IsActive);
            }

            var model = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategory = currentCategory,
                TotalProducts = totalProducts,
                CurrentPage = page,
                PageSize = pageSize,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SearchQuery = search,
                CategorySlug = category
            };

            return PartialView("_ProductGrid", model);
        }

        // GET: Product/Category/{slug}
        public async Task<IActionResult> Category(string slug, int page = 1, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            var pageSize = 12;
            var query = _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.CategoryId == category.Id);

            // Price filtering
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(p => p.Price),
                "price-high" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            var totalProducts = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .ToListAsync();

            var model = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategory = category,
                TotalProducts = totalProducts,
                CurrentPage = page,
                PageSize = pageSize,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            ViewData["Title"] = $"{category.Name} - Ürünlerimiz";
            ViewData["Description"] = category.Description ?? $"ILISAN {category.Name} ürünleri";

            return View("Index", model);
        }

        // GET: Product/Details/{slug}
        public async Task<IActionResult> Details(string slug)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSpecifications)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.VariantImages)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Related products
            var relatedProducts = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToListAsync();

            // Seçilebilir özellikleri oluştur
            var optionGroups = CreateProductOptionGroups(product);
            
            // Varsayılan varyantı seç
            var defaultVariant = product.ProductVariants.FirstOrDefault(v => v.IsDefault && v.IsActive) 
                                ?? product.ProductVariants.FirstOrDefault(v => v.IsActive);

            var model = new ProductDetailViewModel
            {
                Product = product,
                Variants = product.ProductVariants.Where(v => v.IsActive).ToList(),
                RelatedProducts = relatedProducts,
                OptionGroups = optionGroups,
                SelectedVariant = defaultVariant,
                SelectedVariantId = defaultVariant?.Id ?? 0
            };

            ViewData["Title"] = product.Name;
            ViewData["Description"] = product.ShortDescription ?? product.Name;

            return View(model);
        }

        // GET: Product/Search
        public async Task<IActionResult> Search(string? q, int page = 1, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var pageSize = 12;
            var query = _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Search filtering
            if (!string.IsNullOrEmpty(q))
            {
                var searchTerm = q.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.ShortDescription!.ToLower().Contains(searchTerm) ||
                    p.Description!.ToLower().Contains(searchTerm) ||
                    p.ProductCode.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm));
            }

            // Price filtering
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(p => p.Price),
                "price-high" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            var totalProducts = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .ToListAsync();

            var model = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                TotalProducts = totalProducts,
                CurrentPage = page,
                PageSize = pageSize,
                SearchQuery = q,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            ViewData["Title"] = string.IsNullOrEmpty(q) ? "Arama" : $"'{q}' için Arama Sonuçları";
            ViewData["Description"] = "ILISAN ürün arama sonuçları";

            return View("Index", model);
        }

        // AJAX: Get product variants
        [HttpGet]
        public async Task<IActionResult> GetProductVariants(int productId)
        {
            var variants = await _context.ProductVariants
                .Include(pv => pv.VariantImages)
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .Select(pv => new
                {
                    id = pv.Id,
                    name = pv.VariantName,
                    sku = pv.SKU,
                    price = pv.Price,
                    stock = pv.StockQuantity,
                    images = pv.VariantImages.Select(vi => vi.ImagePath).ToList()
                })
                .ToListAsync();

            return Json(variants);
        }

        // AJAX: Check product availability
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int productId, int? variantId = null)
        {
            if (variantId.HasValue)
            {
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.Id == variantId.Value && pv.ProductId == productId);
                
                if (variant == null)
                    return Json(new { available = false, message = "Varyant bulunamadı" });

                return Json(new { 
                    available = variant.StockQuantity > 0,
                    stock = variant.StockQuantity,
                    message = variant.StockQuantity > 0 ? "Stokta var" : "Stokta yok"
                });
            }
            else
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                    return Json(new { available = false, message = "Ürün bulunamadı" });

                return Json(new { 
                    available = product.StockQuantity > 0,
                    stock = product.StockQuantity,
                    message = product.StockQuantity > 0 ? "Stokta var" : "Stokta yok"
                });
            }
        }

        // AJAX: Get variant by selected options
        [HttpPost]
        public async Task<IActionResult> GetVariantByOptions(int productId, [FromBody] Dictionary<string, string> selectedOptions)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                    return Json(new { success = false, message = "Ürün bulunamadı" });

                // Seçilen özelliklere göre varyantı bul
                var matchingVariant = FindMatchingVariant(product.ProductVariants, selectedOptions);

                if (matchingVariant == null)
                    return Json(new { success = false, message = "Seçilen özelliklerle eşleşen varyant bulunamadı" });

                return Json(new
                {
                    success = true,
                    variant = new
                    {
                        id = matchingVariant.Id,
                        name = matchingVariant.VariantName,
                        price = matchingVariant.FinalPrice,
                        stock = matchingVariant.StockQuantity,
                        available = matchingVariant.StockQuantity > 0,
                        colorCode = matchingVariant.ColorCode,
                        images = matchingVariant.VariantImages?.Select(vi => vi.ImagePath).ToList() ?? new List<string>()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting variant by options for product {ProductId}", productId);
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        private List<ProductOptionGroup> CreateProductOptionGroups(Product product)
        {
            var optionGroups = new List<ProductOptionGroup>();

            if (!product.ProductVariants.Any())
                return optionGroups;

            // Renk seçenekleri
            var colorOptions = product.ProductVariants
                .Where(v => !string.IsNullOrEmpty(v.Color))
                .GroupBy(v => v.Color)
                .Select(g => new ProductOption
                {
                    Value = g.Key!,
                    DisplayName = g.Key!,
                    ColorCode = g.First().ColorCode,
                    PriceAdjustment = g.First().PriceAdjustment ?? 0,
                    IsAvailable = g.Any(v => v.StockQuantity > 0),
                    StockQuantity = g.Sum(v => v.StockQuantity)
                })
                .ToList();

            if (colorOptions.Any())
            {
                optionGroups.Add(new ProductOptionGroup
                {
                    Name = "color",
                    DisplayName = "Renk",
                    Options = colorOptions,
                    OptionType = "color"
                });
            }

            // Beden seçenekleri
            var sizeOptions = product.ProductVariants
                .Where(v => !string.IsNullOrEmpty(v.Size))
                .GroupBy(v => v.Size)
                .Select(g => new ProductOption
                {
                    Value = g.Key!,
                    DisplayName = g.Key!,
                    PriceAdjustment = g.First().PriceAdjustment ?? 0,
                    IsAvailable = g.Any(v => v.StockQuantity > 0),
                    StockQuantity = g.Sum(v => v.StockQuantity)
                })
                .ToList();

            if (sizeOptions.Any())
            {
                optionGroups.Add(new ProductOptionGroup
                {
                    Name = "size",
                    DisplayName = "Beden",
                    Options = sizeOptions,
                    OptionType = "radio"
                });
            }

            // Malzeme seçenekleri
            var materialOptions = product.ProductVariants
                .Where(v => !string.IsNullOrEmpty(v.Material))
                .GroupBy(v => v.Material)
                .Select(g => new ProductOption
                {
                    Value = g.Key!,
                    DisplayName = g.Key!,
                    PriceAdjustment = g.First().PriceAdjustment ?? 0,
                    IsAvailable = g.Any(v => v.StockQuantity > 0),
                    StockQuantity = g.Sum(v => v.StockQuantity)
                })
                .ToList();

            if (materialOptions.Any())
            {
                optionGroups.Add(new ProductOptionGroup
                {
                    Name = "material",
                    DisplayName = "Malzeme",
                    Options = materialOptions,
                    OptionType = "radio"
                });
            }

            return optionGroups;
        }

        private ProductVariant? FindMatchingVariant(ICollection<ProductVariant> variants, Dictionary<string, string> selectedOptions)
        {
            return variants.FirstOrDefault(variant =>
            {
                // Renk eşleşmesi
                if (selectedOptions.ContainsKey("color") && !string.IsNullOrEmpty(selectedOptions["color"]))
                {
                    if (variant.Color != selectedOptions["color"])
                        return false;
                }

                // Beden eşleşmesi
                if (selectedOptions.ContainsKey("size") && !string.IsNullOrEmpty(selectedOptions["size"]))
                {
                    if (variant.Size != selectedOptions["size"])
                        return false;
                }

                // Malzeme eşleşmesi
                if (selectedOptions.ContainsKey("material") && !string.IsNullOrEmpty(selectedOptions["material"]))
                {
                    if (variant.Material != selectedOptions["material"])
                        return false;
                }

                return true;
            });
        }

    }
}
