using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Users")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _viewPath = "~/Views/Admin/Users/";

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(_viewPath + "Index.cshtml", users);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(_viewPath + "Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Create.cshtml", user);
                }

                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.ReferralCode = "REF" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                await _context.Users.AddAsync(user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"User '{user.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "User was not created.";
                return View(_viewPath + "Create.cshtml", user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Create.cshtml", user);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Edit.cshtml", user);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppUser user)
        {
            if (id != user.Id)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Edit.cshtml", user);
                }

                var existingUser = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (existingUser == null)
                {
                    TempData["Error"] = $"User with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                user.CreatedAt = existingUser.CreatedAt;
                user.Password = existingUser.Password; // Preserve password

                _context.Update(user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"User '{user.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No changes were saved.";
                return View(_viewPath + "Edit.cshtml", user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Edit.cshtml", user);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Delete.cshtml", user);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["Error"] = $"User with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Users.Remove(user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    TempData["Success"] = $"User '{user.Name}' deleted successfully!";
                else
                    TempData["Error"] = "User was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .Include(u => u.Payments)
                .Include(u => u.Wishlists)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Details.cshtml", user);
        }

        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                user.IsActive = !user.IsActive;
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var status = user.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"User '{user.Name}' {status} successfully!",
                        isActive = user.IsActive
                    });
                }

                return Json(new { success = false, message = "No changes were saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}