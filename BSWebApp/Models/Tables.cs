using System.ComponentModel.DataAnnotations;

namespace BSWebApp.Models
{
    public class Tables
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string TableID { get; set; }
        [Required]
        public int AreaID { get; set; }

        // Nav Properties - Relation ships
        public Area Area { get; set; }
        public ICollection<ReservedTables> ReservedTables { get; set; }
    }
}
