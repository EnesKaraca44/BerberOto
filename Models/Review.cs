using System.ComponentModel.DataAnnotations;

namespace BerberOto.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int ShopId { get; set; }
        public Shop? Shop { get; set; }

        [Required(ErrorMessage = "Adınızı giriniz.")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen puan verin.")]
        [Range(1, 5, ErrorMessage = "1-5 arası puan verin.")]
        [Display(Name = "Puan")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Yorum yazınız.")]
        [StringLength(500)]
        [Display(Name = "Yorum")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
