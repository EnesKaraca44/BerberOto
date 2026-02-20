using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BerberOto.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public decimal Price { get; set; }

        [StringLength(50)]
        public string Icon { get; set; } = "fa-scissors";

        // Shop relationship
        public int ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}
