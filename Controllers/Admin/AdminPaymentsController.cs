using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Payments")]
    public class AdminPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewFolder = "~/Views/Admin/Payments/";

        public AdminPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Payments
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.SubscriptionPlan)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(viewFolder + "Index.cshtml", payments);
        }

        // GET: Admin/Payments/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.SubscriptionPlan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                TempData["Error"] = $"❌ Payment with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Details.cshtml", payment);
        }

        // GET: Admin/Payments/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.SubscriptionPlan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                TempData["Error"] = $"❌ Payment with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Delete.cshtml", payment);
        }

        // POST: Admin/Payments/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);

                if (payment == null)
                {
                    TempData["Error"] = $"❌ Payment with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Payments.Remove(payment);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Payment #{id} deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Payment was not deleted.";
                }
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting payment: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}