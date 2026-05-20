//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Security.Claims;
//using MovieMania.Models; // Ensure this matches your namespace

//namespace MovieMania.Controllers
//{
//    public class AuthController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AuthController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Register() => View("~/Views/Guest/Auth/Register.cshtml");

//        [HttpPost]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (!ModelState.IsValid) return View("~/Views/Guest/Auth/Register.cshtml", model);

//            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
//            {
//                TempData["Error"] = "Email already exists.";
//                return View("~/Views/Guest/Auth/Register.cshtml", model);
//            }

//            var user = new AppUser
//            {
//                Name = model.Name,
//                Email = model.Email,
//                Password = model.Password,
//                Role = "user",
//                IsActive = false,
//                IsPaymentDone = false,
//                PaymentStatus = "Pending",
//                CreatedAt = DateTime.Now,
//                ReferralCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            return RedirectToAction("PaymentQRCode", new { userId = user.Id });
//        }

//        [HttpGet]
//        public async Task<IActionResult> PaymentQRCode(int userId)
//        {
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null) return RedirectToAction("Register");
//            if (user.IsPaymentDone) return RedirectToAction("Login");

//            ViewBag.UserId = user.Id;
//            ViewBag.Name = user.Name;
//            ViewBag.Email = user.Email;
//            ViewBag.Plans = await _context.SubscriptionPlans.Where(p => p.IsActive).ToListAsync();

//            return View("~/Views/Guest/Auth/PaymentQRCode.cshtml");
//        }

//        // FIXED: Parameter name now matches the internal variable usage
//        [HttpPost]
//        public async Task<IActionResult> CaptureRazorpayPayment(int userId, int subscriptionPlanId, string razorpayPaymentId)
//        {
//            try
//            {
//                var user = await _context.Users.FindAsync(userId);
//                var plan = await _context.SubscriptionPlans.FindAsync(subscriptionPlanId);

//                if (user == null || plan == null)
//                    return Json(new { success = false, message = "Invalid User or Plan" });

//                user.IsActive = true;
//                user.IsPaymentDone = true;
//                user.PaymentStatus = "Completed";

//                var payment = new Payment
//                {
//                    UserId = user.Id,
//                    SubscriptionPlanId = plan.Id,
//                    Amount = plan.Price,
//                    PaymentMethod = "Razorpay",
//                    Status = "Completed",
//                    TransactionId = razorpayPaymentId,
//                    PaymentDate = DateTime.Now,
//                    CreatedAt = DateTime.Now
//                };

//                _context.Payments.Add(payment);
//                _context.Users.Update(user);
//                await _context.SaveChangesAsync();

//                return Json(new { success = true });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public IActionResult Login() => View("~/Views/Guest/Auth/Login.cshtml");

//        [HttpPost]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

//            if (user == null)
//            {
//                TempData["Error"] = "Invalid credentials.";
//                return View("~/Views/Guest/Auth/Login.cshtml", model);
//            }

//            if (!user.IsPaymentDone)
//            {
//                return RedirectToAction("PaymentQRCode", new { userId = user.Id });
//            }

//            var claims = new List<Claim> {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.Name),
//                new Claim(ClaimTypes.Role, user.Role)
//            };

//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
//                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

//            return RedirectToAction("Index", "Home", new { area = "User" });
//        }
//    }
//}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Security.Claims;
//using MovieMania.Models;
//using Razorpay.Api;

//namespace MovieMania.Controllers
//{
//    public class AuthController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AuthController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Register() => View("~/Views/Guest/Auth/Register.cshtml");

//        [HttpPost]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (!ModelState.IsValid) return View("~/Views/Guest/Auth/Register.cshtml", model);

//            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
//            {
//                TempData["Error"] = "Email already exists.";
//                return View("~/Views/Guest/Auth/Register.cshtml", model);
//            }

