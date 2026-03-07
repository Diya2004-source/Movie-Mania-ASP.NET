//using Microsoft.AspNetCore.Mvc;
//using MovieMania.Models;
//using System.Linq;

//namespace MovieMania.Controllers.User
//{
//    public class HomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public HomeController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            var movies = _context.Movies.ToList();

//            return View(movies);
//        }
//    }
//}