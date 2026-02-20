namespace BerberOto.Services
{
    /// <summary>
    /// Geliştirme ortamında SMS'leri konsola yazdırır.
    /// Gerçek SMS göndermek için NetgsmSmsService veya TwilioSmsService ile değiştirin.
    /// </summary>
    public class LogSmsService : ISmsService
    {
        private readonly ILogger<LogSmsService> _logger;

        public LogSmsService(ILogger<LogSmsService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("========== SMS GÖNDERİLDİ ==========");
            _logger.LogInformation("Telefon: {Phone}", phoneNumber);
            _logger.LogInformation("Mesaj: {Message}", message);
            _logger.LogInformation("=====================================");

            // Gerçek SMS API'si olmadığı için başarılı kabul ediyoruz
            return Task.FromResult(true);
        }
    }
}
