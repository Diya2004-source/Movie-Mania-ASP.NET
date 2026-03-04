//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using MovieMania.Models;
//using System;
//using System.IO;
//using System.Linq;

//namespace MovieMania.Controllers
//{
//    public class AdminMoviesController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AdminMoviesController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ===================== INDEX =====================
//        public IActionResult Index()
//        {
//            var movies = _context.Movies.ToList();
//            return View("~/Views/Admin/Movies/Index.cshtml", movies);
//        }

//        // ===================== CREATE (GET) =====================
//        public IActionResult Create()
//        {
//            return View("~/Views/Admin/Movies/Create.cshtml");
//        }

//        // ===================== CREATE (POST) =====================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(Movie movie, IFormFile ThumbnailFile)
//        {
//            if (!ModelState.IsValid)
//                return View("~/Views/Admin/Movies/Create.cshtml", movie);

//            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
//            {
//                movie.Thumbnail = SaveThumbnail(ThumbnailFile);
//            }

//            _context.Movies.Add(movie);
//            _context.SaveChanges();

//            return RedirectToAction("Index");
//        }

//        // ===================== EDIT (GET) =====================
//        public IActionResult Edit(int id)
//        {
//            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
//            if (movie == null)
//                return NotFound();

//            return View("~/Views/Admin/Movies/Edit.cshtml", movie);
//        }

//        // ===================== EDIT (POST) =====================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Edit(int id, Movie movie, IFormFile ThumbnailFile)
//        {
//            if (!ModelState.IsValid)
//                return View("~/Views/Admin/Movies/Edit.cshtml", movie);

//            var existingMovie = _context.Movies.FirstOrDefault(m => m.Id == id);
//            if (existingMovie == null)
//                return NotFound();

//            existingMovie.Title = movie.Title;
//            existingMovie.Description = movie.Description;
//            existingMovie.Genre = movie.Genre;
//            existingMovie.ReleaseYear = movie.ReleaseYear;
//            existingMovie.Duration = movie.Duration;
//            existingMovie.VideoUrl = movie.VideoUrl;

//            // If new thumbnail uploaded
//            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
//            {
//                // Delete old image
//                if (!string.IsNullOrEmpty(existingMovie.Thumbnail))
//                {
//                    DeleteThumbnail(existingMovie.Thumbnail);
//                }

//                existingMovie.Thumbnail = SaveThumbnail(ThumbnailFile);
//            }

//            _context.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        // ===================== DELETE =====================
//        public IActionResult Delete(int id)
//        {
//            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
//            if (movie == null)
//                return NotFound();

//            // Delete image file
//            if (!string.IsNullOrEmpty(movie.Thumbnail))
//            {
//                DeleteThumbnail(movie.Thumbnail);
//            }

//            _context.Movies.Remove(movie);
//            _context.SaveChanges();

//            return RedirectToAction("Index");
//        }

//        // ===================== PRIVATE IMAGE SAVE METHOD =====================
//        private string SaveThumbnail(IFormFile file)
//        {
//            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

//            if (!Directory.Exists(uploadsFolder))
//                Directory.CreateDirectory(uploadsFolder);

//            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
//            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                file.CopyTo(stream);
//            }

//            return uniqueFileName;
//        }

//        // ===================== PRIVATE IMAGE DELETE METHOD =====================
//        private void DeleteThumbnail(string fileName)
//        {
//            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

//            if (System.IO.File.Exists(filePath))
//            {
//                System.IO.File.Delete(filePath);
//            }
//        }
//    }
//}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System;
using System.IO;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    public class AdminMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== INDEX =====================
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View("~/Views/Admin/Movies/Index.cshtml", movies);
        }

        // ===================== CREATE (GET) =====================
        public IActionResult Create()
        {
            return View("~/Views/Admin/Movies/Create.cshtml");
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile ThumbnailFile)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Movies/Create.cshtml", movie);
            }

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                movie.Thumbnail = SaveThumbnail(ThumbnailFile);
            }

            movie.CreatedAt = DateTime.Now;
            _context.Movies.Add(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT (GET) =====================
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            return View("~/Views/Admin/Movies/Edit.cshtml", movie);
        }

        // ===================== EDIT (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Movie movie, IFormFile ThumbnailFile)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Movies/Edit.cshtml", movie);
            }

            var existingMovie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (existingMovie == null)
                return NotFound();

            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.Genre = movie.Genre;
            existingMovie.ReleaseYear = movie.ReleaseYear;
            existingMovie.Duration = movie.Duration;
            existingMovie.VideoUrl = movie.VideoUrl;

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingMovie.Thumbnail))
                {
                    DeleteThumbnail(existingMovie.Thumbnail);
                }

                existingMovie.Thumbnail = SaveThumbnail(ThumbnailFile);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            if (!string.IsNullOrEmpty(movie.Thumbnail))
            {
                DeleteThumbnail(movie.Thumbnail);
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ===================== PRIVATE IMAGE SAVE METHOD =====================
        private string SaveThumbnail(IFormFile file)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return uniqueFileName;
        }

        // ===================== PRIVATE IMAGE DELETE METHOD =====================
        private void DeleteThumbnail(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}