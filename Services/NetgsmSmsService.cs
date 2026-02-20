using System.Net.Http;
using System.Web;

namespace BerberOto.Services
{
    /// <summary>
    /// Netgsm SMS API entegrasyonu.
    /// appsettings.json'dan ayarları okur.
    /// Kullanmak için Program.cs'de LogSmsService yerine bu servisi kaydedin.
    /// </summary>
    public class NetgsmSmsService : ISmsService
    {
        private readonly ILogger<NetgsmSmsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public NetgsmSmsService(ILogger<NetgsmSmsService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var usercode = _configuration["Sms:Netgsm:UserCode"];
                var password = _configuration["Sms:Netgsm:Password"];
                var msgheader = _configuration["Sms:Netgsm:MsgHeader"];

                // Telefon numarasını düzenle (başındaki + ve boşlukları kaldır)
                var phone = phoneNumber.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

                var url = $"https://api.netgsm.com.tr/sms/send/get?" +
                    $"usercode={HttpUtility.UrlEncode(usercode)}" +
                    $"&password={HttpUtility.UrlEncode(password)}" +
                    $"&gsmno={phone}" +
                    $"&message={HttpUtility.UrlEncode(message)}" +
                    $"&msgheader={HttpUtility.UrlEncode(msgheader)}";

                var response = await _httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Netgsm SMS sonucu: {Result} -> {Phone}", result, phone);

                // Netgsm başarılı kodları: 00, 01, 02
                return result.StartsWith("00") || result.StartsWith("01") || result.StartsWith("02");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMS gönderme hatası: {Phone}", phoneNumber);
                return false;
            }
        }
    }
}
