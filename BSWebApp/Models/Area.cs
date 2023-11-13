using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace BSWebApp.Models
{
    public class Area
    {
        public Area()
        {
            Tables = new HashSet<Tables>();
        }

        [Key]
        public int AreaId { get; set; }
        // eg: outside 1, balcony 1, inside 1
        public Name AreaName { get; set; }
        public string AreaDescription { get; set; }

        public ICollection<Tables> Tables { get; set; }
    }
    public enum Name
    {
        Main,
        Outside,
        Balcony
    }
}

