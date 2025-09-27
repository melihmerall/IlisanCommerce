using IlisanCommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IlisanCommerce.Services
{
    public class SettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "site_settings";

        public SettingsService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> GetSettingAsync(string key, string defaultValue = "")
        {
            // Cache'den kontrol et
            if (_cache.TryGetValue(CACHE_KEY, out Dictionary<string, string>? cachedSettings))
            {
                if (cachedSettings != null && cachedSettings.ContainsKey(key))
                {
                    return cachedSettings[key];
                }
            }

            // Veritabanından al
            var setting = await _context.SiteSettings
                .Where(s => s.Key == key && s.IsActive)
                .FirstOrDefaultAsync();

            var value = setting?.Value ?? defaultValue;

            // Cache'i güncelle
            await RefreshCacheAsync();

            return value;
        }

        public async Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false)
        {
            var value = await GetSettingAsync(key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        public async Task<int> GetIntSettingAsync(string key, int defaultValue = 0)
        {
            var value = await GetSettingAsync(key, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        public async Task<Dictionary<string, string>> GetAllSettingsAsync()
        {
            // Cache'den kontrol et
            if (_cache.TryGetValue(CACHE_KEY, out Dictionary<string, string>? cachedSettings))
            {
                return cachedSettings ?? new Dictionary<string, string>();
            }

            // Veritabanından al
            var settings = await _context.SiteSettings
                .Where(s => s.IsActive)
                .ToDictionaryAsync(s => s.Key, s => s.Value ?? "");

            // Cache'e kaydet
            _cache.Set(CACHE_KEY, settings, TimeSpan.FromMinutes(30));

            return settings;
        }

        public async Task RefreshCacheAsync()
        {
            var settings = await _context.SiteSettings
                .Where(s => s.IsActive)
                .ToDictionaryAsync(s => s.Key, s => s.Value ?? "");

            _cache.Set(CACHE_KEY, settings, TimeSpan.FromMinutes(30));
        }

        // Kolay erişim için method'lar
        public async Task<bool> GetEnableUserRegistrationAsync() => await GetBoolSettingAsync("EnableUserRegistration", true);
        public async Task<bool> GetEnableGuestCheckoutAsync() => await GetBoolSettingAsync("EnableGuestCheckout", true);
        public async Task<bool> GetShowProductStockAsync() => await GetBoolSettingAsync("ShowProductStock", true);
        public async Task<bool> GetEnableProductReviewsAsync() => await GetBoolSettingAsync("EnableProductReviews", true);
        public async Task<string> GetSiteNameAsync() => await GetSettingAsync("SiteName", "ILISAN Commerce");
        public async Task<string> GetSiteTitleAsync() => await GetSettingAsync("SiteTitle", "ILISAN Savunma Sanayi E-Ticaret");
        public async Task<string> GetSiteDescriptionAsync() => await GetSettingAsync("SiteDescription", "Türkiye'nin güvenilir savunma sanayi e-ticaret platformu");
        public async Task<string> GetContactEmailAsync() => await GetSettingAsync("ContactEmail", "info@ilisan.com.tr");
        public async Task<string> GetContactPhoneAsync() => await GetSettingAsync("ContactPhone", "+90 (850) 532 5237");
        public async Task<string> GetContactAddressAsync() => await GetSettingAsync("ContactAddress", "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş");
        public async Task<string> GetFooterTextAsync() => await GetSettingAsync("FooterText", "© 2024 ILISAN Commerce. Tüm hakları saklıdır.");
        public async Task<string> GetSiteKeywordsAsync() => await GetSettingAsync("SiteKeywords", "savunma sanayi, e-ticaret, güvenlik, askeri malzeme");
        public async Task<string> GetFacebookUrlAsync() => await GetSettingAsync("FacebookUrl", "");
        public async Task<string> GetTwitterUrlAsync() => await GetSettingAsync("TwitterUrl", "");
        public async Task<string> GetInstagramUrlAsync() => await GetSettingAsync("InstagramUrl", "");
        public async Task<string> GetLinkedInUrlAsync() => await GetSettingAsync("LinkedInUrl", "");

        public void ClearCache()
        {
            _cache.Remove(CACHE_KEY);
        }
    }
}