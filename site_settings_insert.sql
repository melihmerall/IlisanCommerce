-- ILISAN Commerce Site Settings Insert Script
-- Tüm gerekli site ayarlarını ekler

-- Site ayarları tablosunu temizle (isteğe bağlı)
-- DELETE FROM SiteSettings;

-- Site ayarlarını ekle
INSERT INTO SiteSettings ([Key], [Value], [Description], [SortOrder], [Category], [IsActive], [UpdatedDate], [DataType], [DisplayName], [IsRequired], [UpdatedBy])
VALUES 
-- Genel Ayarlar
('SiteName', 'ILISAN Commerce', 'Site adı', 1, 'General', 1, GETDATE(), 'text', 'Site Adı', 1, 'System'),
('SiteTitle', 'ILISAN Savunma Sanayi E-Ticaret', 'Site başlığı', 2, 'General', 1, GETDATE(), 'text', 'Site Başlığı', 1, 'System'),
('SiteDescription', 'Türkiye''nin güvenilir savunma sanayi e-ticaret platformu', 'Site açıklaması', 3, 'General', 1, GETDATE(), 'textarea', 'Site Açıklaması', 1, 'System'),
('SiteKeywords', 'savunma sanayi, e-ticaret, güvenlik, askeri malzeme, balistik yelek', 'Site anahtar kelimeleri', 4, 'General', 1, GETDATE(), 'textarea', 'Site Anahtar Kelimeleri', 0, 'System'),
('FooterText', '© 2024 ILISAN Commerce. Tüm hakları saklıdır.', 'Footer metni', 5, 'General', 1, GETDATE(), 'text', 'Footer Metni', 0, 'System'),

-- İletişim Bilgileri
('ContactEmail', 'info@ilisan.com.tr', 'İletişim e-postası', 1, 'Contact', 1, GETDATE(), 'email', 'İletişim E-postası', 1, 'System'),
('ContactPhone', '+90 (850) 532 5237', 'İletişim telefonu', 2, 'Contact', 1, GETDATE(), 'text', 'İletişim Telefonu', 1, 'System'),
('ContactAddress', 'Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş', 'İletişim adresi', 3, 'Contact', 1, GETDATE(), 'textarea', 'İletişim Adresi', 1, 'System'),
('WorkingHours', 'Pazartesi - Cuma: 08:00 - 18:00, Cumartesi: 09:00 - 15:00', 'Çalışma saatleri', 4, 'Contact', 1, GETDATE(), 'text', 'Çalışma Saatleri', 0, 'System'),

-- Sosyal Medya
('FacebookUrl', 'https://facebook.com/ilisan', 'Facebook URL', 1, 'Social', 1, GETDATE(), 'url', 'Facebook URL', 0, 'System'),
('TwitterUrl', 'https://twitter.com/ilisan', 'Twitter URL', 2, 'Social', 1, GETDATE(), 'url', 'Twitter URL', 0, 'System'),
('InstagramUrl', 'https://instagram.com/ilisansavunma', 'Instagram URL', 3, 'Social', 1, GETDATE(), 'url', 'Instagram URL', 0, 'System'),
('LinkedInUrl', 'https://linkedin.com/company/ilisan', 'LinkedIn URL', 4, 'Social', 1, GETDATE(), 'url', 'LinkedIn URL', 0, 'System'),
('YouTubeUrl', 'https://youtube.com/@ilisan', 'YouTube URL', 5, 'Social', 1, GETDATE(), 'url', 'YouTube URL', 0, 'System'),

