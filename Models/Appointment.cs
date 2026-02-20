using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BerberOto.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [StringLength(20)]
        [Display(Name = "Telefon")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Berber seçimi zorunludur.")]
        [Display(Name = "Berber")]
        public int BarberId { get; set; }

        [ForeignKey("BarberId")]
        public Barber? Barber { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        // Shop relationship
        public int ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        [Display(Name = "Randevu Saati")]
        public string AppointmentTime { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public bool ReminderSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum AppointmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }
}
