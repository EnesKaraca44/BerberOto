using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BerberOto.Models
{
    public class Barber
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        // Shop relationship
        public int ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}