-- Özellik Ayarları
('EnableUserRegistration', 'true', 'Kullanıcı kaydına izin ver', 1, 'Features', 1, GETDATE(), 'checkbox', 'Kullanıcı Kaydına İzin Ver', 0, 'System'),
('EnableGuestCheckout', 'true', 'Misafir alışverişine izin ver', 2, 'Features', 1, GETDATE(), 'checkbox', 'Misafir Alışverişine İzin Ver', 0, 'System'),
('ShowProductStock', 'true', 'Ürün stok durumunu göster', 3, 'Features', 1, GETDATE(), 'checkbox', 'Ürün Stok Durumunu Göster', 0, 'System'),
('EnableProductReviews', 'true', 'Ürün yorumlarına izin ver', 4, 'Features', 1, GETDATE(), 'checkbox', 'Ürün Yorumlarına İzin Ver', 0, 'System'),
('EnableWishlist', 'true', 'İstek listesi özelliğini etkinleştir', 5, 'Features', 1, GETDATE(), 'checkbox', 'İstek Listesi', 0, 'System'),
('EnableProductComparison', 'true', 'Ürün karşılaştırma özelliğini etkinleştir', 6, 'Features', 1, GETDATE(), 'checkbox', 'Ürün Karşılaştırma', 0, 'System'),

-- E-ticaret Ayarları
('Currency', 'TRY', 'Para birimi', 1, 'Ecommerce', 1, GETDATE(), 'select', 'Para Birimi', 1, 'System'),
('TaxRate', '18', 'KDV oranı (%)', 2, 'Ecommerce', 1, GETDATE(), 'number', 'KDV Oranı', 1, 'System'),
('FreeShippingThreshold', '500', 'Ücretsiz kargo eşiği (TL)', 3, 'Ecommerce', 1, GETDATE(), 'number', 'Ücretsiz Kargo Eşiği', 0, 'System'),
('DefaultShippingCost', '25', 'Varsayılan kargo ücreti (TL)', 4, 'Ecommerce', 1, GETDATE(), 'number', 'Varsayılan Kargo Ücreti', 0, 'System'),
('MinOrderAmount', '50', 'Minimum sipariş tutarı (TL)', 5, 'Ecommerce', 1, GETDATE(), 'number', 'Minimum Sipariş Tutarı', 0, 'System'),

-- SEO Ayarları
('GoogleAnalyticsId', '', 'Google Analytics ID', 1, 'SEO', 1, GETDATE(), 'text', 'Google Analytics ID', 0, 'System'),
('GoogleTagManagerId', '', 'Google Tag Manager ID', 2, 'SEO', 1, GETDATE(), 'text', 'Google Tag Manager ID', 0, 'System'),
('FacebookPixelId', '', 'Facebook Pixel ID', 3, 'SEO', 1, GETDATE(), 'text', 'Facebook Pixel ID', 0, 'System'),
('RobotsTxt', 'User-agent: *\nAllow: /', 'Robots.txt içeriği', 4, 'SEO', 1, GETDATE(), 'textarea', 'Robots.txt', 0, 'System'),

-- Güvenlik Ayarları
('MaintenanceMode', 'false', 'Bakım modu', 1, 'Security', 1, GETDATE(), 'checkbox', 'Bakım Modu', 0, 'System'),
('MaxLoginAttempts', '5', 'Maksimum giriş denemesi', 2, 'Security', 1, GETDATE(), 'number', 'Maksimum Giriş Denemesi', 0, 'System'),
('SessionTimeout', '30', 'Oturum zaman aşımı (dakika)', 3, 'Security', 1, GETDATE(), 'number', 'Oturum Zaman Aşımı', 0, 'System'),

