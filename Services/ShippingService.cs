using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;

namespace IlisanCommerce.Services
{
    public interface IShippingService
    {
        Task<ShippingRate?> GetShippingRateAsync(decimal totalDesi);
        Task<decimal> CalculateShippingCostAsync(decimal totalDesi, decimal cartTotal);
        Task<List<ShippingRate>> GetActiveShippingRatesAsync();
        Task<ShippingRate?> GetDefaultShippingRateAsync();
        Task<decimal> GetTotalDesiAsync(List<CartItem> cartItems);
    }

    public class ShippingService : IShippingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShippingService> _logger;

        public ShippingService(ApplicationDbContext context, ILogger<ShippingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ShippingRate?> GetShippingRateAsync(decimal totalDesi)
        {
            try
            {
                var shippingRate = await _context.ShippingRates
                    .Where(sr => sr.IsActive && 
                                sr.MinDesi <= totalDesi && 
                                (sr.MaxDesi == null || sr.MaxDesi >= totalDesi))
                    .OrderBy(sr => sr.SortOrder)
                    .FirstOrDefaultAsync();

                return shippingRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shipping rate for desi: {TotalDesi}", totalDesi);
                return null;
            }
        }

        public async Task<decimal> CalculateShippingCostAsync(decimal totalDesi, decimal cartTotal)
        {
            try
            {
                var shippingRate = await GetShippingRateAsync(totalDesi);
                
                if (shippingRate == null)
                {
                    _logger.LogWarning("No shipping rate found for desi: {TotalDesi}", totalDesi);
                    return 0;
                }

                // Ücretsiz kargo kontrolü
                if (shippingRate.FreeShippingThreshold.HasValue && 
                    cartTotal >= shippingRate.FreeShippingThreshold.Value)
                {
                    return 0;
                }

                return shippingRate.ShippingCost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating shipping cost for desi: {TotalDesi}, cart total: {CartTotal}", 
                    totalDesi, cartTotal);
                return 0;
            }
        }

        public async Task<List<ShippingRate>> GetActiveShippingRatesAsync()
        {
            try
            {
                return await _context.ShippingRates
                    .Where(sr => sr.IsActive)
                    .OrderBy(sr => sr.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active shipping rates");
                return new List<ShippingRate>();
            }
        }

        public async Task<ShippingRate?> GetDefaultShippingRateAsync()
        {
            try
            {
                return await _context.ShippingRates
                    .Where(sr => sr.IsActive && sr.IsDefault)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default shipping rate");
                return null;
            }
        }

        public Task<decimal> GetTotalDesiAsync(List<CartItem> cartItems)
        {
            try
            {
                decimal totalDesi = 0;

                foreach (var item in cartItems)
                {
                    // Ürün varyantı varsa varyantın desi bilgisini kullan, yoksa ana ürünün desi bilgisini kullan
                    decimal itemDesi = 1.0m; // Varsayılan değer

                    if (item.ProductVariant != null)
                    {
                        // Varyant için desi bilgisi yoksa ana ürünün desi bilgisini kullan
                        itemDesi = item.ProductVariant.Desi ?? item.Product.Desi;
                    }
                    else
                    {
                        itemDesi = item.Product.Desi;
                    }

                    totalDesi += itemDesi * item.Quantity;
                }

                return Task.FromResult(totalDesi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total desi for cart items");
                return Task.FromResult(0m);
            }
        }
    }
}
