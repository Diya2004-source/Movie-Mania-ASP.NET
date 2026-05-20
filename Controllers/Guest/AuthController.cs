<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
﻿//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Security.Claims;
//using MovieMania.Models; // Ensure this matches your namespace

//namespace MovieMania.Controllers
<<<<<<< HEAD
=======
=======
﻿//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Mvc;
//using MovieMania.Models;
//using System.Security.Claims;
//using MovieMania.ViewModels;

//namespace MovieMania.Controllers.Guest
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
//{
//    public class AuthController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AuthController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
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
<<<<<<< HEAD
=======
=======
//        // GET: /Auth/Login
//        [HttpGet]
//        public IActionResult Login(string? returnUrl = null)
//        {
//            // If already logged in as regular user, go to user dashboard
//            if (User.Identity?.IsAuthenticated == true)
//            {
//                if (User.IsInRole("admin"))
//                {
//                    // If admin is logged in, log them out first
//                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
//                    TempData["Info"] = "Please use admin login page";
//                }
//                else
//                {
//                    return RedirectToAction("Index", "Home", new { area = "User" });
//                }
//            }

//            ViewData["ReturnUrl"] = returnUrl ?? string.Empty;
//            return View("~/Views/Guest/Auth/Login.cshtml");
//        }

//        // POST: /Auth/Login
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
//        {
//            if (!ModelState.IsValid)
//                return View("~/Views/Guest/Auth/Login.cshtml", model);

//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

//            if (user == null || user.Password != model.Password || !user.IsActive)
//            {
//                ModelState.AddModelError(string.Empty, "Invalid login attempt");
//                TempData["Error"] = "Invalid email or password.";
//                return View("~/Views/Guest/Auth/Login.cshtml", model);
//            }

//            // IMPORTANT: Prevent admin login from guest login page
//            if (user.Role == "admin")
//            {
//                ModelState.AddModelError(string.Empty, "Please use the admin login page");
//                TempData["Error"] = "Admin accounts cannot login here. Please use the admin login page.";
//                return View("~/Views/Guest/Auth/Login.cshtml", model);
//            }

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
//                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
//                new Claim(ClaimTypes.Role, user.Role ?? "user")
//            };

//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
//                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
//                new AuthenticationProperties { IsPersistent = model.RememberMe });

//            user.LastLoginAt = DateTime.Now;
//            await _context.SaveChangesAsync();

//            TempData["Success"] = $"Welcome back, {user.Name}!";

//            // Check if there's a pending payment
//            if (TempData["PendingPlanId"] != null)
//            {
//                int planId = Convert.ToInt32(TempData["PendingPlanId"]);
//                TempData.Remove("PendingPlanId");
//                return RedirectToAction("Process", "Payment", new { area = "User", planId = planId });
//            }

//            // For regular users, always go to User dashboard
//            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !returnUrl.StartsWith("/Admin"))
//            {
//                return Redirect(returnUrl);
//            }

//            return RedirectToAction("Index", "Home", new { area = "User" });
//        }

//        // GET: /Auth/Register
//        [HttpGet]
//        public IActionResult Register(string? planId = null, string? referralCode = null)
//        {
//            ViewBag.PlanId = planId;
//            ViewBag.ReferralCode = referralCode;
//            return View("~/Views/Guest/Auth/Register.cshtml");
//        }

//        // POST: /Auth/Register
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel model, string? planId = null)
//        {
//            if (!ModelState.IsValid)
//            {
//                ViewBag.PlanId = planId;
//                return View("~/Views/Guest/Auth/Register.cshtml", model);
//            }

//            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
//            {
//                ModelState.AddModelError("Email", "Email already exists");
//                TempData["Error"] = "Email already registered.";
//                ViewBag.PlanId = planId;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
//                return View("~/Views/Guest/Auth/Register.cshtml", model);
//            }

//            var user = new AppUser
//            {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
//                Name = model.Name,
//                Email = model.Email,
//                Password = model.Password,
//                Role = "user",
//                IsActive = false,
//                IsPaymentDone = false,
//                PaymentStatus = "Pending",
//                CreatedAt = DateTime.Now,
//                ReferralCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
<<<<<<< HEAD
=======
=======
//                Name = model.Name ?? string.Empty,
//                Email = model.Email ?? string.Empty,
//                Password = model.Password ?? string.Empty,
//                Role = "user", // Always set to "user" for new registrations
//                IsActive = true,
//                CreatedAt = DateTime.Now,
//                ReferralCode = GenerateReferralCode()
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
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
<<<<<<< HEAD
=======
=======
//            // After successful registration, ALWAYS redirect to login page
//            if (!string.IsNullOrEmpty(planId) && int.TryParse(planId, out int parsedPlanId))
//            {
//                TempData["PendingPlanId"] = parsedPlanId;
//                TempData["Success"] = "Registration successful! Please login to complete your payment.";
//            }
//            else
//            {
//                TempData["Success"] = "Registration successful! Please login.";
//            }

