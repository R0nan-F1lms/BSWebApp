using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSWebApp.Models
{
    public class Reservation
    {

        [Key]
        public int ReservationsId { get; set; }

        public int GuestCount { get; set; }
        // Pending or not
        public string? Status { get; set; }
        public string? Notes { get; set; }

        public DateTime ReservationDateTime { get; set; }

        public int SittingID { get; set; }
        // made from phone? website? email? in person?
        public string? ReservationSource { get; set; }

        public string? UserId { get; set; }


        [Required]
        public int Duration { get; set; }
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [ForeignKey("SittingID")]
        public Sitting? Sitting { get; set; }
        public ICollection<Tables>? Tables { get; set; }

    }
}