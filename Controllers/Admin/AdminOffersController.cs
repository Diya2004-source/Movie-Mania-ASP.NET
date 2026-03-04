using Microsoft.AspNetCore.Mvc;
using movie.Models;
using MovieMania.Models;
using System.Linq;

namespace MovieMania.Controllers
{
    [Route("Offers")]
    public class AdminOffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: /Offers
        // =========================
        [HttpGet("")]
        public IActionResult Index()
        {
            var offers = _context.Offers.ToList();
            return View("/Views/Admin/Offers/Index.cshtml", offers);
        }

        // =========================
        // GET: /Offers/Create
        // =========================
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("/Views/Admin/Offers/Create.cshtml");
        }

        // =========================
        // POST: /Offers/Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Offer offer)
        {
            if (ModelState.IsValid)
            {
                _context.Offers.Add(offer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("/Views/Admin/Offers/Create.cshtml", offer);
        }

        // =========================
        // GET: /Offers/Edit/5
        // =========================
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var offer = _context.Offers.Find(id);
            if (offer == null) return NotFound();

            return View("/Views/Admin/Offers/Edit.cshtml", offer);
        }

        // =========================
        // POST: /Offers/Edit/5
        // =========================
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Offer offer)
        {
            if (id != offer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Offers.Update(offer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("/Views/Admin/Offers/Edit.cshtml", offer);
        }

        // =========================
        // GET: /Offers/Delete/5
        // =========================
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var offer = _context.Offers.Find(id);
            if (offer == null) return NotFound();

            return View("/Views/Admin/Offers/Delete.cshtml", offer);
        }

        // =========================
        // POST: /Offers/Delete/5
        // =========================
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var offer = _context.Offers.Find(id);

            if (offer != null)
            {
                _context.Offers.Remove(offer);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}