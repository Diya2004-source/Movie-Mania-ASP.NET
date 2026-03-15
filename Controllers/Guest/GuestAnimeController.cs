using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MovieMania.Controllers.Guest
{
    [Route("Anime")]
    public class GuestAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestAnimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy, int page = 1)
        {
            // Filter movies where Genre is "Anime" (case-insensitive)
            var query = _context.Movies
                .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime");

            int totalCount = await query.CountAsync();

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "latest" => query.OrderByDescending(m => m.CreatedAt),
                "views" => query.OrderByDescending(m => m.ViewsCount),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };

            var animes = await query
                .Skip((page - 1) * 12)
                .Take(12)
                .ToListAsync(); // Return Movie entities

            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / 12) : 1;

            return View("~/Views/Guest/Home/Anime.cshtml", animes); // Pass List<Movie>
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var anime = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime");

            if (anime == null)
            {
                return NotFound();
            }

            anime.ViewsCount++;
            await _context.SaveChangesAsync();

            var similarAnimes = await _context.Movies
                .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime" && m.Id != id)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToListAsync();

            ViewBag.SimilarAnimes = similarAnimes;

            return View("~/Views/Guest/Home/MovieDetails.cshtml", anime);
        }
    }
}