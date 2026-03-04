using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MovieMania.Controllers.Admin
{
    [Route("Payments")]
    public class AdminPaymentsController : Controller
    {
        // Fake in-memory database for demonstration
        private static List<Payment> _payments = new List<Payment>
        {
            new Payment { Id = 1, UserId = 1, User = new User { Id = 1, Name = "John Doe" }, Amount = 49.99m, PaymentMethod = "Credit Card", PaymentDate = DateTime.Now.AddDays(-5) },
            new Payment { Id = 2, UserId = 2, User = new User { Id = 2, Name = "Jane Smith" }, Amount = 19.99m, PaymentMethod = "PayPal", PaymentDate = DateTime.Now.AddDays(-2) },
            new Payment { Id = 3, UserId = 1, User = new User { Id = 1, Name = "John Doe" }, Amount = 9.99m, PaymentMethod = "Credit Card", PaymentDate = DateTime.Now }
        };

        private readonly string viewFolder = "~/Views/Admin/Payments/";

        // GET: /Payments
        [HttpGet("")]
        public IActionResult Index()
        {
            return View(viewFolder + "Index.cshtml", _payments.OrderByDescending(p => p.PaymentDate).ToList());
        }

        // GET: /Payments/Details/5
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);
            if (payment == null) return NotFound();
            return View(viewFolder + "Details.cshtml", payment);
        }

        // GET: /Payments/Delete/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);
            if (payment == null) return NotFound();
            return View(viewFolder + "Delete.cshtml", payment);
        }

        // POST: /Payments/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);
            if (payment != null)
            {
                _payments.Remove(payment);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}