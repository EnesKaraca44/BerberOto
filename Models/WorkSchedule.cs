using System.ComponentModel.DataAnnotations;

namespace BerberOto.Models
{
    public class WorkSchedule
    {
        public int Id { get; set; }

        public int ShopId { get; set; }
        public Shop? Shop { get; set; }

        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        [Display(Name = "Kapalı mı?")]
        public bool IsClosed { get; set; } = false;

        [StringLength(5)]
        [Display(Name = "Açılış")]
        public string OpenTime { get; set; } = "09:00";

        [StringLength(5)]
        [Display(Name = "Kapanış")]
        public string CloseTime { get; set; } = "20:00";
    }
}
