using Microsoft.AspNetCore.Mvc;

namespace MovieMania.Controllers
{
    public class BaseController : Controller
    {
        protected int? GetCurrentUserId()
        {
            // Try to get from session first
            var userId = HttpContext.Session.GetInt32("UserId");

            // If not in session, try to get from cookie
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