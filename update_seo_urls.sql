-- SEO dostu Türkçe URL'ler (türkçe karakter olmadan)
UPDATE Sliders SET ButtonUrl = '/urunler' WHERE ButtonUrl = '/Product';
UPDATE Sliders SET ButtonUrl = '/hakkimizda' WHERE ButtonUrl = '/Home/About';

-- Kategori slug'larını da SEO dostu yapalım
UPDATE Categories SET Slug = 'balistik-yelekler' WHERE Name LIKE '%Balistik%';
UPDATE Categories SET Slug = 'anti-radyasyon' WHERE Name LIKE '%Anti%';
UPDATE Categories SET Slug = 'askeri-tekstil' WHERE Name LIKE '%Askeri%' OR Name LIKE '%Tekstil%';

SELECT 'SEO URL''leri güncellendi!' as Message;
