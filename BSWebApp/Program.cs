using BSWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BSWebApp.Models;
using BSWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// BUILDER SERVICE CONNECTING DB CONTEXT
builder.Services.AddDbContext<ResDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ResDBConnection") ?? throw new InvalidOperationException("Connection string 'RestaurantContext' not found"))
);



builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<ResDBContext>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<FileUploadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Check if roles exist, and create them if they don't
    foreach (var roleName in new[] { "Guest", "Member", "Staff", "Admin" })
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
        }
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var dis = app.Services.CreateScope();
var userManager = dis.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
try
{
    await ResDBContext.Initialize(userManager);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();
