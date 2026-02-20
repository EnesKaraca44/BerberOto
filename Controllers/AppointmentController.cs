using BerberOto.Data;
using BerberOto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BerberOto.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Appointment/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new AppointmentViewModel
            {
                AppointmentDate = DateTime.Today.AddDays(1),
                Barbers = await _context.Barbers
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FullName })
                    .ToListAsync(),
                Services = await _context.Services
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Name} - {s.Price:N0} ₺" })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
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

                return RedirectToAction(nameof(Confirmation), new { id = appointment.Id });
            }

            // Repopulate select lists if validation fails
            model.Barbers = await _context.Barbers
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FullName })
                .ToListAsync();
            model.Services = await _context.Services
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Name} - {s.Price:N0} ₺" })
                .ToListAsync();

            return View(model);
        }

        // GET: Appointment/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // API: Get available time slots
        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int barberId, string date)
        {
            if (!DateTime.TryParse(date, out DateTime selectedDate))
            {
                return BadRequest("Geçersiz tarih formatı.");
            }

            // Working hours: 09:00 - 20:00, 30-minute slots
            var allSlots = new List<string>();
            for (int hour = 9; hour < 20; hour++)
            {
                allSlots.Add($"{hour:D2}:00");
                allSlots.Add($"{hour:D2}:30");
            }

            // Get booked slots for the selected barber and date
            var bookedSlots = await _context.Appointments
                .Where(a => a.BarberId == barberId
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
    }
}