//            return RedirectToAction(nameof(Login));
//        }

//        private string GenerateReferralCode()
//        {
//            return "MOVIE" + new Random().Next(1000, 9999);
//        }

//        // GET: /Auth/Logout
//        [HttpGet]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            TempData["Success"] = "You have been logged out successfully.";
//            return RedirectToAction("Index", "GuestHome");
//        }

//        // POST: /Auth/Logout
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> LogoutPost()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            TempData["Success"] = "You have been logged out successfully.";
//            return RedirectToAction("Index", "GuestHome");
//        }

//        // GET: /Auth/AccessDenied
//        [HttpGet]
//        public IActionResult AccessDenied()
//        {
//            return View("~/Views/Guest/Auth/AccessDenied.cshtml");
//        }

//        // GET: /Auth/ForgotPassword
//        [HttpGet]
//        public IActionResult ForgotPassword()
//        {
//            return View("~/Views/Guest/Auth/ForgotPassword.cshtml");
//        }

//        // POST: /Auth/ForgotPassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View("~/Views/Guest/Auth/ForgotPassword.cshtml", model);

//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

//            if (user != null)
//            {
//                // Here you would send password reset email
//                TempData["Success"] = "Password reset link has been sent to your email.";
//            }
//            else
//            {
//                TempData["Success"] = "If your email is registered, you will receive a password reset link.";
//            }

//            return RedirectToAction(nameof(Login));
//        }

//        // GET: /Auth/ResetPassword
//        [HttpGet]
//        public IActionResult ResetPassword(string? token = null, string? email = null)
//        {
//            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
//            {
//                TempData["Error"] = "Invalid password reset token.";
//                return RedirectToAction(nameof(Login));
//            }

//            var model = new ResetPasswordViewModel
//            {
//                Email = email,
//                Code = token
//            };

//            return View("~/Views/Guest/Auth/ResetPassword.cshtml", model);
//        }

//        // POST: /Auth/ResetPassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View("~/Views/Guest/Auth/ResetPassword.cshtml", model);

//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

//            if (user == null)
//            {
//                TempData["Error"] = "Invalid password reset attempt.";
//                return RedirectToAction(nameof(Login));
//            }

//            user.Password = model.Password ?? string.Empty;
//            await _context.SaveChangesAsync();

//            TempData["Success"] = "Password has been reset successfully. Please login with your new password.";
//            return RedirectToAction(nameof(Login));
//        }

//        // GET: /Auth/CheckEmailAvailability
//        [HttpGet]
//        public async Task<IActionResult> CheckEmailAvailability(string email)
//        {
//            if (string.IsNullOrEmpty(email))
//                return Json(new { available = false, message = "Email is required" });

//            var exists = await _context.Users.AnyAsync(u => u.Email == email);
//            return Json(new { available = !exists, message = exists ? "Email already registered" : "Email available" });
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
//        }
//    }
//}

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MovieMania.Models;
using Razorpay.Api;

namespace MovieMania.Controllers
<<<<<<< HEAD
=======
=======
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Security.Claims;
using MovieMania.ViewModels;

namespace MovieMania.Controllers.Guest
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // REGISTER
        // =========================

<<<<<<< HEAD
=======
=======
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("admin"))
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                    TempData["Info"] = "Please use admin login page";
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }

            ViewData["ReturnUrl"] = returnUrl ?? string.Empty;
            return View("~/Views/Guest/Auth/Login.cshtml");
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Guest/Auth/Login.cshtml", model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.Password != model.Password || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                TempData["Error"] = "Invalid email or password.";
                return View("~/Views/Guest/Auth/Login.cshtml", model);
            }

            if (user.Role == "admin")
            {
                ModelState.AddModelError(string.Empty, "Please use the admin login page");
                TempData["Error"] = "Admin accounts cannot login here.";
                return View("~/Views/Guest/Auth/Login.cshtml", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Welcome back, {user.Name}!";

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        // GET: /Auth/Register
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Guest/Auth/Register.cshtml");
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
        // POST: /Auth/Register
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
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
<<<<<<< HEAD
=======
=======
            if (!ModelState.IsValid)
                return View("~/Views/Guest/Auth/Register.cshtml", model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                TempData["Error"] = "Email already registered.";
                return View("~/Views/Guest/Auth/Register.cshtml", model);
            }

            var user = new AppUser
            {
                Name = model.Name ?? string.Empty,
                Email = model.Email ?? string.Empty,
                Password = model.Password ?? string.Empty,
                Role = "user",
                IsActive = false, // inactive until payment
                CreatedAt = DateTime.Now,
                ReferralCode = GenerateReferralCode()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Redirect to payment page after registration
            return RedirectToAction("PaymentQRCode", "Home", new { area = "User", email = user.Email });
        }

        private string GenerateReferralCode()
        {
            return "MOVIE" + new Random().Next(1000, 9999);
        }

        // GET: /Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "GuestHome");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "GuestHome");
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        }
    }
}