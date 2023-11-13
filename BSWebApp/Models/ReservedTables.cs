using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSWebApp.Models
{
    public class ReservedTables
    {
        [Key, Required]
        public int ReservationId { get; set; }
        [Required]
        public int tableId { get; set; }
      
        [ForeignKey("tableId")]
        public virtual Tables? Tables { get; set; }  

        [ForeignKey("ReservationId")]
        public virtual Reservation? Reservation { get; set; }
       
    }
}
