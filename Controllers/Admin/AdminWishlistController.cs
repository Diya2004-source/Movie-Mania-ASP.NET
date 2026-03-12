using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Wishlist")]
    public class AdminWishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewFolder = "~/Views/Admin/Wishlist/";

        public AdminWishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Wishlist
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

            return View(viewFolder + "Index.cshtml", wishlist);
        }

        // GET: Admin/Wishlist/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var wishlistItem = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);

            if (wishlistItem == null)
            {
                TempData["Error"] = $"❌ Wishlist item with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Details.cshtml", wishlistItem);
        }

        // GET: Admin/Wishlist/User/5
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> UserWishlist(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = $"❌ User with ID {userId} not found.";
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

            return View(viewFolder + "UserWishlist.cshtml", wishlist);
        }

        // GET: Admin/Wishlist/Movies
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
            return View(viewFolder + "Index.cshtml", movies);
        }

        // GET: Admin/Wishlist/Shows
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
            return View(viewFolder + "Index.cshtml", shows);
        }
    }
}