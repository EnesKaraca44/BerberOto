using BerberOto.Data;
using Microsoft.EntityFrameworkCore;

namespace BerberOto.Services
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AppointmentReminderService> _logger;

        public AppointmentReminderService(IServiceScopeFactory scopeFactory, ILogger<AppointmentReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Randevu Hatırlatma Servisi Başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

                        var now = DateTime.Now;
                        var today = now.Date;

                        // Bugünün onaylanmış ve hatırlatılmamış randevularını getir
                        var appointments = await context.Appointments
                            .Include(a => a.Shop)
                            .Include(a => a.Barber)
                            .Include(a => a.Service)
                            .Where(a => 
                                a.Status == Models.AppointmentStatus.Confirmed &&
                                !a.ReminderSent &&
                                a.AppointmentDate.Date == today
                            )
                            .ToListAsync(stoppingToken);

                        foreach (var appt in appointments)
                        {
                            // Saat bilgisini parse et (Örn: "14:30")
                            if (TimeSpan.TryParse(appt.AppointmentTime, out var time))
                            {
                                var apptDateTime = appt.AppointmentDate.Date.Add(time);
                                var timeUntil = apptDateTime - now;

                                // Randevuya 45-75 dakika arası kaldıysa hatırlat (1 saat hedefi + buffer)
                                if (timeUntil.TotalMinutes >= 45 && timeUntil.TotalMinutes <= 75)
                                {
                                    var message = $"HATIRLATMA: {appt.Shop.Name} randevunuza 1 saat kaldı!\n" +
                                                $"Saat: {appt.AppointmentTime}\n" +
                                                $"Berber: {appt.Barber.FullName}\n" +
                                                $"Hizmet: {appt.Service.Name}";

                                    await smsService.SendSmsAsync(appt.CustomerPhone, message);
                                    
                                    appt.ReminderSent = true;
                                    _logger.LogInformation("Hatırlatma gönderildi: {Id} - {Phone}", appt.Id, appt.CustomerPhone);
                                }
                            }
                        }

                        if (context.ChangeTracker.HasChanges())
                        {
                            await context.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Randevu hatırlatma servisinde hata oluştu.");
                }

                // 2 dakikada bir kontrol et
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
    }
}
