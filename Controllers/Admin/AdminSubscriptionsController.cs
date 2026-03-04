using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Collections.Generic;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Route("SubscriptionPlans")]
    public class AdminSubscriptionsController : Controller
    {
        private static List<SubscriptionPlan> _plans = new List<SubscriptionPlan>
        {
            new SubscriptionPlan { Id = 1, Name = "Basic", Price = 9.99m, DurationDays = 30 },
            new SubscriptionPlan { Id = 2, Name = "Standard", Price = 19.99m, DurationDays = 90 },
            new SubscriptionPlan { Id = 3, Name = "Premium", Price = 49.99m, DurationDays = 365 }
        };

        private readonly string viewFolder = "~/Views/Admin/Subscriptions/";

        // GET: /SubscriptionPlans
        [HttpGet("")]
        public IActionResult Index()
        {
            return View(viewFolder + "Index.cshtml", _plans);
        }

        // GET: /SubscriptionPlans/Details/5
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == id);
            if (plan == null) return NotFound();
            return View(viewFolder + "Details.cshtml", plan);
        }

        // GET: /SubscriptionPlans/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(viewFolder + "Create.cshtml");
        }

        // POST: /SubscriptionPlans/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(SubscriptionPlan plan)
        {
            if (ModelState.IsValid)
            {
                plan.Id = _plans.Any() ? _plans.Max(p => p.Id) + 1 : 1;
                _plans.Add(plan);
                return RedirectToAction(nameof(Index));
            }
            return View(viewFolder + "Create.cshtml", plan);
        }

        // GET: /SubscriptionPlans/Edit/5
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == id);
            if (plan == null) return NotFound();
            return View(viewFolder + "Edit.cshtml", plan);
        }

        // POST: /SubscriptionPlans/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id, SubscriptionPlan plan)
        {
            if (id != plan.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = _plans.FirstOrDefault(p => p.Id == id);
                if (existing == null) return NotFound();

                existing.Name = plan.Name;
                existing.Price = plan.Price;
                existing.DurationDays = plan.DurationDays;
                existing.Status = plan.Status;

                return RedirectToAction(nameof(Index));
            }
            return View(viewFolder + "Edit.cshtml", plan);
        }

        // GET: /SubscriptionPlans/Delete/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == id);
            if (plan == null) return NotFound();
            return View(viewFolder + "Delete.cshtml", plan);
        }

        // POST: /SubscriptionPlans/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == id);
            if (plan != null) _plans.Remove(plan);

            return RedirectToAction(nameof(Index));
        }
    }
}