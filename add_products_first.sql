-- ILISAN Commerce - Önce Ürünleri Ekle
-- Products tablosu boş olduğu için önce ürünleri ekliyoruz

-- Kategoriler var mı kontrol et, yoksa ekle
INSERT INTO [Categories] ([Name], [Description], [Slug], [IsActive], [CreatedDate])
SELECT * FROM (
    VALUES 
    ('Balistik Yelekler', 'NATO standartlarında balistik koruma ürünleri', 'balistik-yelekler', 1, '2024-01-01'),
    ('Anti-Rad Ürünler', 'Radyasyon koruma ürünleri', 'anti-rad-urunler', 1, '2024-01-01'),
    ('Askeri Tekstil', 'TSK standartlarında tekstil ürünleri', 'askeri-tekstil', 1, '2024-01-01')
) AS C([Name], [Description], [Slug], [IsActive], [CreatedDate])
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE [Slug] = C.[Slug]);

-- Ürünleri ekle
INSERT INTO [Products] ([Name], [ProductCode], [Slug], [ShortDescription], [LongDescription], [Price], [ComparePrice], [CategoryId], [Brand], [Material], [Weight], [Color], [ProtectionLevel], [CertificateType], [StockQuantity], [MinStockLevel], [IsActive], [IsFeatured], [IsSpecialProduct], [MetaTitle], [MetaDescription], [CreatedDate])
VALUES 
-- Balistik Yelek 1
('ILISAN BV-01 Balistik Yelek', 'ILISAN-BV-01', 'ilisan-bv-01-balistik-yelek', 
 'NATO standartlarında Level IIIA koruma sağlayan hafif balistik yelek',
 'ILISAN BV-01 Balistik Yelek, NATO standartlarında Level IIIA koruma sağlayan son teknoloji balistik koruma sistemidir. Bor ve polietilen bazlı özel malzemelerle üretilen bu yelek, maksimum koruma ile minimum ağırlık kombinasyonu sunar.',
 2500.00, 3000.00, 1, 'ILISAN', 'Bor + Polietilen', '1.2 kg', 'Siyah', 3, 'NATO STANAG 2920',
 50, 5, 1, 1, 1, 'ILISAN BV-01 Balistik Yelek', 'NATO STANAG 2920 Level IIIA sertifikalı balistik yelek', '2024-01-01'),

-- Balistik Yelek 2
('ILISAN BV-02 Taktikal Yelek', 'ILISAN-BV-02', 'ilisan-bv-02-taktikal-yelek',
 'Özel kuvvetler için tasarlanmış hafif taktikal balistik yelek',
 'ILISAN BV-02 Taktikal Yelek, özel kuvvetler için özel olarak tasarlanmış hafif balistik koruma sistemidir. MOLLE uyumlu tasarımı ile ek ekipman taşıma imkanı sunar.',
 1800.00, 2200.00, 1, 'ILISAN', 'Aramid + Seramik', '0.9 kg', 'Kamuflaj', 2, 'NIJ Level II',
 30, 3, 1, 1, 0, 'ILISAN BV-02 Taktikal Yelek', 'Özel kuvvetler için taktikal balistik yelek', '2024-01-01'),

-- Anti-Rad Önlük
('ILISAN AR-01 Anti-Rad Önlük', 'ILISAN-AR-01', 'ilisan-ar-01-anti-rad-onluk',
 'Tıbbi personel için radyasyon koruyucu kurşun içermeyen önlük',
 'ILISAN AR-01 Anti-Rad Önlük, tıbbi personel için özel olarak geliştirilmiş kurşun içermeyen radyasyon koruma sistemidir. %98 radyasyon emilimi ile güvenli çalışma ortamı sağlar.',
 850.00, NULL, 2, 'ILISAN', 'Bor + Tekstil', '0.6 kg', 'Mavi', NULL, 'CE Belgeli',
 100, 10, 1, 1, 0, 'ILISAN AR-01 Anti-Rad Önlük', 'Kurşunsuz radyasyon koruyucu önlük', '2024-01-01'),

-- Kamuflaj Üniforma
('ILISAN AT-01 Kamuflaj Üniforma', 'ILISAN-AT-01', 'ilisan-at-01-kamuflaj-uniforma',
 'Askeri standartlarda kamuflaj üniforma takımı',
 'ILISAN AT-01 Kamuflaj Üniforma, TSK standartlarına uygun olarak üretilen yüksek kaliteli kamuflaj üniforma takımıdır. Dayanıklı polyester/pamuk karışımı kumaştan üretilmiştir.',
 350.00, NULL, 3, 'ILISAN', 'Polyester/Pamuk', '0.8 kg', 'Kamuflaj', NULL, 'TSK Onaylı',
 150, 15, 1, 1, 0, 'ILISAN AT-01 Kamuflaj Üniforma', 'TSK standartlarında kamuflaj üniforma', '2024-01-01');

