using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BerberOto.Models
{
    public class AppointmentViewModel
    {
        public string ShopSlug { get; set; } = string.Empty;

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Berber seçimi zorunludur.")]
        [Display(Name = "Berber")]
        public int BarberId { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        [Display(Name = "Randevu Saati")]
        public string AppointmentTime { get; set; } = string.Empty;

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // Select list items
        public List<SelectListItem>? Barbers { get; set; }
        public List<SelectListItem>? Services { get; set; }
    }
}
