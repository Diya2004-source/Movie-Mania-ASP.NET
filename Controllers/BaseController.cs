using Microsoft.AspNetCore.Mvc;

namespace MovieMania.Controllers
{
    public class BaseController : Controller
    {
        protected int? GetCurrentUserId()
        {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            // Try to get from session first
            var userId = HttpContext.Session.GetInt32("UserId");

            // If not in session, try to get from cookie
<<<<<<< HEAD
=======
=======
            //Will Try to get from session first
            var userId = HttpContext.Session.GetInt32("UserId");

            // If not in session, try to get from cookie    
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            if (userId == null)
            {
                var cookieValue = Request.Cookies["UserId"];
                if (!string.IsNullOrEmpty(cookieValue) && int.TryParse(cookieValue, out int id))
                {
                    userId = id;
                    // Restore session from cookie
                    HttpContext.Session.SetInt32("UserId", id);
                }
            }

            return userId;
        }

        protected bool IsUserLoggedIn()
        {
            return GetCurrentUserId() != null;
        }
    }
}