-- Ürün resimlerini ekle
INSERT INTO [ProductImages] ([ProductId], [ImagePath], [AltText], [IsMainImage], [SortOrder], [CreatedDate])
VALUES 
(1, '/images/demo/demo_400x280.png', 'ILISAN BV-01 Balistik Yelek', 1, 1, '2024-01-01'),
(1, '/images/demo/demo_370x530.png', 'ILISAN BV-01 Yan Görünüm', 0, 2, '2024-01-01'),
(2, '/images/demo/demo_400x280.png', 'ILISAN BV-02 Taktikal Yelek', 1, 1, '2024-01-01'),
(2, '/images/demo/demo_370x530.png', 'ILISAN BV-02 Detay', 0, 2, '2024-01-01'),
(3, '/images/demo/demo_400x280.png', 'ILISAN AR-01 Anti-Rad Önlük', 1, 1, '2024-01-01'),
(3, '/images/demo/demo_370x530.png', 'ILISAN AR-01 Detay', 0, 2, '2024-01-01'),
(4, '/images/demo/demo_400x280.png', 'ILISAN AT-01 Kamuflaj Üniforma', 1, 1, '2024-01-01'),
(4, '/images/demo/demo_370x530.png', 'ILISAN AT-01 Detay', 0, 2, '2024-01-01');

-- Ürün varyantlarını ekle
INSERT INTO [ProductVariants] ([ProductId], [VariantName], [Size], [Color], [Material], [PriceAdjustment], [StockQuantity], [MinStockLevel], [SKU], [IsActive], [IsDefault], [SortOrder], [CreatedDate])
VALUES 
-- BV-01 varyantları
(1, 'Small', 'S', 'Siyah', 'Bor + Polietilen', 0.00, 15, 2, 'BV01-S-BLK', 1, 1, 1, '2024-01-01'),
(1, 'Medium', 'M', 'Siyah', 'Bor + Polietilen', 0.00, 20, 3, 'BV01-M-BLK', 1, 0, 2, '2024-01-01'),
(1, 'Large', 'L', 'Siyah', 'Bor + Polietilen', 100.00, 15, 2, 'BV01-L-BLK', 1, 0, 3, '2024-01-01'),
(1, 'X-Large', 'XL', 'Siyah', 'Bor + Polietilen', 150.00, 10, 2, 'BV01-XL-BLK', 1, 0, 4, '2024-01-01'),

-- BV-02 varyantları
(2, 'Medium Kamuflaj', 'M', 'Kamuflaj', 'Aramid + Seramik', 0.00, 12, 2, 'BV02-M-CAM', 1, 1, 1, '2024-01-01'),
(2, 'Large Kamuflaj', 'L', 'Kamuflaj', 'Aramid + Seramik', 50.00, 8, 1, 'BV02-L-CAM', 1, 0, 2, '2024-01-01'),

-- AR-01 varyantları
(3, 'Standard', 'One Size', 'Mavi', 'Bor + Tekstil', 0.00, 50, 5, 'AR01-OS-BLU', 1, 1, 1, '2024-01-01'),
(3, 'Premium', 'One Size', 'Beyaz', 'Bor + Tekstil', 100.00, 25, 3, 'AR01-OS-WHT', 1, 0, 2, '2024-01-01'),

-- AT-01 varyantları
(4, 'Medium', 'M', 'Kamuflaj', 'Polyester/Pamuk', 0.00, 40, 5, 'AT01-M-CAM', 1, 1, 1, '2024-01-01'),
(4, 'Large', 'L', 'Kamuflaj', 'Polyester/Pamuk', 0.00, 35, 5, 'AT01-L-CAM', 1, 0, 2, '2024-01-01'),
(4, 'X-Large', 'XL', 'Kamuflaj', 'Polyester/Pamuk', 25.00, 25, 3, 'AT01-XL-CAM', 1, 0, 3, '2024-01-01');

-- Site ayarları ekle
INSERT INTO [SiteSettings] ([Key], [Value], [Type], [Description], [IsActive], [CreatedDate])
SELECT * FROM (
    VALUES 
    ('CompanyAddress', 'Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş', 1, 'Şirket Adresi', 1, '2024-01-01'),
    ('CompanyWorkingHours', 'Pazartesi - Cuma: 08:00 - 18:00, Cumartesi: 09:00 - 15:00', 1, 'Çalışma Saatleri', 1, '2024-01-01'),
    ('SocialMediaFacebook', 'https://facebook.com/ilisan', 7, 'Facebook Hesabı', 1, '2024-01-01'),
    ('SocialMediaInstagram', 'https://instagram.com/ilisan', 7, 'Instagram Hesabı', 1, '2024-01-01'),
    ('SocialMediaLinkedIn', 'https://linkedin.com/company/ilisan', 7, 'LinkedIn Hesabı', 1, '2024-01-01')
) AS S([Key], [Value], [Type], [Description], [IsActive], [CreatedDate])
WHERE NOT EXISTS (SELECT 1 FROM SiteSettings WHERE [Key] = S.[Key]);

-- RAPOR
PRINT '🎉 ÜRÜNLER BAŞARIYLA EKLENDİ!';
PRINT '================================';

SELECT 'Kategori Sayısı' as [Tablo], COUNT(*) as [Adet] FROM Categories;
SELECT 'Ürün Sayısı' as [Tablo], COUNT(*) as [Adet] FROM Products;
SELECT 'Ürün Varyant Sayısı' as [Tablo], COUNT(*) as [Adet] FROM ProductVariants;
SELECT 'Ürün Resim Sayısı' as [Tablo], COUNT(*) as [Adet] FROM ProductImages;
SELECT 'Site Ayar Sayısı' as [Tablo], COUNT(*) as [Adet] FROM SiteSettings;

PRINT '================================';
PRINT '✅ Artık 4 ürün, 11 varyant ve resimler var!';
PRINT '🚀 Website test edilmeye hazır!';
