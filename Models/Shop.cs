using System.ComponentModel.DataAnnotations;

namespace BerberOto.Models
{
    public class Shop
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dükkan adı zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Dükkan Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [StringLength(20)]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [StringLength(300)]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [StringLength(100)]
        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Yetkili adı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Yetkili Adı")]
        public string OwnerName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(100)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = "admin123";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public List<Barber> Barbers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<WorkSchedule> WorkSchedules { get; set; } = new();
    }
}
