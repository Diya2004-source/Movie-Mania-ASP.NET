using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Linq;

namespace MovieMania.Controllers
{
    [Route("Users")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Users
        [HttpGet("")]
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View("/Views/Admin/Users/Index.cshtml", users);
        }

        // GET: /Users/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("/Views/Admin/Users/Create.cshtml");
        }

        // POST: /Users/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("/Views/Admin/Users/Create.cshtml", user);
        }

        // GET: /Users/Edit/5
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View("/Views/Admin/Users/Edit.cshtml", user);
        }

        // POST: /Users/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("/Views/Admin/Users/Edit.cshtml", user);
        }

        // GET: /Users/Delete/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View("/Views/Admin/Users/Delete.cshtml", user);
        }

        // POST: /Users/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}