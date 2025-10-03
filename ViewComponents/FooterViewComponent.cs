using Microsoft.AspNetCore.Mvc;
using IlisanCommerce.Services;

namespace IlisanCommerce.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly SettingsService _settingsService;

        public FooterViewComponent(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new FooterViewModel
            {
                SiteName = await _settingsService.GetSettingAsync("SiteName", "ILISAN"),
                SitePhone = await _settingsService.GetSettingAsync("Phone", "+90 (850) 532 5237"),
                SiteEmail = await _settingsService.GetSettingAsync("Email", "info@ilisan.com.tr"),
                SiteAddress = await _settingsService.GetSettingAsync("Address", "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş"),
                CompanyAddress = await _settingsService.GetSettingAsync("CompanyAddress", "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş"),
                CompanyWorkingHours = await _settingsService.GetSettingAsync("CompanyWorkingHours", "Pazartesi - Cuma: 08:00 - 18:00"),
                FacebookUrl = await _settingsService.GetSettingAsync("SocialMediaFacebook", "#"),
                InstagramUrl = await _settingsService.GetSettingAsync("SocialMediaInstagram", "https://instagram.com/ilisansavunma"),
                LinkedInUrl = await _settingsService.GetSettingAsync("SocialMediaLinkedIn", "#"),
                CompanyFoundedYear = await _settingsService.GetSettingAsync("CompanyFoundedYear", "2018"),
                CompanyVision = await _settingsService.GetSettingAsync("CompanyVision", "Savunma sanayinde dünya çapında tanınan, AR-GE odaklı bir lider firma olmak."),
                CompanyMission = await _settingsService.GetSettingAsync("CompanyMission", "Milli savunma sanayinin geliştirilmesi ve tam bağımsızlığa katkı sağlamak.")
            };

            return View(model);
        }
    }

    public class FooterViewModel
    {
        public string SiteName { get; set; } = string.Empty;
        public string SitePhone { get; set; } = string.Empty;
        public string SiteEmail { get; set; } = string.Empty;
        public string SiteAddress { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string CompanyWorkingHours { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string LinkedInUrl { get; set; } = string.Empty;
        public string CompanyFoundedYear { get; set; } = string.Empty;
        public string CompanyVision { get; set; } = string.Empty;
        public string CompanyMission { get; set; } = string.Empty;
    }
}
