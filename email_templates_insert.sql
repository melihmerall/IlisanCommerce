-- ILISAN Commerce Email Templates Insert Script
-- Eksik email template'leri ekler

-- Müşteri Kayıt Hoş Geldin Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Müşteri Kayıt Hoş Geldin', 'ILISAN''a Hoş Geldiniz! 🎉', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Hoş Geldiniz</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .header .logo { font-size: 32px; margin-bottom: 10px; }
        .content { padding: 40px 30px; }
        .welcome-message { text-align: center; margin-bottom: 30px; }
        .welcome-message h2 { color: #2c3e50; margin: 0 0 15px 0; font-size: 24px; }
        .welcome-message p { color: #666; font-size: 16px; margin: 0; }
        .benefits { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .benefits h3 { color: #2c3e50; margin-top: 0; text-align: center; }
        .benefit-list { list-style: none; padding: 0; margin: 0; }
        .benefit-list li { padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .benefit-list li:last-child { border-bottom: none; }
        .benefit-icon { color: #28a745; font-weight: bold; margin-right: 10px; }
        .cta-section { text-align: center; margin: 30px 0; }
        .btn { display: inline-block; background: linear-gradient(135deg, #007bff, #0056b3); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; transition: transform 0.2s; }
        .btn:hover { transform: translateY(-2px); color: white; }
        .social-section { text-align: center; margin: 30px 0; }
        .social-links { margin: 20px 0; }
        .social-links a { display: inline-block; margin: 0 10px; color: #666; text-decoration: none; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer h4 { margin: 0 0 15px 0; color: #ecf0f1; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
        .footer .contact-info { margin: 15px 0; }
        .stats { display: flex; justify-content: space-around; text-align: center; margin: 25px 0; padding: 20px; background: #f8f9fa; border-radius: 10px; }
        .stat { flex: 1; }
        .stat-number { font-size: 24px; font-weight: bold; color: #007bff; display: block; }
        .stat-label { font-size: 14px; color: #666; }
        
        @media (max-width: 600px) {
            .container { margin: 0 10px; }
            .header, .content { padding: 20px 15px; }
            .stats { flex-direction: column; }
            .stat { margin-bottom: 15px; }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <div class="logo">🛡️</div>
            <h1>ILISAN Savunma Sanayi</h1>
            <p>Kalite ve Güvenin Adresi</p>
        </div>
        
        <div class="content">
            <div class="welcome-message">
                <h2>Hoş Geldin {FirstName}! 🎉</h2>
                <p>ILISAN ailesine katıldığın için çok mutluyuz. Hesabın başarıyla oluşturuldu ve artık tüm ayrıcalıklardan yararlanabilirsin.</p>
            </div>

            <div class="stats">
                <div class="stat">
                    <span class="stat-number">500+</span>
                    <span class="stat-label">Ürün Çeşidi</span>
                </div>
                <div class="stat">
                    <span class="stat-number">10K+</span>
                    <span class="stat-label">Mutlu Müşteri</span>
                </div>
                <div class="stat">
                    <span class="stat-number">24/7</span>
                    <span class="stat-label">Destek</span>
                </div>
            </div>

            <div class="benefits">
                <h3>🎁 Üyelik Avantajları</h3>
                <ul class="benefit-list">
                    <li><span class="benefit-icon">✓</span> Özel fiyatlar ve kampanyalar</li>
                    <li><span class="benefit-icon">✓</span> Hızlı ve güvenli alışveriş</li>
                    <li><span class="benefit-icon">✓</span> Sipariş takibi ve geçmişi</li>
                    <li><span class="benefit-icon">✓</span> 7/24 müşteri desteği</li>
                    <li><span class="benefit-icon">✓</span> Ücretsiz kargo fırsatları</li>
                </ul>
            </div>

            <div class="cta-section">
                <p style="margin-bottom: 20px; color: #666;">Alışverişe başlamak için hemen giriş yap!</p>
                <a href="https://ilisan.com.tr/giris" class="btn">Giriş Yap & Alışverişe Başla</a>
            </div>

            <div class="social-section">
                <p style="color: #666; margin-bottom: 15px;">Bizi sosyal medyada takip et:</p>
                <div class="social-links">
                    <a href="#">📘 Facebook</a>
                    <a href="#">📷 Instagram</a>
                    <a href="#">🐦 Twitter</a>
                    <a href="#">💼 LinkedIn</a>
                </div>
            </div>
        </div>

        <div class="footer">
            <h4>📞 İletişim Bilgileri</h4>
            <div class="contact-info">
                <p>📍 Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş</p>
                <p>☎️ +90 (850) 532 5237</p>
                <p>📧 info@ilisan.com.tr</p>
                <p>🌐 <a href="https://ilisan.com.tr" style="color: #3498db;">ilisan.com.tr</a></p>
            </div>
            <div style="margin-top: 25px; padding-top: 20px; border-top: 1px solid #34495e;">
                <p style="font-size: 14px; margin: 0;">© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
                <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                    Bu e-posta {Email} adresine gönderilmiştir. 
                    Eğer bu hesabı oluşturmadıysanız, bu e-postayı görmezden gelebilirsiniz.
                </p>
            </div>
        </div>
    </div>
</body>
</html>', 
6, 1, GETDATE(), 'Müşteri kayıt olduğunda gönderilen hoş geldin email template''i');

-- Şifre Sıfırlama Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Şifre Sıfırlama', 'ILISAN - Şifre Sıfırlama Talebi', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Şifre Sıfırlama</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .message { text-align: center; margin-bottom: 30px; }
        .message h2 { color: #2c3e50; margin: 0 0 15px 0; font-size: 24px; }
        .message p { color: #666; font-size: 16px; margin: 0; }
        .reset-section { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; text-align: center; }
        .btn { display: inline-block; background: linear-gradient(135deg, #dc3545, #c82333); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; transition: transform 0.2s; }
        .btn:hover { transform: translateY(-2px); color: white; }
        .security-info { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 20px; border-radius: 10px; margin: 25px 0; }
        .security-info h4 { color: #856404; margin-top: 0; }
        .security-info p { color: #856404; margin: 0; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🔐 Şifre Sıfırlama</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <div class="message">
                <h2>Merhaba {FirstName},</h2>
                <p>Hesabınız için şifre sıfırlama talebinde bulundunuz.</p>
            </div>

            <div class="reset-section">
                <h3>Şifrenizi Sıfırlamak İçin</h3>
                <p>Yeni şifre oluşturmak için aşağıdaki butona tıklayın:</p>
                <a href="{ResetLink}" class="btn">Şifremi Sıfırla</a>
                <p style="margin-top: 20px; font-size: 14px; color: #666;">
                    Bu link 24 saat geçerlidir.
                </p>
            </div>

            <div class="security-info">
                <h4>🛡️ Güvenlik Bilgisi</h4>
                <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz. Hesabınız güvende kalacaktır.</p>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta {Email} adresine gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
7, 1, GETDATE(), 'Şifre sıfırlama talebinde gönderilen email template''i');

-- Sipariş Durumu Güncelleme Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Sipariş Durumu Güncelleme', 'ILISAN - Sipariş Durumu Güncellendi #{OrderNumber}', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sipariş Durumu</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .order-info { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .order-info h3 { color: #2c3e50; margin-top: 0; }
        .info-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-weight: bold; color: #495057; }
        .info-value { color: #6c757d; }
        .status-badge { display: inline-block; padding: 8px 16px; border-radius: 20px; font-weight: bold; color: white; }
        .status-processing { background-color: #ffc107; }
        .status-shipped { background-color: #17a2b8; }
        .status-delivered { background-color: #28a745; }
        .cta-section { text-align: center; margin: 30px 0; }
        .btn { display: inline-block; background: linear-gradient(135deg, #007bff, #0056b3); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>📦 Sipariş Güncellemesi</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <h2>Merhaba {CustomerName},</h2>
            <p>Siparişinizin durumu güncellenmiştir.</p>

            <div class="order-info">
                <h3>📋 Sipariş Bilgileri</h3>
                <div class="info-row">
                    <span class="info-label">Sipariş No:</span>
                    <span class="info-value">#{OrderNumber}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Sipariş Tarihi:</span>
                    <span class="info-value">{OrderDate}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Toplam Tutar:</span>
                    <span class="info-value">{TotalAmount} TL</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Yeni Durum:</span>
                    <span class="info-value">
                        <span class="status-badge status-{StatusClass}">{StatusText}</span>
                    </span>
                </div>
            </div>

            <div class="cta-section">
                <p>Siparişinizi takip etmek için:</p>
                <a href="https://ilisan.com.tr/siparislerim" class="btn">Siparişlerimi Görüntüle</a>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta {CustomerEmail} adresine gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
2, 1, GETDATE(), 'Sipariş durumu güncellendiğinde gönderilen email template''i');

-- Kargo Bildirimi Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Kargo Bildirimi', 'ILISAN - Siparişiniz Kargoya Verildi #{OrderNumber}', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Kargo Bildirimi</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #17a2b8 0%, #138496 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .shipping-info { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .shipping-info h3 { color: #2c3e50; margin-top: 0; }
        .info-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-weight: bold; color: #495057; }
        .info-value { color: #6c757d; }
        .tracking-section { background-color: #e7f3ff; padding: 25px; border-radius: 10px; margin: 25px 0; text-align: center; }
        .tracking-number { font-size: 24px; font-weight: bold; color: #007bff; margin: 10px 0; }
        .cta-section { text-align: center; margin: 30px 0; }
        .btn { display: inline-block; background: linear-gradient(135deg, #17a2b8, #138496); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🚚 Kargo Bildirimi</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <h2>Merhaba {CustomerName},</h2>
            <p>Harika haber! Siparişiniz kargoya verilmiştir.</p>

            <div class="shipping-info">
                <h3>📦 Kargo Bilgileri</h3>
                <div class="info-row">
                    <span class="info-label">Sipariş No:</span>
                    <span class="info-value">#{OrderNumber}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Kargo Firması:</span>
                    <span class="info-value">{ShippingCompany}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Kargo Tarihi:</span>
                    <span class="info-value">{ShippingDate}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Tahmini Teslimat:</span>
                    <span class="info-value">{EstimatedDelivery}</span>
                </div>
            </div>

            <div class="tracking-section">
                <h3>🔍 Takip Numarası</h3>
                <div class="tracking-number">{TrackingNumber}</div>
                <p>Siparişinizi takip etmek için yukarıdaki numarayı kullanabilirsiniz.</p>
            </div>

            <div class="cta-section">
                <p>Kargo takibi için:</p>
                <a href="{TrackingLink}" class="btn">Kargo Takip Et</a>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta {CustomerEmail} adresine gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
3, 1, GETDATE(), 'Sipariş kargoya verildiğinde gönderilen email template''i');

-- Teslimat Onayı Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Teslimat Onayı', 'ILISAN - Siparişiniz Teslim Edildi #{OrderNumber}', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Teslimat Onayı</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .delivery-info { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .delivery-info h3 { color: #2c3e50; margin-top: 0; }
        .info-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-weight: bold; color: #495057; }
        .info-value { color: #6c757d; }
        .success-message { background-color: #d4edda; border: 1px solid #c3e6cb; padding: 20px; border-radius: 10px; margin: 25px 0; text-align: center; }
        .success-message h4 { color: #155724; margin-top: 0; }
        .success-message p { color: #155724; margin: 0; }
        .cta-section { text-align: center; margin: 30px 0; }
        .btn { display: inline-block; background: linear-gradient(135deg, #28a745, #20c997); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>✅ Teslimat Onayı</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <h2>Merhaba {CustomerName},</h2>
            <p>Siparişiniz başarıyla teslim edilmiştir!</p>

            <div class="success-message">
                <h4>🎉 Teslimat Tamamlandı!</h4>
                <p>Siparişiniz güvenli bir şekilde teslim edilmiştir. Memnun kaldıysanız değerlendirmenizi bekliyoruz.</p>
            </div>

            <div class="delivery-info">
                <h3>📦 Teslimat Bilgileri</h3>
                <div class="info-row">
                    <span class="info-label">Sipariş No:</span>
                    <span class="info-value">#{OrderNumber}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Teslimat Tarihi:</span>
                    <span class="info-value">{DeliveryDate}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Teslimat Adresi:</span>
                    <span class="info-value">{DeliveryAddress}</span>
                </div>
            </div>

            <div class="cta-section">
                <p>Ürününüzü değerlendirmek için:</p>
                <a href="https://ilisan.com.tr/siparislerim" class="btn">Siparişi Değerlendir</a>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta {CustomerEmail} adresine gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
4, 1, GETDATE(), 'Sipariş teslim edildiğinde gönderilen email template''i');

-- Sipariş İptali Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('Sipariş İptali', 'ILISAN - Siparişiniz İptal Edildi #{OrderNumber}', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sipariş İptali</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .cancellation-info { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .cancellation-info h3 { color: #2c3e50; margin-top: 0; }
        .info-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-weight: bold; color: #495057; }
        .info-value { color: #6c757d; }
        .refund-info { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 20px; border-radius: 10px; margin: 25px 0; }
        .refund-info h4 { color: #856404; margin-top: 0; }
        .refund-info p { color: #856404; margin: 0; }
        .cta-section { text-align: center; margin: 30px 0; }
        .btn { display: inline-block; background: linear-gradient(135deg, #007bff, #0056b3); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>❌ Sipariş İptali</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <h2>Merhaba {CustomerName},</h2>
            <p>Siparişiniz iptal edilmiştir.</p>

            <div class="cancellation-info">
                <h3>📋 İptal Bilgileri</h3>
                <div class="info-row">
                    <span class="info-label">Sipariş No:</span>
                    <span class="info-value">#{OrderNumber}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">İptal Tarihi:</span>
                    <span class="info-value">{CancellationDate}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">İptal Sebebi:</span>
                    <span class="info-value">{CancellationReason}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Toplam Tutar:</span>
                    <span class="info-value">{TotalAmount} TL</span>
                </div>
            </div>

            <div class="refund-info">
                <h4>💰 İade Bilgisi</h4>
                <p>Ödeme yaptıysanız, tutar 3-5 iş günü içinde hesabınıza iade edilecektir.</p>
            </div>

            <div class="cta-section">
                <p>Yeni sipariş vermek için:</p>
                <a href="https://ilisan.com.tr" class="btn">Alışverişe Devam Et</a>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta {CustomerEmail} adresine gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
5, 1, GETDATE(), 'Sipariş iptal edildiğinde gönderilen email template''i');

-- İletişim Formu Email Template
INSERT INTO EmailTemplates ([Name], [Subject], [Body], [TemplateType], [IsActive], [CreatedDate], [Description])
VALUES 
('İletişim Formu', 'ILISAN - İletişim Formu Mesajı', 
'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>İletişim Formu</title>
    <style>
        body { font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 20px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #6f42c1 0%, #5a32a3 100%); color: white; padding: 40px 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 28px; font-weight: 300; }
        .content { padding: 40px 30px; }
        .message-info { background-color: #f8f9fa; padding: 25px; border-radius: 10px; margin: 25px 0; }
        .message-info h3 { color: #2c3e50; margin-top: 0; }
        .info-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 10px 0; border-bottom: 1px solid #e9ecef; }
        .info-row:last-child { border-bottom: none; }
        .info-label { font-weight: bold; color: #495057; }
        .info-value { color: #6c757d; }
        .message-content { background-color: #e7f3ff; padding: 20px; border-radius: 10px; margin: 25px 0; }
        .message-content h4 { color: #0056b3; margin-top: 0; }
        .message-content p { color: #0056b3; margin: 0; }
        .footer { background-color: #2c3e50; color: white; padding: 30px; text-align: center; }
        .footer p { margin: 5px 0; color: #bdc3c7; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>📧 İletişim Formu</h1>
            <p>ILISAN Savunma Sanayi</p>
        </div>
        
        <div class="content">
            <h2>Yeni İletişim Formu Mesajı</h2>
            <p>Web sitenizden yeni bir iletişim formu mesajı alındı.</p>

            <div class="message-info">
                <h3>👤 Gönderen Bilgileri</h3>
                <div class="info-row">
                    <span class="info-label">Ad Soyad:</span>
                    <span class="info-value">{SenderName}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Email:</span>
                    <span class="info-value">{SenderEmail}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Telefon:</span>
                    <span class="info-value">{SenderPhone}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Konu:</span>
                    <span class="info-value">{Subject}</span>
                </div>
                <div class="info-row">
                    <span class="info-label">Gönderim Tarihi:</span>
                    <span class="info-value">{SendDate}</span>
                </div>
            </div>

            <div class="message-content">
                <h4>💬 Mesaj İçeriği</h4>
                <p>{Message}</p>
            </div>
        </div>

        <div class="footer">
            <p>© 2024 ILISAN Savunma Sanayi. Tüm hakları saklıdır.</p>
            <p style="font-size: 12px; margin: 5px 0 0 0; color: #95a5a6;">
                Bu e-posta admin paneline gönderilmiştir.
            </p>
        </div>
    </div>
</body>
</html>', 
8, 1, GETDATE(), 'İletişim formu gönderildiğinde admin''e gönderilen email template''i');

PRINT '================================================';
PRINT '           EMAIL TEMPLATES INSERT RAPORU';
PRINT '================================================';
PRINT 'Email template''leri başarıyla eklendi:';
PRINT '- Müşteri Kayıt Hoş Geldin (TemplateType: 6)';
PRINT '- Şifre Sıfırlama (TemplateType: 7)';
PRINT '- Sipariş Durumu Güncelleme (TemplateType: 2)';
PRINT '- Kargo Bildirimi (TemplateType: 3)';
PRINT '- Teslimat Onayı (TemplateType: 4)';
PRINT '- Sipariş İptali (TemplateType: 5)';
PRINT '- İletişim Formu (TemplateType: 8)';
PRINT '================================================';
