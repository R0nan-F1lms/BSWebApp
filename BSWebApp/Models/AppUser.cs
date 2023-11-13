using Microsoft.AspNetCore.Identity;

namespace BSWebApp.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string? ProfilePicturePath { get; set; }

    }
}