//            var user = new AppUser
//            {
//                Name = model.Name,
//                Email = model.Email,
//                Password = model.Password,
//                Role = "user",
//                IsActive = false,
//                IsPaymentDone = false,
//                PaymentStatus = "Pending",
//                CreatedAt = DateTime.Now,
//                ReferralCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            return RedirectToAction("PaymentQRCode", new { userId = user.Id });
//        }

//        [HttpGet]
//        public async Task<IActionResult> PaymentQRCode(int userId)
//        {
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null) return RedirectToAction("Register");
//            if (user.IsPaymentDone) return RedirectToAction("Login");

//            ViewBag.UserId = user.Id;
//            ViewBag.Name = user.Name;
//            ViewBag.Email = user.Email;
//            ViewBag.Plans = await _context.SubscriptionPlans.Where(p => p.IsActive).ToListAsync();

//            return View("~/Views/Guest/Auth/PaymentQRCode.cshtml");
//        }

//        // ✅ CREATE ORDER
//        [HttpPost]
//        public IActionResult CreateOrder(int amount)
//        {
//            try
//            {
//                string key = "rzp_test_Skm5zcDuIhucIg";
//                string secret = "cv8sbNU4UxiMaaCvceUsBzXs";   
//                RazorpayClient client = new RazorpayClient(key, secret);

//                var options = new Dictionary<string, object>();
//                options.Add("amount", amount * 100);
//                options.Add("currency", "INR");
//                options.Add("receipt", "order_" + DateTime.Now.Ticks);

//                Order order = client.Order.Create(options);

//                return Json(new
//                {
//                    id = order["id"].ToString(),
//                    amount = order["amount"]
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // ✅ PAYMENT CAPTURE
//        [HttpPost]
//        public async Task<IActionResult> CaptureRazorpayPayment(
//            int userId,
//            int subscriptionPlanId,
//            string razorpay_payment_id,
//            string razorpay_order_id,
//            string razorpay_signature)
//        {
//            try
//            {
//                var user = await _context.Users.FindAsync(userId);
//                var plan = await _context.SubscriptionPlans.FindAsync(subscriptionPlanId);

//                if (user == null || plan == null)
//                    return Json(new { success = false, message = "Invalid User or Plan" });

//                user.IsActive = true;
//                user.IsPaymentDone = true;
//                user.PaymentStatus = "Completed";

//                var payment = new MovieMania.Models.Payment
//                {
//                    UserId = user.Id,
//                    SubscriptionPlanId = plan.Id,
//                    Amount = plan.Price,
//                    PaymentMethod = "Razorpay",
//                    Status = "Completed",
//                    TransactionId = razorpay_payment_id,
//                    PaymentDate = DateTime.Now,
//                    CreatedAt = DateTime.Now
//                };

//                _context.Payments.Add(payment);
//                _context.Users.Update(user);
//                await _context.SaveChangesAsync();

//                return Json(new { success = true });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public IActionResult Login() => View("~/Views/Guest/Auth/Login.cshtml");

//        [HttpPost]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            var user = await _context.Users
//                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

//            if (user == null)
//            {
//                TempData["Error"] = "Invalid credentials.";
//                return View("~/Views/Guest/Auth/Login.cshtml", model);
//            }

//            if (!user.IsPaymentDone)
//            {
//                return RedirectToAction("PaymentQRCode", new { userId = user.Id });
//            }

//            var claims = new List<Claim> {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.Name),
//                new Claim(ClaimTypes.Role, user.Role)
//            };

//            await HttpContext.SignInAsync(
//                CookieAuthenticationDefaults.AuthenticationScheme,
//                new ClaimsPrincipal(new ClaimsIdentity(claims,
//                CookieAuthenticationDefaults.AuthenticationScheme)));

//            return RedirectToAction("Index", "Home", new { area = "User" });
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MovieMania.Models;
using Razorpay.Api;

