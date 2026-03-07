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

        // ================= MOVIE LIST =================
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View("~/Views/Admin/Movies/Index.cshtml", movies);
        }

        // ================= CREATE GET =================
        public IActionResult Create()
        {
            return View("~/Views/Admin/Movies/Create.cshtml");
        }

        // ================= CREATE POST =================
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
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ThumbnailFile.CopyTo(stream);
                }

                movie.Thumbnail = uniqueFileName;
            }

            movie.CreatedAt = DateTime.Now;

            _context.Movies.Add(movie);
            _context.SaveChanges();

            TempData["Success"] = "Movie Added Successfully!";

            return RedirectToAction("Index");
        }

        // ================= EDIT GET =================
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Find(id);

            if (movie == null)
                return NotFound();

            return View("~/Views/Admin/Movies/Edit.cshtml", movie);
        }

        // ================= EDIT POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Movie movie, IFormFile ThumbnailFile)
        {
            var existingMovie = _context.Movies.Find(id);

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
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies");

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ThumbnailFile.CopyTo(stream);
                }

                existingMovie.Thumbnail = uniqueFileName;
            }

            _context.SaveChanges();

            TempData["Success"] = "Movie Updated Successfully!";

            return RedirectToAction("Index");
        }

        // ================= DELETE =================
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            TempData["Success"] = "Movie Deleted Successfully!";

            return RedirectToAction("Index");
        }
    }
}