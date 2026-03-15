using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "MovieMania.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "MovieMania.Session";
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Guest Routes (public)
app.MapControllerRoute(
    name: "guest",
    pattern: "{controller=GuestHome}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/{action=Index}/{id?}",
    defaults: new { controller = "GuestMovies", action = "Index" });

app.MapControllerRoute(
    name: "shows",
    pattern: "Shows/{action=Index}/{id?}",
    defaults: new { controller = "GuestShows", action = "Index" });

app.MapControllerRoute(
    name: "anime",
    pattern: "Anime/{action=Index}/{id?}",
    defaults: new { controller = "GuestAnime", action = "Index" });

app.MapControllerRoute(
    name: "auth",
    pattern: "Auth/{action=Login}/{id?}",
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "payment",
    pattern: "Payment/{action=Plans}/{id?}",
    defaults: new { controller = "Payment", action = "Plans" });

// User Routes (authenticated)
app.MapControllerRoute(
    name: "user",
    pattern: "User/{controller=Home}/{action=Index}/{id?}");

// Admin Routes
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "adminMovies",
    pattern: "AdminMovies/{action=Index}/{id?}",
    defaults: new { controller = "AdminMovies", action = "Index" });

app.MapControllerRoute(
    name: "adminShows",
    pattern: "AdminShows/{action=Index}/{id?}",
    defaults: new { controller = "AdminShows", action = "Index" });

app.Run();