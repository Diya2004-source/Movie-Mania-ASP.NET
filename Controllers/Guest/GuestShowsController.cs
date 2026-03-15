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
    [Route("Shows")]
    public class GuestShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? genre, string? sortBy, int page = 1)
        {
            var query = _context.Shows
                .Include(s => s.Episodes)
                .Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(s => s.Genre != null && s.Genre == genre);
            }

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(s => s.Rating),
                "latest" => query.OrderByDescending(s => s.CreatedAt),
                "views" => query.OrderByDescending(s => s.ViewsCount),
                _ => query.OrderByDescending(s => s.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var shows = await query
                .Skip((page - 1) * 12)
                .Take(12)
                .ToListAsync(); // Return Show entities, not ViewModels

            var genres = await _context.Shows
                .Where(s => s.IsActive && s.Genre != null)
                .Select(s => s.Genre!)
                .Distinct()
                .ToListAsync();

            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = genre ?? "All";
            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / 12) : 1;

            return View("~/Views/Guest/Home/Shows.cshtml", shows); // Pass List<Show>
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber))
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (show == null)
            {
                return NotFound();
            }

            show.ViewsCount++;
            await _context.SaveChangesAsync();

            var episodesBySeason = show.Episodes?
                .GroupBy(e => e.SeasonNumber)
                .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<Episode>>();

            var similarShows = await _context.Shows
                .Where(s => s.IsActive && s.Genre == show.Genre && s.Id != show.Id)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToListAsync();

            var relatedShows = await _context.Shows
                .Where(s => s.IsActive && s.Id != show.Id)
                .OrderByDescending(s => s.ViewsCount)
                .Take(6)
                .ToListAsync();

            double averageRating = show.Rating.HasValue ? (double)show.Rating.Value : 0;

            var viewModel = new GuestShowDetailsViewModel
            {
                Show = show,
                Episodes = show.Episodes?.ToList() ?? new List<Episode>(),
                SimilarShows = similarShows,
                RelatedShows = relatedShows,
                EpisodesBySeason = episodesBySeason,
                TotalSeasons = episodesBySeason.Count,
                Reviews = new List<ShowReview>(),
                AverageRating = averageRating,
                TotalReviews = 0,
                IsInWishlist = false
            };

            return View("~/Views/Guest/Home/ShowDetails.cshtml", viewModel);
        }
    }
}