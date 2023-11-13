using BSWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Mvc;

namespace BSWebApp.Data
{
    public class ResDBContext : IdentityDbContext<AppUser>
    {
        public ResDBContext(DbContextOptions<ResDBContext> options) : base(options)
        {

        }

        public static async Task Initialize(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                // Member
                var member = new AppUser
                {
                    UserName = "member@test.com",
                    Email = "member@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(member, "PasSword");
                await userManager.AddToRoleAsync(member, "Member");
                // Staff
                var staff = new AppUser
                {
                    UserName = "staff@test.com",
                    Email = "staff@test.com",
                    FirstName = "Terry",
                    LastName = "Frog",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(staff, "Pa$$word"); await userManager.AddToRoleAsync(staff, "Staff");
                // Manager
                var manager = new AppUser
                {
                    UserName = "manager@test.com",
                    Email = "manager@test.com",
                    FirstName = "Peter",
                    LastName = "Sauce",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(manager, "Pa$$word");
                await userManager.AddToRoleAsync(manager, "Manager");
            }
        }

        //public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Sitting> Sitting { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Tables> Tables { get; set; }
        
        public DbSet<ReservedTables> ReservedTables { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

    }
}
