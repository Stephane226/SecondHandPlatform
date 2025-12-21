SecondHand Platform - İkinci El Eşya Satış Platformu
📋 Proje Özeti
ASP.NET Core MVC kullanılarak geliştirilmiş ikinci el eşya satış platformu. Kullanıcıların ürün ekleyebildiği, silebildiği, düzenleyebildiği ve diğer kullanıcıların ürünlerini görüntüleyebildiği bir web uygulaması.

🎯 Proje Amacı
ISE309 - Web Programlama dersi kapsamında ASP.NET Core platformunu kullanarak bir web uygulamasını baştan sona geliştirmek.

✨ Özellikler
✅ Kullanıcı Yönetimi: Kayıt, giriş, profil yönetimi

✅ Ürün Yönetimi: Ürün ekleme, silme, düzenleme, listeleme

✅ Kategori Yönetimi: Admin panelinden kategori CRUD işlemleri

✅ Resim Yükleme: Ürün resimleri yükleyebilme

✅ Rol Bazlı Yetkilendirme: Admin ve Kullanıcı rolleri

✅ Filtreleme: Kategori ve fiyata göre ürün filtreleme

✅ Responsive Tasarım: Bootstrap ile uyumlu arayüz

🏗️ Teknolojiler
Platform: ASP.NET Core 10.0

Mimari: MVC (Model-View-Controller)

Veritabanı: Entity Framework Core (Code-First)

Kimlik Doğrulama: ASP.NET Core Identity

Frontend: Bootstrap 5.3, jQuery

Database: SQL Server LocalDB

📁 Proje Yapısı

SecondHandPlatform/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   └── AdminController.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── Product.cs
│   └── Category.cs
├── Views/
│   ├── Home/
│   ├── Products/
│   ├── Admin/
│   └── Shared/
├── Data/
│   └── ApplicationDbContext.cs
├── ViewModels/
│   └── AdminViewModels.cs
└── wwwroot/
    └── uploads/


🚀 Kurulum Adımları
1. Gereksinimler
.NET SDK 10.0+

Visual Studio Code veya Visual Studio

SQL Server LocalDB

2. Proje Kurulumu
# Projeyi klonlayın
git clone https://github.com/Stephane226/SecondHandPlatform

# Proje dizinine girin
cd SecondHandPlatform

# Gerekli paketleri yükleyin
dotnet restore

# Veritabanını oluşturun
dotnet ef database update

# Uygulamayı çalıştırın
dotnet run


3. Giriş Bilgileri
Admin Hesabı:

Email: admin@admin.com

Şifre: Admin123!
------------------------------------
Test Kullanıcısı:

Email: test@gmail.com

Şifre: Mase329@219*



🔧 Veritabanı Yapısı
Tablolar
AspNetUsers - Kullanıcı bilgileri

AspNetRoles - Roller (Admin, User)

Categories - Ürün kategorileri

Products - Ürünler

İlişkiler
Bir kullanıcı birden fazla ürün ekleyebilir

Bir kategoriye ait birden fazla ürün olabilir

Her ürün bir kategoriye ve bir kullanıcıya aittir

🎨 Kullanıcı Arayüzü
1. Ana Sayfa
En son eklenen ürünler

Kategori filtreleme

Arama özelliği

2. Ürün Sayfaları
Tüm Ürünler: Tüm kullanıcıların ürünleri

Ürün Detay: Ürünün detaylı bilgileri

Ürün Ekle: Yeni ürün ekleme formu

Benim Ürünlerim: Kullanıcının kendi ürünleri

3. Admin Paneli
Dashboard: İstatistikler ve özet

Kategori Yönetimi: Kategori ekleme/silme/düzenleme

Ürün Yönetimi: Tüm ürünleri yönetme

Kullanıcı Yönetimi: Kullanıcıları yönetme ve rol atama

🛠️ Geliştirici Notları
Migration İşlemleri

# Yeni migration oluştur
dotnet ef migrations add MigrationAdi

# Database güncelle
dotnet ef database update

# Migration listesi
dotnet ef migrations list



📝 Proje Gereksinimleri 

ASP.NET Core MVC	✅ Tamamlandı
Entity Framework Core (Code-First)	✅ Tamamlandı
ASP.NET Core Identity	✅ Tamamlandı
Rol Bazlı Yetkilendirme	✅ Tamamlandı
Resim Yükleme	✅ Tamamlandı
Bootstrap ile Arayüz	✅ Tamamlandı
CRUD İşlemleri	✅ Tamamlandı
Filtreleme ve Arama	✅ Tamamlandı





    
