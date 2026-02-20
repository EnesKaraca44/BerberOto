using BerberOto.Data;
using BerberOto.Models;
using BerberOto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BerberOto.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISmsService _smsService;

        public ShopController(AppDbContext context, ISmsService smsService)
        {
            _context = context;
            _smsService = smsService;
        }

        // GET: /salon/{slug}
        [Route("salon/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var shop = await _context.Shops
                .Include(s => s.Barbers)
                .Include(s => s.Services)
                .Include(s => s.Reviews.OrderByDescending(r => r.CreatedAt))
                .Include(s => s.WorkSchedules)
                .FirstOrDefaultAsync(s => s.Slug == slug);

            if (shop == null) return NotFound();

            return View(shop);
        }

        // GET: /salon/{slug}/qr
        [Route("salon/{slug}/qr")]
        public async Task<IActionResult> QRCode(string slug)
        {
            var shop = await _context.Shops
                .Include(s => s.Services)
                .FirstOrDefaultAsync(s => s.Slug == slug);

            if (shop == null) return NotFound();

            return View(shop);
        }

        // POST: /salon/{slug}/yorum
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/yorum")]
        public async Task<IActionResult> AddReview(string slug, Review model)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            if (ModelState.IsValid)
            {
                model.ShopId = shop.Id;
                model.CreatedAt = DateTime.Now;
                _context.Reviews.Add(model);
                await _context.SaveChangesAsync();

                return Redirect($"/salon/{slug}#yorumlar");
            }

            return Redirect($"/salon/{slug}#yorumlar");
        }

        // GET: /salon/kayit
        [Route("salon/kayit")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /salon/kayit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/kayit")]
        public async Task<IActionResult> Register(Shop model)
        {
            // Generate slug from name
            model.Slug = GenerateSlug(model.Name);

            // Check if slug already exists
            if (await _context.Shops.AnyAsync(s => s.Slug == model.Slug))
            {
                ModelState.AddModelError("Name", "Bu isimle bir dükkan zaten mevcut. Farklı bir isim deneyin.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                _context.Shops.Add(model);
                await _context.SaveChangesAsync();

                // Add default services for the new shop
                var defaultServices = new List<Service>
                {
                    new Service { ShopId = model.Id, Name = "Saç Kesimi", Description = "Klasik ve modern erkek saç kesimi.", DurationMinutes = 30, Price = 200, Icon = "fa-scissors" },
                    new Service { ShopId = model.Id, Name = "Sakal Tıraşı", Description = "Profesyonel sakal şekillendirme.", DurationMinutes = 20, Price = 100, Icon = "fa-face-smile-beam" },
                    new Service { ShopId = model.Id, Name = "Saç + Sakal Kombo", Description = "Saç kesimi ve sakal tıraşı bir arada.", DurationMinutes = 45, Price = 280, Icon = "fa-star" }
                };

                _context.Services.AddRange(defaultServices);

                // Add the owner as the first barber
                var ownerBarber = new Barber
                {
                    ShopId = model.Id,
                    FullName = model.OwnerName,
                    Title = "Kurucu & Baş Berber",
                    Bio = $"{model.Name} kurucusu ve baş berberi."
                };
                _context.Barbers.Add(ownerBarber);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Detail), new { slug = model.Slug });
            }

            return View(model);
        }

        // GET: /salon/{slug}/randevu
        [Route("salon/{slug}/randevu")]
        public async Task<IActionResult> CreateAppointment(string slug)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            var viewModel = new AppointmentViewModel
            {
                ShopSlug = slug,
                AppointmentDate = DateTime.Today.AddDays(1),
                Barbers = await _context.Barbers
                    .Where(b => b.ShopId == shop.Id)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FullName })
                    .ToListAsync(),
                Services = await _context.Services
                    .Where(s => s.ShopId == shop.Id)
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Name} - {s.Price:N0} ₺" })
                    .ToListAsync()
            };

            ViewBag.Shop = shop;
            return View(viewModel);
        }

        // POST: /salon/{slug}/randevu
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/randevu")]
        public async Task<IActionResult> CreateAppointment(string slug, AppointmentViewModel model)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    ShopId = shop.Id,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    BarberId = model.BarberId,
                    ServiceId = model.ServiceId,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    Notes = model.Notes,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // SMS gönder
                var service = await _context.Services.FindAsync(model.ServiceId);
                var barber = await _context.Barbers.FindAsync(model.BarberId);
                var smsMessage = $"{shop.Name} - Randevunuz alındı!\n" +
                    $"Tarih: {model.AppointmentDate:dd.MM.yyyy}\n" +
                    $"Saat: {model.AppointmentTime}\n" +
                    $"Berber: {barber?.FullName}\n" +
                    $"Hizmet: {service?.Name}\n" +
                    $"İyi günler dileriz!";
                await _smsService.SendSmsAsync(model.CustomerPhone, smsMessage);

                return RedirectToAction(nameof(Confirmation), new { slug, id = appointment.Id });
            }

            // Repopulate
            model.ShopSlug = slug;
            model.Barbers = await _context.Barbers
                .Where(b => b.ShopId == shop.Id)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FullName })
                .ToListAsync();
            model.Services = await _context.Services
                .Where(s => s.ShopId == shop.Id)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Name} - {s.Price:N0} ₺" })
                .ToListAsync();

            ViewBag.Shop = shop;
            return View(model);
        }

        // GET: /salon/{slug}/onay/{id}
        [Route("salon/{slug}/onay/{id}")]
        public async Task<IActionResult> Confirmation(string slug, int id)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Include(a => a.Shop)
                .FirstOrDefaultAsync(a => a.Id == id && a.ShopId == shop.Id);

            if (appointment == null) return NotFound();

            ViewBag.ShopSlug = slug;
            return View(appointment);
        }

        // API: /salon/{slug}/GetAvailableSlots
        [HttpGet]
        [Route("salon/{slug}/GetAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(string slug, int barberId, string date)
        {
            if (!DateTime.TryParse(date, out DateTime selectedDate))
            {
                return BadRequest("Geçersiz tarih formatı.");
            }

            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            // Çalışma saatlerini kontrol et
            var schedule = await _context.WorkSchedules
                .FirstOrDefaultAsync(w => w.ShopId == shop.Id && w.DayOfWeek == selectedDate.DayOfWeek);

            if (schedule == null || schedule.IsClosed)
            {
                return Json(new List<object>()); // Kapalı gün — boş slot
            }

            // Açılış/kapanış saatlerinden slot üret
            var openParts = schedule.OpenTime.Split(':');
            var closeParts = schedule.CloseTime.Split(':');
            int openHour = int.Parse(openParts[0]);
            int openMin = int.Parse(openParts[1]);
            int closeHour = int.Parse(closeParts[0]);

            var allSlots = new List<string>();
            for (int hour = openHour; hour < closeHour; hour++)
            {
                if (hour == openHour && openMin > 0)
                {
                    allSlots.Add($"{hour:D2}:30");
                }
                else
                {
                    allSlots.Add($"{hour:D2}:00");
                    allSlots.Add($"{hour:D2}:30");
                }
            }

            var bookedSlots = await _context.Appointments
                .Where(a => a.ShopId == shop.Id
                         && a.BarberId == barberId
                         && a.AppointmentDate.Date == selectedDate.Date
                         && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            var availableSlots = allSlots
                .Where(s => !bookedSlots.Contains(s))
                .Select(s => new { time = s, display = s })
                .ToList();

            return Json(availableSlots);
        }

        private string GenerateSlug(string name)
        {
            var slug = name.ToLowerInvariant()
                .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                .Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u")
                .Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");

            // Replace non-alphanumeric with dashes
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }
    }
}
