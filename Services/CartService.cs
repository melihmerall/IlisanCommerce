using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;

namespace IlisanCommerce.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsAsync(int? userId, string? sessionId);
        Task<CartItem?> AddToCartAsync(int productId, int? variantId, int quantity, int? userId, string? sessionId);
        Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int? userId, string? sessionId);
        Task<bool> RemoveFromCartAsync(int cartItemId, int? userId, string? sessionId);
        Task<bool> ClearCartAsync(int? userId, string? sessionId);
        Task<decimal> GetCartTotalAsync(int? userId, string? sessionId);
        Task<int> GetCartItemCountAsync(int? userId, string? sessionId);
        Task<bool> MergeGuestCartToUserAsync(string sessionId, int userId);
        Task<bool> IsProductInCartAsync(int productId, int? variantId, int? userId, string? sessionId);
        Task<decimal> GetTotalDesiAsync(int? userId, string? sessionId);
        Task<decimal> CalculateShippingCostAsync(int? userId, string? sessionId);
    }

    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;
        private readonly IShippingService _shippingService;

        public CartService(ApplicationDbContext context, ILogger<CartService> logger, IShippingService shippingService)
        {
            _context = context;
            _logger = logger;
            _shippingService = shippingService;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int? userId, string? sessionId)
        {
            var query = _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                .Include(ci => ci.ProductVariant)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(ci => ci.UserId == userId.Value);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                query = query.Where(ci => ci.SessionId == sessionId);
            }
            else
            {
                return new List<CartItem>();
            }

            return await query.ToListAsync();
        }

        public async Task<CartItem?> AddToCartAsync(int productId, int? variantId, int quantity, int? userId, string? sessionId)
        {
            try
            {
                // Ürün kontrolü
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found or inactive", productId);
                    return null;
                }

                // Varyant kontrolü (eğer varsa)
                ProductVariant? variant = null;
                if (variantId.HasValue)
                {
                    variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(pv => pv.Id == variantId.Value && pv.ProductId == productId && pv.IsActive);

                    if (variant == null)
                    {
                        _logger.LogWarning("Product variant {VariantId} not found or inactive", variantId);
                        return null;
                    }

                    // Stok kontrolü (varyant için)
                    if (variant.StockQuantity < quantity)
                    {
                        _logger.LogWarning("Insufficient stock for variant {VariantId}. Available: {Stock}, Requested: {Quantity}", 
                            variantId, variant.StockQuantity, quantity);
                        return null;
                    }
                }
                else
                {
                    // Ana ürün stok kontrolü
                    if (product.StockQuantity < quantity)
                    {
                        _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Stock}, Requested: {Quantity}", 
                            productId, product.StockQuantity, quantity);
                        return null;
                    }
                }

                // Mevcut sepet öğesi kontrolü
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => 
                        ci.ProductId == productId && 
                        ci.ProductVariantId == variantId &&
                        ((userId.HasValue && ci.UserId == userId.Value) ||
                         (!userId.HasValue && ci.SessionId == sessionId)));

                if (existingCartItem != null)
                {
                    // Mevcut öğeyi güncelle
                    var newQuantity = existingCartItem.Quantity + quantity;
                    var availableStock = variant?.StockQuantity ?? product.StockQuantity;

                    if (newQuantity > availableStock)
                    {
                        _logger.LogWarning("Total quantity would exceed available stock");
                        return null;
                    }

                    existingCartItem.Quantity = newQuantity;
                    existingCartItem.UpdatedDate = DateTime.Now;
                    
                    await _context.SaveChangesAsync();
                    return existingCartItem;
                }

                // Yeni sepet öğesi oluştur
                var unitPrice = variant?.FinalPrice ?? product.Price;
                _logger.LogInformation("Creating new cart item - UserId: {UserId}, SessionId: {SessionId}, ProductId: {ProductId}", userId, sessionId, productId);
                
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    UserId = userId,
                    SessionId = sessionId
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} (Variant: {VariantId}) added to cart for {User}", 
                    productId, variantId, userId?.ToString() ?? sessionId);

                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart", productId);
                return null;
            }
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int? userId, string? sessionId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Include(ci => ci.ProductVariant)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId &&
                        ((userId.HasValue && ci.UserId == userId.Value) ||
                         (!userId.HasValue && ci.SessionId == sessionId)));

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                    return false;
                }

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    // Stok kontrolü
                    var availableStock = cartItem.ProductVariant?.StockQuantity ?? cartItem.Product.StockQuantity;
                    if (quantity > availableStock)
                    {
                        _logger.LogWarning("Requested quantity {Quantity} exceeds available stock {Stock}", 
                            quantity, availableStock);
                        return false;
                    }

                    cartItem.Quantity = quantity;
                    cartItem.UpdatedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId}", cartItemId);
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId, int? userId, string? sessionId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId &&
                        ((userId.HasValue && ci.UserId == userId.Value) ||
                         (!userId.HasValue && ci.SessionId == sessionId)));

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                    return false;
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cart item {CartItemId} removed", cartItemId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int? userId, string? sessionId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(ci => (userId.HasValue && ci.UserId == userId.Value) ||
                                (!userId.HasValue && ci.SessionId == sessionId))
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Cart cleared for {User}", userId?.ToString() ?? sessionId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for {User}", userId?.ToString() ?? sessionId);
                return false;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int? userId, string? sessionId)
        {
            var cartItems = await GetCartItemsAsync(userId, sessionId);
            return cartItems.Sum(ci => ci.TotalPrice);
        }

        public async Task<int> GetCartItemCountAsync(int? userId, string? sessionId)
        {
            var cartItems = await GetCartItemsAsync(userId, sessionId);
            return cartItems.Sum(ci => ci.Quantity);
        }

        public async Task<bool> MergeGuestCartToUserAsync(string sessionId, int userId)
        {
            try
            {
                var guestCartItems = await _context.CartItems
                    .Where(ci => ci.SessionId == sessionId)
                    .ToListAsync();

                if (!guestCartItems.Any())
                    return true;

                foreach (var guestItem in guestCartItems)
                {
                    // Kullanıcının sepetinde aynı ürün var mı kontrol et
                    var existingUserItem = await _context.CartItems
                        .FirstOrDefaultAsync(ci => 
                            ci.UserId == userId && 
                            ci.ProductId == guestItem.ProductId && 
                            ci.ProductVariantId == guestItem.ProductVariantId);

                    if (existingUserItem != null)
                    {
                        // Miktarları birleştir
                        existingUserItem.Quantity += guestItem.Quantity;
                        existingUserItem.UpdatedDate = DateTime.Now;
                        _context.CartItems.Remove(guestItem);
                    }
                    else
                    {
                        // Misafir sepet öğesini kullanıcıya aktar
                        guestItem.UserId = userId;
                        guestItem.SessionId = null;
                        guestItem.UpdatedDate = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Guest cart merged to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging guest cart to user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> IsProductInCartAsync(int productId, int? variantId, int? userId, string? sessionId)
        {
            return await _context.CartItems
                .AnyAsync(ci => 
                    ci.ProductId == productId && 
                    ci.ProductVariantId == variantId &&
                    ((userId.HasValue && ci.UserId == userId.Value) ||
                     (!userId.HasValue && ci.SessionId == sessionId)));
        }

        public async Task<decimal> GetTotalDesiAsync(int? userId, string? sessionId)
        {
            try
            {
                var cartItems = await GetCartItemsAsync(userId, sessionId);
                return await _shippingService.GetTotalDesiAsync(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total desi for cart");
                return 0;
            }
        }

        public async Task<decimal> CalculateShippingCostAsync(int? userId, string? sessionId)
        {
            try
            {
                var cartItems = await GetCartItemsAsync(userId, sessionId);
                var totalDesi = await _shippingService.GetTotalDesiAsync(cartItems);
                var cartTotal = cartItems.Sum(ci => ci.TotalPrice);
                
                return await _shippingService.CalculateShippingCostAsync(totalDesi, cartTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating shipping cost for cart");
                return 0;
            }
        }
    }
}
