using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Wishlist")]
    public class AdminWishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _viewPath = "~/Views/Admin/Wishlist/";

        public AdminWishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            return View(_viewPath + "Index.cshtml", wishlist);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);

            if (item == null)
            {
                TempData["Error"] = $"Wishlist item with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Details.cshtml", item);
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> UserWishlist(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {userId} not found.";
                return RedirectToAction(nameof(Index));
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            ViewBag.UserName = user.Name;
            ViewBag.UserId = userId;
            return View(_viewPath + "UserWishlist.cshtml", wishlist);
        }

        [HttpGet("Movies")]
        public async Task<IActionResult> Movies()
        {
            var movies = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Movie)
                .Where(w => w.ItemType == "Movie" && w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            ViewBag.Type = "Movies";
            return View(_viewPath + "Index.cshtml", movies);
        }

        [HttpGet("Shows")]
        public async Task<IActionResult> Shows()
        {
            var shows = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Show)
                .Where(w => w.ItemType == "Show" && w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            ViewBag.Type = "Shows";
            return View(_viewPath + "Index.cshtml", shows);
        }
    }
}