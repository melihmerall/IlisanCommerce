-- ILISAN Commerce Admin User Create Script
-- Admin kullanıcısı oluşturur

-- Admin kullanıcısını oluştur (AdminUsers tablosuna)
INSERT INTO AdminUsers ([FirstName], [LastName], [Email], [PasswordHash], [PhoneNumber], [Role], [IsActive], [IsEmailConfirmed], [CreatedDate])
VALUES 
('Admin', 'User', 'admin@ilisan.com.tr', '$2a$11$K8vZ8vZ8vZ8vZ8vZ8vZ8vO8vZ8vZ8vZ8vZ8vZ8vZ8vZ8vZ8vZ8vZ8vZ', '+90 (850) 532 5237', 2, 1, 1, GETDATE());

-- Rapor
PRINT '================================================';
PRINT '           ADMIN USER CREATE RAPORU';
PRINT '================================================';
PRINT 'Admin kullanıcısı oluşturuldu.';
PRINT 'Email: admin@ilisan.com.tr';
PRINT 'Şifre: IlisanAdmin2024!@#';
PRINT '================================================';
