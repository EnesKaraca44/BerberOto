using BerberOto.Models;
using Microsoft.EntityFrameworkCore;

namespace BerberOto.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique slug index
            modelBuilder.Entity<Shop>()
                .HasIndex(s => s.Slug)
                .IsUnique();

            // Seed Shops
            modelBuilder.Entity<Shop>().HasData(
                new Shop
                {
                    Id = 1,
                    Name = "Ahmet Berber",
                    Slug = "ahmet-berber",
                    Description = "15 yıllık tecrübe ile İstanbul'un en prestijli berber salonu. Klasik ve modern tarzı bir arada sunuyoruz.",
                    Phone = "+90 (212) 555 0142",
                    Address = "Atatürk Caddesi No:42, Beyoğlu",
                    City = "İstanbul",
                    OwnerName = "Ahmet Yılmaz",
                    Password = "admin123"
                },
                new Shop
                {
                    Id = 2,
                    Name = "Elite Kuaför",
                    Slug = "elite-kuafor",
                    Description = "Ankara'nın kalbinde premium erkek bakım deneyimi. Trend saç modelleri ve özel bakım hizmetleri.",
                    Phone = "+90 (312) 444 1234",
                    Address = "Kızılay Meydanı, Çankaya",
                    City = "Ankara",
                    OwnerName = "Burak Özdemir",
                    Password = "admin123"
                }
            );

            // Seed Barbers for Shop 1 (Ahmet Berber)
            modelBuilder.Entity<Barber>().HasData(
                new Barber { Id = 1, ShopId = 1, FullName = "Ahmet Yılmaz", Title = "Kurucu & Baş Berber", Bio = "15 yıllık deneyim ile klasik ve modern kesim teknikleri konusunda uzman." },
                new Barber { Id = 2, ShopId = 1, FullName = "Mehmet Kaya", Title = "Kıdemli Berber", Bio = "Sakal tasarımı ve fade kesim konusunda ödüllü uzman." },
                new Barber { Id = 3, ShopId = 1, FullName = "Can Demir", Title = "Stil Uzmanı", Bio = "Trend saç modelleri ve renklendirme konusunda yaratıcı dokunuşlar." }
            );

            // Seed Barbers for Shop 2 (Elite Kuaför)
            modelBuilder.Entity<Barber>().HasData(
                new Barber { Id = 4, ShopId = 2, FullName = "Burak Özdemir", Title = "Kurucu & Stil Direktörü", Bio = "Uluslararası fuarlarda ödüllü saç tasarım uzmanı." },
                new Barber { Id = 5, ShopId = 2, FullName = "Emre Aksoy", Title = "Sakal Uzmanı", Bio = "Geleneksel ustura tıraşı ve modern sakal şekillendirme ustası." }
            );

            // Seed Services for Shop 1
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, ShopId = 1, Name = "Saç Kesimi", Description = "Klasik ve modern erkek saç kesimi.", DurationMinutes = 30, Price = 250, Icon = "fa-scissors" },
                new Service { Id = 2, ShopId = 1, Name = "Sakal Tıraşı", Description = "Profesyonel sakal şekillendirme ve bakım.", DurationMinutes = 20, Price = 150, Icon = "fa-face-smile-beam" },
                new Service { Id = 3, ShopId = 1, Name = "Saç + Sakal Kombo", Description = "Saç kesimi ve sakal tıraşı bir arada.", DurationMinutes = 45, Price = 350, Icon = "fa-star" },
                new Service { Id = 4, ShopId = 1, Name = "Çocuk Kesimi", Description = "12 yaş altı çocuklar için özel saç kesimi.", DurationMinutes = 20, Price = 150, Icon = "fa-child" },
                new Service { Id = 5, ShopId = 1, Name = "Damat Paketi", Description = "Düğün öncesi özel bakım paketi.", DurationMinutes = 90, Price = 750, Icon = "fa-gem" },
                new Service { Id = 6, ShopId = 1, Name = "Saç Boyama", Description = "Profesyonel saç renklendirme.", DurationMinutes = 60, Price = 400, Icon = "fa-palette" }
            );

            // Seed Services for Shop 2
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 7, ShopId = 2, Name = "Premium Kesim", Description = "VIP saç kesimi deneyimi, yıkama ve masaj dahil.", DurationMinutes = 45, Price = 350, Icon = "fa-scissors" },
                new Service { Id = 8, ShopId = 2, Name = "Sakal Tasarımı", Description = "Özel sakal tasarımı ve şekillendirme.", DurationMinutes = 30, Price = 200, Icon = "fa-face-smile-beam" },
                new Service { Id = 9, ShopId = 2, Name = "Komple Bakım", Description = "Saç, sakal ve cilt bakımı paketi.", DurationMinutes = 60, Price = 500, Icon = "fa-star" },
                new Service { Id = 10, ShopId = 2, Name = "Cilt Bakımı", Description = "Yüz temizliği ve nemlendirme maskesi.", DurationMinutes = 30, Price = 200, Icon = "fa-spa" }
            );

            // Seed WorkSchedules for both shops (Mon-Sat 09:00-20:00, Sun closed)
            var scheduleId = 1;
            for (int shopId = 1; shopId <= 2; shopId++)
            {
                for (int day = 0; day <= 6; day++)
                {
                    modelBuilder.Entity<WorkSchedule>().HasData(
                        new WorkSchedule
                        {
                            Id = scheduleId++,
                            ShopId = shopId,
                            DayOfWeek = (DayOfWeek)day,
                            IsClosed = day == 0, // Pazar kapalı
                            OpenTime = "09:00",
                            CloseTime = "20:00"
                        }
                    );
                }
            }
        }
    }
}
