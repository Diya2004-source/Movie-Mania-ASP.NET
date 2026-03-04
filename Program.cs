using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Add Services
// --------------------

// Add MVC
builder.Services.AddControllersWithViews();

// Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --------------------
// Middleware Pipeline
// --------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// VERY IMPORTANT ORDER
app.UseAuthentication();
app.UseAuthorization();

// Default Route → Guest
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GuestHome}/{action=Index}/{id?}");

app.Run();