-- Şirket Bilgileri
('CompanyName', 'ILISAN Savunma Sanayi', 'Şirket adı', 1, 'Company', 1, GETDATE(), 'text', 'Şirket Adı', 1, 'System'),
('CompanyTaxNumber', '1234567890', 'Vergi numarası', 2, 'Company', 1, GETDATE(), 'text', 'Vergi Numarası', 0, 'System'),
('CompanyFoundedYear', '2018', 'Kuruluş yılı', 3, 'Company', 1, GETDATE(), 'number', 'Kuruluş Yılı', 0, 'System'),
('CompanyVision', 'Savunma sanayinde dünya çapında tanınan, AR-GE odaklı bir lider firma olmak.', 'Şirket vizyonu', 4, 'Company', 1, GETDATE(), 'textarea', 'Şirket Vizyonu', 0, 'System'),
('CompanyMission', 'Milli savunma sanayinin geliştirilmesi ve tam bağımsızlığa katkı sağlamak.', 'Şirket misyonu', 5, 'Company', 1, GETDATE(), 'textarea', 'Şirket Misyonu', 0, 'System'),

-- Ana Sayfa Ayarları
('HomePageTitle', 'ILISAN Savunma Sanayi - Güvenliğin Adresi', 'Ana sayfa başlığı', 1, 'HomePage', 1, GETDATE(), 'text', 'Ana Sayfa Başlığı', 0, 'System'),
('HomePageDescription', 'Balistik yelekler, anti-rad ürünleri ve askeri tekstil ürünleri için güvenilir adresiniz.', 'Ana sayfa açıklaması', 2, 'HomePage', 1, GETDATE(), 'textarea', 'Ana Sayfa Açıklaması', 0, 'System'),
('AboutUsTitle', 'Biz Kimiz?', 'Hakkımızda başlığı', 3, 'HomePage', 1, GETDATE(), 'text', 'Hakkımızda Başlığı', 0, 'System'),
('AboutUsText', '2018''de AR-GE çalışmalarına başladığımız süreçten bu yana patentli ürünlerimizi sektörde rakipsiz hale getirdik.', 'Hakkımızda metni', 4, 'HomePage', 1, GETDATE(), 'textarea', 'Hakkımızda Metni', 0, 'System'),

-- Logo ve Görsel Ayarları
('Logo', '/images/logo/ilisan-logo.png', 'Site logosu', 1, 'Design', 1, GETDATE(), 'file', 'Site Logosu', 0, 'System'),
('Favicon', '/images/favicon.ico', 'Favicon', 2, 'Design', 1, GETDATE(), 'file', 'Favicon', 0, 'System'),
('DefaultProductImage', '/images/demo/demo_370x530.png', 'Varsayılan ürün resmi', 3, 'Design', 1, GETDATE(), 'file', 'Varsayılan Ürün Resmi', 0, 'System'),

-- Email Ayarları
('EmailFromName', 'ILISAN Shop', 'E-posta gönderen adı', 1, 'Email', 1, GETDATE(), 'text', 'E-posta Gönderen Adı', 1, 'System'),
('EmailFromAddress', 'shop@ilisan.com.tr', 'E-posta gönderen adresi', 2, 'Email', 1, GETDATE(), 'email', 'E-posta Gönderen Adresi', 1, 'System'),
('EmailReplyTo', 'info@ilisan.com.tr', 'E-posta yanıt adresi', 3, 'Email', 1, GETDATE(), 'email', 'E-posta Yanıt Adresi', 0, 'System'),

-- Bildirim Ayarları
('EnableEmailNotifications', 'true', 'E-posta bildirimlerini etkinleştir', 1, 'Notifications', 1, GETDATE(), 'checkbox', 'E-posta Bildirimleri', 0, 'System'),
('EnableSmsNotifications', 'false', 'SMS bildirimlerini etkinleştir', 2, 'Notifications', 1, GETDATE(), 'checkbox', 'SMS Bildirimleri', 0, 'System'),
('OrderNotificationEmail', 'orders@ilisan.com.tr', 'Sipariş bildirim e-postası', 3, 'Notifications', 1, GETDATE(), 'email', 'Sipariş Bildirim E-postası', 0, 'System');

-- Rapor
PRINT '================================================';
PRINT '           SITE SETTINGS INSERT RAPORU';
PRINT '================================================';
PRINT 'Toplam ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' site ayarı eklendi.';
PRINT '================================================';
