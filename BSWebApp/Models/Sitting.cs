using System.ComponentModel.DataAnnotations;

namespace BSWebApp.Models
{
    public class Sitting
    {
        [Required]
        [Key]
        public int SittingID { get; set; }
        [Required]
        public string SittingType { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