namespace MovieMania.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // REGISTER
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Guest/Auth/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("~/Views/Guest/Auth/Register.cshtml", model);
                }

                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email);

                if (emailExists)
                {
                    TempData["Error"] = "Email already exists.";

                    return View("~/Views/Guest/Auth/Register.cshtml", model);
                }

                var user = new AppUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "user",
                    IsActive = false,
                    IsPaymentDone = false,
                    PaymentStatus = "Pending",
                    CreatedAt = DateTime.Now,
                    ReferralCode = Guid.NewGuid()
                        .ToString()
                        .Substring(0, 8)
                        .ToUpper()
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                TempData["success"] = "Registration successful";

                return RedirectToAction("PaymentQRCode",
                    new { userId = user.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View("~/Views/Guest/Auth/Register.cshtml", model);
            }
        }

        // =========================
        // PAYMENT QR PAGE
        // =========================

        [HttpGet]
        public async Task<IActionResult> PaymentQRCode(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Register");
            }

            if (user.IsPaymentDone)
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserId = user.Id;
            ViewBag.Name = user.Name;
            ViewBag.Email = user.Email;

            ViewBag.Plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();

            return View("~/Views/Guest/Auth/PaymentQRCode.cshtml");
        }

        // =========================
        // CREATE RAZORPAY ORDER
        // =========================

        [HttpPost]
        public IActionResult CreateOrder(int amount)
        {
            try
            {
                string key = "rzp_test_Skm5zcDuIhucIg";
                string secret = "cv8sbNU4UxiMaaCvceUsBzXs";

                RazorpayClient client = new RazorpayClient(key, secret);

                Dictionary<string, object> options = new();

                options.Add("amount", amount * 100);
                options.Add("currency", "INR");
                options.Add("receipt", "receipt_" + DateTime.Now.Ticks);

                Order order = client.Order.Create(options);

                return Json(new
                {
                    success = true,
                    orderId = order["id"].ToString(),
                    amount = order["amount"].ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // =========================
        // PAYMENT SUCCESS
        // =========================

        [HttpPost]
        public async Task<IActionResult> CaptureRazorpayPayment(
            int userId,
            int subscriptionPlanId,
            string razorpay_payment_id,
            string razorpay_order_id,
            string razorpay_signature)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                var plan = await _context.SubscriptionPlans
                    .FindAsync(subscriptionPlanId);

                if (user == null || plan == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid user or plan"
                    });
                }

                user.IsActive = true;
                user.IsPaymentDone = true;
                user.PaymentStatus = "Completed";

                var payment = new MovieMania.Models.Payment
                {
                    UserId = user.Id,
                    SubscriptionPlanId = plan.Id,
                    Amount = plan.Price,
                    PaymentMethod = "Razorpay",
                    Status = "Completed",
                    TransactionId = razorpay_payment_id,
                    PaymentDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);

                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // =========================
        // LOGIN
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Guest/Auth/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("~/Views/Guest/Auth/Login.cshtml", model);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == model.Email &&
                    u.Password == model.Password);

                if (user == null)
                {
                    TempData["Error"] = "Invalid email or password.";

                    return View("~/Views/Guest/Auth/Login.cshtml", model);
                }

                if (!user.IsPaymentDone)
                {
                    TempData["Error"] = "Please complete payment first.";

                    return RedirectToAction("PaymentQRCode",
                        new { userId = user.Id });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(ClaimTypes.Name,
                        user.Name),

                    new Claim(ClaimTypes.Email,
                        user.Email),

                    new Claim(ClaimTypes.Role,
                        user.Role)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                TempData["success"] = "Login successful";

                // ADMIN LOGIN
                if (user.Role.ToLower() == "admin")
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Admin" });
                }

                // USER LOGIN
                return RedirectToAction(
                    "Index",
                    "Home",
                    new { area = "User" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View("~/Views/Guest/Auth/Login.cshtml", model);
            }
        }

        // =========================
        // LOGOUT
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["success"] = "Logged out successfully";

            // Redirect to guest homepage
            return RedirectToAction(
                "Index",
                "Home",
                new { area = "Guest" });
        }
    }
}