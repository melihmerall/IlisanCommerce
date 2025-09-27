-- ILISAN Commerce Final Seed Data Script
-- Enum değerleriyle düzeltilmiş versiyon

-- Site ayarları doğru enum değerleriyle ekle
INSERT INTO [SiteSettings] ([Key], [Value], [Type], [Description], [CreatedDate])
SELECT * FROM (
    VALUES 
    ('CompanyAddress', 'Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş', 1, 'Şirket Adresi', '2024-01-01'),
    ('CompanyWorkingHours', 'Pazartesi - Cuma: 08:00 - 18:00, Cumartesi: 09:00 - 15:00', 1, 'Çalışma Saatleri', '2024-01-01'),
    ('SocialMediaFacebook', 'https://facebook.com/ilisan', 7, 'Facebook Hesabı', '2024-01-01'),
    ('SocialMediaInstagram', 'https://instagram.com/ilisan', 7, 'Instagram Hesabı', '2024-01-01'),
    ('SocialMediaLinkedIn', 'https://linkedin.com/company/ilisan', 7, 'LinkedIn Hesabı', '2024-01-01'),
    ('CompanyFoundedYear', '2018', 5, 'Şirket Kuruluş Yılı', '2024-01-01'),
    ('CompanyVision', 'Tam Bağımsız Türkiye için güvenlik çözümleri', 3, 'Şirket Vizyonu', '2024-01-01')
) AS S([Key], [Value], [Type], [Description], [CreatedDate])
WHERE NOT EXISTS (SELECT 1 FROM SiteSettings WHERE [Key] = S.[Key]);

-- Email şablonları doğru enum değerleriyle ekle
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmailTemplates')
BEGIN
    INSERT INTO [EmailTemplates] ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate])
    SELECT * FROM (
        VALUES 
        ('WelcomeEmail', 'ILISAN''a Hoş Geldiniz!', 
         '<h1>Hoş Geldiniz {CustomerName}!</h1><p>ILISAN ailesine katıldığınız için teşekkür ederiz.</p>', 
         6, 1, '2024-01-01'),
        ('OrderConfirmationEmail', 'Siparişiniz Onaylandı - {OrderNumber}', 
         '<h1>Siparişiniz Alındı</h1><p>Sipariş No: {OrderNumber}</p><p>Toplam: {TotalAmount} TL</p>', 
         1, 1, '2024-01-01'),
        ('ContactFormEmail', 'Yeni İletişim Mesajı', 
         '<h1>Yeni İletişim Mesajı</h1><p>Ad: {FullName}</p><p>Email: {Email}</p><p>Mesaj: {Message}</p>', 
         8, 1, '2024-01-01')
    ) AS E([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate])
    WHERE NOT EXISTS (SELECT 1 FROM EmailTemplates WHERE [Name] = E.[Name]);
END

-- Mevcut ürünler için ürün varyantları ekle
DECLARE @ProductCount INT;
SELECT @ProductCount = COUNT(*) FROM Products;

IF @ProductCount >= 2
BEGIN
    INSERT INTO [ProductVariants] ([ProductId], [VariantName], [Size], [Color], [Material], [PriceAdjustment], [StockQuantity], [MinStockLevel], [SKU], [IsActive], [IsDefault], [SortOrder], [CreatedDate])
    SELECT * FROM (
        VALUES 
        (1, 'Small', 'S', 'Siyah', 'Bor + Polietilen', 0.00, 15, 2, 'BV01-S-BLK', 1, 1, 1, '2024-01-01'),
        (1, 'Medium', 'M', 'Siyah', 'Bor + Polietilen', 0.00, 20, 3, 'BV01-M-BLK', 1, 0, 2, '2024-01-01'),
        (1, 'Large', 'L', 'Siyah', 'Bor + Polietilen', 100.00, 15, 2, 'BV01-L-BLK', 1, 0, 3, '2024-01-01'),
        (2, 'Medium Kamuflaj', 'M', 'Kamuflaj', 'Aramid + Seramik', 0.00, 12, 2, 'BV02-M-CAM', 1, 1, 1, '2024-01-01')
    ) AS V(ProductId, VariantName, Size, Color, Material, PriceAdjustment, StockQuantity, MinStockLevel, SKU, IsActive, IsDefault, SortOrder, CreatedDate)
    WHERE NOT EXISTS (SELECT 1 FROM ProductVariants WHERE SKU = V.SKU);

    PRINT 'Ürün varyantları eklendi.';
END

-- Mevcut ürünler için ek resimler ekle
INSERT INTO [ProductImages] ([ProductId], [ImagePath], [AltText], [IsMainImage], [SortOrder], [CreatedDate])
SELECT P.Id, '/images/demo/demo_370x530.png', P.Name + ' Detay', 0, 2, '2024-01-01'
FROM Products P
WHERE NOT EXISTS (
    SELECT 1 FROM ProductImages PI 
    WHERE PI.ProductId = P.Id AND PI.IsMainImage = 0
);

-- Ek kategoriler ekle
INSERT INTO [Categories] ([Name], [Description], [Slug], [IsActive], [CreatedDate])
SELECT * FROM (
    VALUES 
    ('Güvenlik Ekipmanları', 'Güvenlik ve koruyucu ekipmanlar', 'guvenlik-ekipmanlari', 1, '2024-01-01'),
    ('Teknik Tekstil', 'Özel teknik tekstil ürünleri', 'teknik-tekstil', 1, '2024-01-01'),
    ('Tıbbi Koruyucu', 'Tıbbi personel için koruyucu ürünler', 'tibbi-koruyucu', 1, '2024-01-01')
) AS C([Name], [Description], [Slug], [IsActive], [CreatedDate])
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE [Slug] = C.[Slug]);

-- Sonuçları göster
SELECT 'SEED DATA RAPORU' as Bilgi;
SELECT 'Kullanıcı Sayısı:' as Tablo, COUNT(*) as Adet FROM Users;
SELECT 'Kategori Sayısı:' as Tablo, COUNT(*) as Adet FROM Categories;
SELECT 'Ürün Sayısı:' as Tablo, COUNT(*) as Adet FROM Products;
SELECT 'Ürün Varyant Sayısı:' as Tablo, COUNT(*) as Adet FROM ProductVariants;
SELECT 'Ürün Resim Sayısı:' as Tablo, COUNT(*) as Adet FROM ProductImages;
SELECT 'Site Ayar Sayısı:' as Tablo, COUNT(*) as Adet FROM SiteSettings;
SELECT 'Slider Sayısı:' as Tablo, COUNT(*) as Adet FROM Sliders;

PRINT 'TÜM SEED DATA BAŞARIYLA EKLENDİ! ✅';
