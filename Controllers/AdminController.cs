using BerberOto.Data;
using BerberOto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BerberOto.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Shop?> GetShopBySlug(string slug)
        {
            return await _context.Shops.FirstOrDefaultAsync(s => s.Slug == slug);
        }

        private bool IsAuthenticated(string slug)
        {
            return HttpContext.Session.GetString("AdminShop") == slug;
        }

        // GET: /salon/{slug}/admin
        [Route("salon/{slug}/admin")]
        public async Task<IActionResult> Login(string slug)
        {
            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            if (IsAuthenticated(slug))
                return RedirectToAction(nameof(Dashboard), new { slug });

            ViewBag.Shop = shop;
            ViewBag.Error = TempData["LoginError"];
            return View();
        }

        // POST: /salon/{slug}/admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/admin")]
        public async Task<IActionResult> Login(string slug, string password)
        {
            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            if (shop.Password == password)
            {
                HttpContext.Session.SetString("AdminShop", slug);
                return RedirectToAction(nameof(Dashboard), new { slug });
            }

            TempData["LoginError"] = "Şifre hatalı!";
            return RedirectToAction(nameof(Login), new { slug });
        }

        // GET: /salon/{slug}/admin/cikis
        [Route("salon/{slug}/admin/cikis")]
        public IActionResult Logout(string slug)
        {
            HttpContext.Session.Remove("AdminShop");
            return Redirect($"/salon/{slug}/admin");
        }

        // GET: /salon/{slug}/admin/dashboard
        [Route("salon/{slug}/admin/dashboard")]
        public async Task<IActionResult> Dashboard(string slug)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await _context.Shops
                .Include(s => s.Barbers)
                .Include(s => s.Services)
                .Include(s => s.Reviews)
                .FirstOrDefaultAsync(s => s.Slug == slug);
            if (shop == null) return NotFound();

            var today = DateTime.Today;
            var todayAppointments = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.ShopId == shop.Id && a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            var pendingCount = await _context.Appointments
                .CountAsync(a => a.ShopId == shop.Id && a.Status == AppointmentStatus.Pending);

            var totalAppointments = await _context.Appointments
                .CountAsync(a => a.ShopId == shop.Id);

            ViewBag.Shop = shop;
            ViewBag.TodayAppointments = todayAppointments;
            ViewBag.PendingCount = pendingCount;
            ViewBag.TotalAppointments = totalAppointments;
            return View();
        }

        // GET: /salon/{slug}/admin/randevular
        [Route("salon/{slug}/admin/randevular")]
        public async Task<IActionResult> Appointments(string slug)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            var appointments = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.ShopId == shop.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            ViewBag.Shop = shop;
            return View(appointments);
        }

        // POST: /salon/{slug}/admin/randevu-onayla/{id}
        [HttpPost]
        [Route("salon/{slug}/admin/randevu-onayla/{id}")]
        public async Task<IActionResult> ApproveAppointment(string slug, int id)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                await _context.SaveChangesAsync();
            }
            return Redirect($"/salon/{slug}/admin/randevular");
        }

        // POST: /salon/{slug}/admin/randevu-iptal/{id}
        [HttpPost]
        [Route("salon/{slug}/admin/randevu-iptal/{id}")]
        public async Task<IActionResult> CancelAppointment(string slug, int id)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
            return Redirect($"/salon/{slug}/admin/randevular");
        }

        // GET: /salon/{slug}/admin/berberler
        [Route("salon/{slug}/admin/berberler")]
        public async Task<IActionResult> Barbers(string slug)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            var barbers = await _context.Barbers
                .Where(b => b.ShopId == shop.Id)
                .ToListAsync();

            ViewBag.Shop = shop;
            return View(barbers);
        }

        // POST: /salon/{slug}/admin/berber-ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/admin/berber-ekle")]
        public async Task<IActionResult> AddBarber(string slug, Barber model)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            model.ShopId = shop.Id;
            _context.Barbers.Add(model);
            await _context.SaveChangesAsync();

            return Redirect($"/salon/{slug}/admin/berberler");
        }

        // POST: /salon/{slug}/admin/berber-sil/{id}
        [HttpPost]
        [Route("salon/{slug}/admin/berber-sil/{id}")]
        public async Task<IActionResult> DeleteBarber(string slug, int id)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var barber = await _context.Barbers.FindAsync(id);
            if (barber != null)
            {
                _context.Barbers.Remove(barber);
                await _context.SaveChangesAsync();
            }
            return Redirect($"/salon/{slug}/admin/berberler");
        }

        // GET: /salon/{slug}/admin/hizmetler
        [Route("salon/{slug}/admin/hizmetler")]
        public async Task<IActionResult> Services(string slug)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            var services = await _context.Services
                .Where(s => s.ShopId == shop.Id)
                .ToListAsync();

            ViewBag.Shop = shop;
            return View(services);
        }

        // POST: /salon/{slug}/admin/hizmet-ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/admin/hizmet-ekle")]
        public async Task<IActionResult> AddService(string slug, Service model)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            model.ShopId = shop.Id;
            if (string.IsNullOrEmpty(model.Icon)) model.Icon = "fa-scissors";
            _context.Services.Add(model);
            await _context.SaveChangesAsync();

            return Redirect($"/salon/{slug}/admin/hizmetler");
        }

        // POST: /salon/{slug}/admin/hizmet-sil/{id}
        [HttpPost]
        [Route("salon/{slug}/admin/hizmet-sil/{id}")]
        public async Task<IActionResult> DeleteService(string slug, int id)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            return Redirect($"/salon/{slug}/admin/hizmetler");
        }

        // GET: /salon/{slug}/admin/calisma-saatleri
        [Route("salon/{slug}/admin/calisma-saatleri")]
        public async Task<IActionResult> Schedule(string slug)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            var schedules = await _context.WorkSchedules
                .Where(w => w.ShopId == shop.Id)
                .OrderBy(w => w.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)w.DayOfWeek)
                .ToListAsync();

            ViewBag.Shop = shop;
            return View(schedules);
        }

        // POST: /salon/{slug}/admin/calisma-saatleri
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("salon/{slug}/admin/calisma-saatleri")]
        public async Task<IActionResult> UpdateSchedule(string slug, List<int> Ids, List<string> OpenTimes, List<string> CloseTimes, List<int> ClosedDays)
        {
            if (!IsAuthenticated(slug)) return Redirect($"/salon/{slug}/admin");

            var shop = await GetShopBySlug(slug);
            if (shop == null) return NotFound();

            var schedules = await _context.WorkSchedules
                .Where(w => w.ShopId == shop.Id)
                .ToListAsync();

            for (int i = 0; i < Ids.Count; i++)
            {
                var schedule = schedules.FirstOrDefault(s => s.Id == Ids[i]);
                if (schedule != null)
                {
                    schedule.OpenTime = OpenTimes[i];
                    schedule.CloseTime = CloseTimes[i];
                    schedule.IsClosed = ClosedDays.Contains(schedule.Id);
                }
            }

            await _context.SaveChangesAsync();
            return Redirect($"/salon/{slug}/admin/calisma-saatleri");
        }
    }
}
