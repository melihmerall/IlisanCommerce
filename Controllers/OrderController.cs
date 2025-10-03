using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace IlisanCommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly ICartService _cartService;
        private readonly IEmailService _emailService;
        private readonly IIyzicoPaymentService _iyzicoPaymentService;
        private readonly IShippingService _shippingService;
        private readonly IConfiguration _configuration;
        private readonly SettingsService _settingsService;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger,
            ICartService cartService, IEmailService emailService, 
            IIyzicoPaymentService iyzicoPaymentService, IShippingService shippingService, 
            IConfiguration configuration, SettingsService settingsService)
        {
            _context = context;
            _logger = logger;
            _cartService = cartService;
            _emailService = emailService;
            _iyzicoPaymentService = iyzicoPaymentService;
            _shippingService = shippingService;
            _configuration = configuration;
            _settingsService = settingsService;
        }

        // GET: Order
        public async Task<IActionResult> Index(int page = 1, OrderStatus? status = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pageSize = 10;
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .Where(o => o.UserId == userId.Value);

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalOrders = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new OrderListViewModel
            {
                Orders = orders,
                TotalOrders = totalOrders,
                CurrentPage = page,
                PageSize = pageSize,
                StatusFilter = status
            };

            ViewData["Title"] = "Siparişlerim";
            ViewData["Description"] = "Sipariş geçmişi ve durumu";

            return View(model);
        }

        // GET: Order/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                // StatusHistory removed - using simple status tracking
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderDetailViewModel
            {
                Order = order,
                OrderItems = order.OrderItems.ToList(),
                // StatusHistory = null // Removed status history tracking
            };

            ViewData["Title"] = $"Sipariş #{order.OrderNumber}";
            ViewData["Description"] = "Sipariş detayları";

            return View(model);
        }

        // GET: Order/Checkout
        public async Task<IActionResult> Checkout(string? paymentType = null)
        {
            var userId = GetCurrentUserId();
            var sessionId = GetCurrentSessionId();

            // Misafir alışverişine izin verilip verilmediğini kontrol et
            var enableGuestCheckout = await _settingsService.GetEnableGuestCheckoutAsync();
            if (userId == null && !enableGuestCheckout)
            {
                TempData["Error"] = "Misafir alışverişi şu anda devre dışı. Lütfen giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
            
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetinizde ürün bulunmuyor.";
                return RedirectToAction("Index", "Cart");
            }

            // Kargo hesaplamaları
            var totalDesi = await _cartService.GetTotalDesiAsync(userId, sessionId);
            var shippingCost = await _cartService.CalculateShippingCostAsync(userId, sessionId);
            var cartTotal = cartItems.Sum(ci => ci.TotalPrice);

            var model = new CheckoutViewModel
            {
                Cart = new CartViewModel { CartItems = cartItems ?? new List<CartItem>() },
                Form = new CheckoutFormModel
                {
                    BillingAddress = new AddressFormModel(),
                    ShippingAddress = new AddressFormModel()
                },
                IsGuest = userId == null,
                SelectedPaymentType = paymentType,
                TotalDesi = totalDesi,
                ShippingCost = shippingCost,
                CartTotal = cartTotal,
                GrandTotal = cartTotal + shippingCost
            };


            if (userId.HasValue)
            {
                model.UserAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId.Value && a.IsActive)
                    .OrderByDescending(a => a.IsDefault)
                    .ThenBy(a => a.CreatedDate)
                    .ToListAsync();
                
                // Eğer kullanıcının varsayılan adresi varsa, onu seçili yap
                var defaultAddress = model.UserAddresses.FirstOrDefault(a => a.IsDefault);
                if (defaultAddress != null)
                {
                    model.Form.BillingAddressId = defaultAddress.Id;
                    model.Form.ShippingAddressId = defaultAddress.Id;
                }
            }

            ViewData["Title"] = "Ödeme";
            ViewData["Description"] = "Sipariş ödeme sayfası";
            ViewData["PaymentType"] = paymentType;

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = GetCurrentUserId();
            var sessionId = GetCurrentSessionId();

            var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetinizde ürün bulunmuyor.";
                return RedirectToAction("Index", "Cart");
            }

            // Cart bilgilerini model'e set et
            model.Cart = new CartViewModel
            {
                CartItems = cartItems,
                ShippingCost = await _cartService.CalculateShippingCostAsync(userId, sessionId)
            };

            model.IsGuest = userId == null;

            // Guest kullanıcı için zorunlu alanlar
            if (userId == null)
            {
                if (string.IsNullOrEmpty(model.Form.FirstName))
                    ModelState.AddModelError("Form.FirstName", "Ad zorunludur.");
                if (string.IsNullOrEmpty(model.Form.LastName))
                    ModelState.AddModelError("Form.LastName", "Soyad zorunludur.");
                if (string.IsNullOrEmpty(model.Form.Email))
                    ModelState.AddModelError("Form.Email", "E-posta adresi zorunludur.");
                if (string.IsNullOrEmpty(model.Form.Phone))
                    ModelState.AddModelError("Form.Phone", "Telefon numarası zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                if (userId.HasValue)
                {
                    model.UserAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId.Value && a.IsActive)
                        .ToListAsync();
                }
                return View(model);
            }

            try
            {
                // Sipariş oluştur
                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    PaymentMethod = model.Form.PaymentMethod,
                    PaymentStatus = PaymentStatus.Pending,
                    SubTotal = model.Cart.SubTotal,
                    ShippingCost = model.Cart.ShippingCost,
                    TotalAmount = model.Cart.Total,
                    Notes = model.Form.Notes,
                    CreatedDate = DateTime.Now,
                    OrderItems = new List<OrderItem>(),
                    // Guest user info
                    GuestEmail = userId == null ? model.Form.Email : null,
                    GuestFirstName = userId == null ? model.Form.FirstName : null,
                    GuestLastName = userId == null ? model.Form.LastName : null,
                    GuestPhone = userId == null ? model.Form.Phone : null
                };

                // Adresleri ayarla
                if (userId.HasValue)
                {
                    // Kayıtlı kullanıcı
                    if (model.Form.BillingAddressId.HasValue && model.Form.BillingAddressId.Value > 0)
                        order.BillingAddressId = model.Form.BillingAddressId.Value;
                    else
                        ModelState.AddModelError("Form.BillingAddressId", "Geçerli bir fatura adresi seçiniz.");

                    if (!model.Form.SameAsShipping)
                    {
                        if (model.Form.ShippingAddressId.HasValue && model.Form.ShippingAddressId.Value > 0)
                            order.ShippingAddressId = model.Form.ShippingAddressId.Value;
                        else
                            ModelState.AddModelError("Form.ShippingAddressId", "Geçerli bir teslimat adresi seçiniz.");
                    }
                    else
                    {
                        order.ShippingAddressId = order.BillingAddressId;
                    }

                    if (!ModelState.IsValid)
                    {
                        model.UserAddresses = await _context.Addresses
                            .Where(a => a.UserId == userId.Value && a.IsActive)
                            .ToListAsync();
                        return View(model);
                    }

                    // Adres bilgilerini Order modeline kopyala
                    var billingAddress = await _context.Addresses.FindAsync(order.BillingAddressId);
                    var shippingAddress = await _context.Addresses.FindAsync(order.ShippingAddressId);

                    if (billingAddress != null)
                    {
                        order.BillingFirstName = billingAddress.FullName?.Split(' ').FirstOrDefault();
                        order.BillingLastName = billingAddress.FullName?.Split(' ').Skip(1).FirstOrDefault();
                        order.BillingPhone = billingAddress.Phone;
                        order.BillingCity = billingAddress.City;
                        order.BillingDistrict = billingAddress.District;
                        order.BillingPostalCode = billingAddress.PostalCode;
                        order.BillingAddressText = $"{billingAddress.AddressLine1} {billingAddress.AddressLine2}".Trim();
                    }

                    if (shippingAddress != null)
                    {
                        order.ShippingFirstName = shippingAddress.FullName?.Split(' ').FirstOrDefault();
                        order.ShippingLastName = shippingAddress.FullName?.Split(' ').Skip(1).FirstOrDefault();
                        order.ShippingPhone = shippingAddress.Phone;
                        order.ShippingCity = shippingAddress.City;
                        order.ShippingDistrict = shippingAddress.District;
                        order.ShippingPostalCode = shippingAddress.PostalCode;
                        order.ShippingAddressText = $"{shippingAddress.AddressLine1} {shippingAddress.AddressLine2}".Trim();
                    }
                }
                else
                {
                    // Guest kullanıcı
                    var billingAddress = new Address
                    {
                        UserId = null,
                        AddressTitle = "Fatura Adresi",
                        FullName = $"{model.Form.FirstName} {model.Form.LastName}",
                        Phone = model.Form.Phone!,
                        City = model.Form.BillingAddress!.City,
                        District = model.Form.BillingAddress.District,
                        Neighborhood = model.Form.BillingAddress.Neighborhood,
                        AddressLine1 = model.Form.BillingAddress.AddressLine1,
                        AddressLine2 = model.Form.BillingAddress.AddressLine2,
                        PostalCode = model.Form.BillingAddress.PostalCode,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.Addresses.Add(billingAddress);
                    await _context.SaveChangesAsync();
                    order.BillingAddressId = billingAddress.Id;

                    order.BillingFirstName = model.Form.FirstName;
                    order.BillingLastName = model.Form.LastName;
                    order.BillingPhone = model.Form.Phone;
                    order.BillingCity = model.Form.BillingAddress.City;
                    order.BillingDistrict = model.Form.BillingAddress.District;
                    order.BillingPostalCode = model.Form.BillingAddress.PostalCode;
                    order.BillingAddressText = $"{model.Form.BillingAddress.AddressLine1} {model.Form.BillingAddress.AddressLine2}".Trim();

                    if (!model.Form.SameAsShipping)
                    {
                        var shippingAddress = new Address
                        {
                            UserId = null,
                            AddressTitle = "Teslimat Adresi",
                            FullName = model.Form.ShippingAddress!.FullName,
                            Phone = model.Form.ShippingAddress.Phone,
                            City = model.Form.ShippingAddress.City,
                            District = model.Form.ShippingAddress.District,
                            Neighborhood = model.Form.ShippingAddress.Neighborhood,
                            AddressLine1 = model.Form.ShippingAddress.AddressLine1,
                            AddressLine2 = model.Form.ShippingAddress.AddressLine2,
                            PostalCode = model.Form.ShippingAddress.PostalCode,
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        };
                        _context.Addresses.Add(shippingAddress);
                        await _context.SaveChangesAsync();
                        order.ShippingAddressId = shippingAddress.Id;

                        order.ShippingFirstName = model.Form.ShippingAddress.FullName?.Split(' ').FirstOrDefault() ?? model.Form.FirstName;
                        order.ShippingLastName = model.Form.ShippingAddress.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? model.Form.LastName;
                        order.ShippingPhone = model.Form.ShippingAddress.Phone ?? model.Form.Phone;
                        order.ShippingCity = model.Form.ShippingAddress.City;
                        order.ShippingDistrict = model.Form.ShippingAddress.District;
                        order.ShippingPostalCode = model.Form.ShippingAddress.PostalCode;
                        order.ShippingAddressText = $"{model.Form.ShippingAddress.AddressLine1} {model.Form.ShippingAddress.AddressLine2}".Trim();
                    }
                    else
                    {
                        order.ShippingAddressId = order.BillingAddressId;
                        order.ShippingFirstName = order.BillingFirstName;
                        order.ShippingLastName = order.BillingLastName;
                        order.ShippingPhone = order.BillingPhone;
                        order.ShippingCity = order.BillingCity;
                        order.ShippingDistrict = order.BillingDistrict;
                        order.ShippingPostalCode = order.BillingPostalCode;
                        order.ShippingAddressText = order.BillingAddressText;
                    }
                }

                // Order items ekle
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        ProductVariantId = cartItem.ProductVariantId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        ProductName = cartItem.Product.Name,
                        ProductCode = cartItem.Product.ProductCode,
                        VariantName = cartItem.ProductVariant?.VariantName
                    };
                    order.OrderItems.Add(orderItem);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Ödeme işlemi
                if (model.Form.PaymentMethod == PaymentMethod.CreditCard)
                {
                    return await InitializeIyzicoCheckoutFormAsync(order, model, cartItems);
                }
                else
                {
                    // Diğer ödeme metodları
                    await _cartService.ClearCartAsync(userId, sessionId);

                    try
                    {
                        await _emailService.SendOrderConfirmationAsync(order);
                        await _emailService.SendAdminOrderNotificationAsync(order);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send order confirmation emails for order {OrderId}", order.Id);
                    }

                    TempData["Message"] = $"Siparişiniz başarıyla alındı. Sipariş numaranız: {order.OrderNumber}";
                    return RedirectToAction("Success", new { id = order.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                ModelState.AddModelError(string.Empty, "Sipariş oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                if (userId.HasValue)
                {
                    model.UserAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId.Value && a.IsActive)
                        .ToListAsync();
                }
                return View(model);
            }
        }

        // GET: Order/Success/{id}
        public async Task<IActionResult> Success(int id)
        {
            var userId = GetCurrentUserId();
            
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id && (userId == null || o.UserId == userId));

            if (order == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Sipariş Başarılı";
            ViewData["Description"] = "Sipariş onay sayfası";

            return View(order);
        }

        // GET: Order/Track/{orderNumber}
        public async Task<IActionResult> Track(string orderNumber)
        {
            var order = await _context.Orders
                // StatusHistory removed - using simple status tracking
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = $"Sipariş Takip - {orderNumber}";
            ViewData["Description"] = "Sipariş durumu takibi";

            return View(order);
        }

        // Helper methods
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        private string GetCurrentSessionId()
        {
            var sessionId = HttpContext.Session.Id;
            if (string.IsNullOrEmpty(sessionId))
            {
                HttpContext.Session.SetString("OrderSession", Guid.NewGuid().ToString());
                sessionId = HttpContext.Session.Id;
            }
            return sessionId;
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100, 999);
            return $"IL{timestamp}{random}";
        }

        // Initialize Iyzico Checkout Form
        private async Task<IActionResult> InitializeIyzicoCheckoutFormAsync(Order order, CheckoutViewModel model, List<CartItem> cartItems)
        {
            try
            {
                var billingAddress = await _context.Addresses.FindAsync(order.BillingAddressId);
                var shippingAddress = await _context.Addresses.FindAsync(order.ShippingAddressId);

                if (billingAddress == null || shippingAddress == null)
                    throw new InvalidOperationException("Address not found");

                var callbackUrl = Url.Action("PaymentCallback", "Order", null, Request.Scheme);

                // BasketItem listesini hazırla ve toplamı kontrol et
                var basketItems = cartItems.Select(ci => new BasketItemModel
                {
                    Id = ci.ProductId.ToString(),
                    Name = ci.Product.Name,
                    Category = ci.Product.Category?.Name ?? "Ürün",
                    Price = Math.Round(ci.UnitPrice * ci.Quantity, 2) // her ürün toplam fiyatı
                }).ToList();

                // Toplam BasketItem fiyatı
                var basketTotal = basketItems.Sum(bi => bi.Price);

                // PaidPrice ile basketTotal eşleşmeli
                var paidPrice = Math.Round(order.TotalAmount, 2);
                if (basketTotal != paidPrice)
                {
                    // Eğer shipping cost eklenmemişse basketTotal < paidPrice olabilir
                    // Shipping cost için ayrı bir basket item ekle
                    var shippingCost = Math.Round(order.ShippingCost, 2);
                    if (shippingCost > 0)
                    {
                        basketItems.Add(new BasketItemModel
                        {
                            Id = "SHIPPING",
                            Name = "Kargo",
                            Category = "Kargo",
                            Price = shippingCost
                        });
                        basketTotal += shippingCost;
                    }

                    // Hala mismatch varsa paidPrice'ı basketTotal yap
                    paidPrice = basketTotal;
                }

                var checkoutFormRequest = new CheckoutFormRequest
                {
                    ConversationId = order.OrderNumber,
                    Price = Math.Round(order.SubTotal, 2),
                    PaidPrice = paidPrice,
                    BasketId = order.OrderNumber,
                    CallbackUrl = callbackUrl!,
                    EnabledInstallments = new List<int> { 1, 2, 3, 6, 9, 12 },

                    // Buyer
                    BuyerId = order.UserId?.ToString() ?? "guest",
                    BuyerName = billingAddress.FullName.Split(' ').FirstOrDefault() ?? "Guest",
                    BuyerSurname = billingAddress.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "User",
                    BuyerEmail = model.Form.Email ?? "guest@ilisan.com.tr",
                    BuyerGsmNumber = billingAddress.Phone,
                    BuyerIdentityNumber = "11111111111",
                    BuyerAddress = billingAddress.AddressLine1,
                    BuyerIp = GetClientIpAddress(),
                    BuyerCity = billingAddress.City,
                    BuyerZipCode = billingAddress.PostalCode,

                    // Shipping
                    ShippingContactName = shippingAddress.FullName,
                    ShippingCity = shippingAddress.City,
                    ShippingAddress = shippingAddress.AddressLine1,
                    ShippingZipCode = shippingAddress.PostalCode,

                    // Billing
                    BillingContactName = billingAddress.FullName,
                    BillingCity = billingAddress.City,
                    BillingAddress = billingAddress.AddressLine1,
                    BillingZipCode = billingAddress.PostalCode,

                    BasketItems = basketItems
                };

                var result = await _iyzicoPaymentService.InitializeCheckoutFormAsync(checkoutFormRequest);

                if (result.Status == "success")
                {
                    order.PaymentToken = result.Token;
                    order.PaymentStatus = PaymentStatus.Processing;
                    await _context.SaveChangesAsync();

                    var userId = GetCurrentUserId();
                    var sessionId = GetCurrentSessionId();
                    await _cartService.ClearCartAsync(userId, sessionId);

                    return Redirect(result.PaymentPageUrl);
                }
                else
                {
                    _logger.LogError("Iyzico checkout form initialization failed: {ErrorMessage}", result.ErrorMessage);
                    ModelState.AddModelError(string.Empty, "Ödeme sayfası açılamadı. Lütfen tekrar deneyin.");
                    throw new InvalidOperationException($"Checkout form initialization failed: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Iyzico payment for order {OrderId}", order.Id);
                order.PaymentStatus = PaymentStatus.Failed;
                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
                throw;
            }
        }

        // Payment Callback from Iyzico Checkout Form
        [HttpPost]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var token = Request.Form["token"].ToString();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Payment callback received with missing token");
                    TempData["Error"] = "Ödeme işlemi tamamlanamadı.";
                    return RedirectToAction("Index", "Cart");
                }

                // Retrieve payment result from Iyzico
                var result = await _iyzicoPaymentService.RetrieveCheckoutFormAsync(token);

                if (result.Status != "success")
                {
                    _logger.LogError("Payment failed for token: {Token}, Error: {ErrorMessage}", token, result.ErrorMessage);
                    TempData["Error"] = "Ödeme işlemi başarısız oldu. Lütfen tekrar deneyin.";
                    return RedirectToAction("Index", "Cart");
                }

                // Find order by conversation ID
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderNumber == result.ConversationId);

                if (order == null)
                {
                    _logger.LogError("Order not found for conversation ID: {ConversationId}", result.ConversationId);
                    TempData["Error"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                // Validate payment token
                if (order.PaymentToken != token)
                {
                    _logger.LogError("Payment token mismatch for order {OrderId}", order.Id);
                    TempData["Error"] = "Ödeme doğrulaması başarısız.";
                    return RedirectToAction("Index", "Cart");
                }

                // Check payment status
                if (result.PaymentStatus == "SUCCESS")
                {
                    // Update order status - Ödeme alındı ve siparş onaylandı
                    order.PaymentStatus = PaymentStatus.Paid; // "Ödeme Alındı" durumu
                    order.Status = OrderStatus.Confirmed; // "Onaylandı" durumu
                    order.PaymentTransactionId = result.PaymentId.ToString();
                    order.PaymentDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Send confirmation emails
                    try
                    {
                        await _emailService.SendOrderConfirmationAsync(order);
                        await _emailService.SendAdminOrderNotificationAsync(order);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send order confirmation emails for order {OrderId}", order.Id);
                    }

                    TempData["Success"] = $"Ödemeniz başarıyla tamamlandı. Sipariş numaranız: {order.OrderNumber}";
                    return RedirectToAction("Success", new { id = order.Id });
                }
                else
                {
                    // Payment failed
                    order.PaymentStatus = PaymentStatus.Failed;
                    order.Status = OrderStatus.Cancelled;
                    await _context.SaveChangesAsync();

                    _logger.LogWarning("Payment failed for order {OrderId}, Status: {PaymentStatus}", order.Id, result.PaymentStatus);
                    TempData["Error"] = "Ödeme işlemi başarısız oldu.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                TempData["Error"] = "Ödeme işlemi sırasında bir hata oluştu.";
                return RedirectToAction("Index", "Cart");
            }
        }

        // Webhook endpoint for Iyzico payment notifications
        [HttpPost]
        [Route("api/payment/webhook")]
        public async Task<IActionResult> PaymentWebhook()
        {
            try
            {
                string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                _logger.LogInformation("Payment webhook received: {RequestBody}", requestBody);

                // Parse webhook data
                var webhookData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

                if (webhookData == null)
                {
                    _logger.LogError("Invalid webhook data received");
                    return BadRequest("Invalid webhook data");
                }

                // Extract required fields
                var iyziEventType = webhookData.GetValueOrDefault("iyziEventType")?.ToString();
                var paymentConversationId = webhookData.GetValueOrDefault("paymentConversationId")?.ToString();
                var status = webhookData.GetValueOrDefault("status")?.ToString();
                var paymentId = webhookData.GetValueOrDefault("paymentId")?.ToString();

                _logger.LogInformation("Webhook details - EventType: {EventType}, ConversationId: {ConversationId}, Status: {Status}, PaymentId: {PaymentId}",
                    iyziEventType, paymentConversationId, status, paymentId);

                if (iyziEventType == "CHECKOUTFORM_AUTH" && !string.IsNullOrEmpty(paymentConversationId))
                {
                    // Find order by conversation ID
                    var order = await _context.Orders
                        .FirstOrDefaultAsync(o => o.OrderNumber == paymentConversationId);

                    if (order == null)
                    {
                        _logger.LogWarning("Order not found for webhook conversation ID: {ConversationId}", paymentConversationId);
                        return Ok(new { message = "Order not found" });
                    }

                    // Update order based on webhook status
                    if (status == "SUCCESS")
                    {
                        if (order.PaymentStatus != PaymentStatus.Paid)
                        {
                            order.PaymentStatus = PaymentStatus.Paid; // "Ödeme Alındı"
                            order.Status = OrderStatus.Confirmed; // "Onaylandı"
                            order.PaymentTransactionId = paymentId;
                            order.PaymentDate = DateTime.Now;

                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Order {OrderId} payment confirmed via webhook", order.Id);
                        }
                    }
                    else if (status == "FAILURE")
                    {
                        if (order.PaymentStatus != PaymentStatus.Failed)
                        {
                            order.PaymentStatus = PaymentStatus.Failed;
                            order.Status = OrderStatus.Cancelled;
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Order {OrderId} payment failed via webhook", order.Id);
                        }
                    }
                }

                // Return 200 to acknowledge webhook receipt
                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment webhook");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Get client IP address
        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;
            if (ipAddress != null)
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                return ipAddress.ToString();
            }
            return "127.0.0.1";
        }

        // Legacy: This method has been replaced by Checkout Form implementation
        // Keeping it commented for reference but not used anymore
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessIyzicoPayment(IyzicoPaymentRequest model)
        {
            // This method is no longer used - replaced by Checkout Form flow
            return BadRequest("This payment method is no longer supported. Please use the standard checkout flow.");
        }
        */

        // GET: Order/Success
        public async Task<IActionResult> Success(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return RedirectToAction("Index", "Home");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Sipariş Tamamlandı";
            ViewData["Description"] = "Siparişiniz başarıyla tamamlandı";

            return View(order);
        }
    }
}
