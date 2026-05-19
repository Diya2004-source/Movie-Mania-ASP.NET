<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
=======
﻿//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MovieMania.Models;
//using MovieMania.ViewModels;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System;

//namespace MovieMania.Controllers.Guest
//{
//    public class GuestHomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public GuestHomeController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            try
//            {
//                var featuredMovies = await _context.Movies
//                    .Where(m => m.IsActive)
//                    .OrderByDescending(m => m.CreatedAt)
//                    .Take(10)
//                    .Select(m => new MovieViewModel
//                    {
//                        Id = m.Id,
//                        Title = m.Title ?? "Untitled",
//                        Description = m.Description ?? "No description available",
//                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
//                        ReleaseYear = m.ReleaseYear,
//                        Genre = m.Genre ?? "Unknown",
//                        Duration = m.Duration,
//                        Language = m.Language ?? "English",
//                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
//                        ViewsCount = m.ViewsCount
//                    })
//                    .ToListAsync();

//                var latestMovies = await _context.Movies
//                    .Where(m => m.IsActive)
//                    .OrderByDescending(m => m.CreatedAt)
//                    .Take(12)
//                    .Select(m => new MovieViewModel
//                    {
//                        Id = m.Id,
//                        Title = m.Title ?? "Untitled",
//                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
//                        Genre = m.Genre ?? "Unknown",
//                        ReleaseYear = m.ReleaseYear,
//                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
//                        ViewsCount = m.ViewsCount
//                    })
//                    .ToListAsync();

//                var popularMovies = await _context.Movies
//                    .Where(m => m.IsActive)
//                    .OrderByDescending(m => m.ViewsCount)
//                    .Take(8)
//                    .Select(m => new MovieViewModel
//                    {
//                        Id = m.Id,
//                        Title = m.Title ?? "Untitled",
//                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
//                        Genre = m.Genre ?? "Unknown",
//                        ReleaseYear = m.ReleaseYear,
//                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
//                        ViewsCount = m.ViewsCount
//                    })
//                    .ToListAsync();

//                var trendingShows = await _context.Shows
//                    .Where(s => s.IsActive)
//                    .Include(s => s.Episodes)
//                    .OrderByDescending(s => s.ViewsCount)
//                    .Take(8)
//                    .Select(s => new ShowViewModel
//                    {
//                        Id = s.Id,
//                        Title = s.Title ?? "Untitled",
//                        ThumbnailUrl = s.ThumbnailUrl ?? "/images/default-show.jpg",
//                        Genre = s.Genre ?? "Unknown",
//                        ReleaseYear = s.ReleaseYear,
//                        Rating = s.Rating.HasValue ? (double?)s.Rating.Value : null,
//                        ViewsCount = s.ViewsCount,
//                        TotalSeasons = s.Episodes != null
//                            ? s.Episodes.Select(e => e.SeasonNumber).Distinct().Count()
//                            : 0,
//                        TotalEpisodes = s.Episodes != null ? s.Episodes.Count : 0
//                    })
//                    .ToListAsync();

//                var viewModel = new GuestHomeViewModel
//                {
//                    FeaturedMovies = featuredMovies,
//                    LatestMovies = latestMovies,
//                    PopularMovies = popularMovies,
//                    TrendingShows = trendingShows,
//                    TotalMovies = await _context.Movies.CountAsync(m => m.IsActive),
//                    TotalShows = await _context.Shows.CountAsync(s => s.IsActive),
//                    TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
//                    RecommendedForYou = latestMovies.Take(6).ToList()
//                };

//                return View("~/Views/Guest/Home/Index.cshtml", viewModel);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error in GuestHomeController.Index: {ex.Message}");

//                var emptyViewModel = new GuestHomeViewModel
//                {
//                    FeaturedMovies = new List<MovieViewModel>(),
//                    LatestMovies = new List<MovieViewModel>(),
//                    PopularMovies = new List<MovieViewModel>(),
//                    TrendingShows = new List<ShowViewModel>(),
//                    TotalMovies = 0,
//                    TotalShows = 0,
//                    TotalUsers = 0,
//                    RecommendedForYou = new List<MovieViewModel>()
//                };

//                return View("~/Views/Guest/Home/Index.cshtml", emptyViewModel);
//            }
//        }

//        // Static pages
//        public IActionResult About()
//        {
//            return View("~/Views/Guest/Home/About.cshtml");
//        }

//        public IActionResult Contact()
//        {
//            return View("~/Views/Guest/Home/Contact.cshtml");
//        }

//        // This method will be redirected by the route to GuestAnimeController
//        public IActionResult Anime()
//        {
//            return RedirectToAction("Index", "GuestAnime");
//        }

//        // Redirect methods for backward compatibility
//        public IActionResult Movies()
//        {
//            return RedirectToAction("Index", "GuestMovies");
//        }

//        public IActionResult Shows()
//        {
//            return RedirectToAction("Index", "GuestShows");
//        }

//        public async Task<IActionResult> MovieDetails(int id)
//        {
//            return RedirectToAction("Details", "GuestMovies", new { id = id });
//        }

//        public async Task<IActionResult> ShowDetails(int id)
//        {
//            return RedirectToAction("Details", "GuestShows", new { id = id });
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MovieMania.Controllers.Guest
{
    public class GuestHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var featuredMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(10)
                    .Select(m => new MovieViewModel
                    {
                        Id = m.Id,
                        Title = m.Title ?? "Untitled",
                        Description = m.Description ?? "No description available",
                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
                        ReleaseYear = m.ReleaseYear,
                        Genre = m.Genre ?? "Unknown",
                        Duration = m.Duration,
                        Language = m.Language ?? "English",
<<<<<<< HEAD
                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
=======
                        Rating = (double?)m.Rating,   // ✅ explicit cast
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                        ViewsCount = m.ViewsCount
                    })
                    .ToListAsync();

                var latestMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(12)
                    .Select(m => new MovieViewModel
                    {
                        Id = m.Id,
                        Title = m.Title ?? "Untitled",
                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
                        Genre = m.Genre ?? "Unknown",
                        ReleaseYear = m.ReleaseYear,
<<<<<<< HEAD
                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
=======
                        Rating = (double?)m.Rating,   // ✅ explicit cast
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                        ViewsCount = m.ViewsCount
                    })
                    .ToListAsync();

                var popularMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.ViewsCount)
                    .Take(8)
                    .Select(m => new MovieViewModel
                    {
                        Id = m.Id,
                        Title = m.Title ?? "Untitled",
                        ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
                        Genre = m.Genre ?? "Unknown",
                        ReleaseYear = m.ReleaseYear,
<<<<<<< HEAD
                        Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
=======
                        Rating = (double?)m.Rating,   // ✅ explicit cast
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                        ViewsCount = m.ViewsCount
                    })
                    .ToListAsync();

                var trendingShows = await _context.Shows
                    .Where(s => s.IsActive)
                    .Include(s => s.Episodes)
                    .OrderByDescending(s => s.ViewsCount)
                    .Take(8)
                    .Select(s => new ShowViewModel
                    {
                        Id = s.Id,
                        Title = s.Title ?? "Untitled",
                        ThumbnailUrl = s.ThumbnailUrl ?? "/images/default-show.jpg",
                        Genre = s.Genre ?? "Unknown",
                        ReleaseYear = s.ReleaseYear,
<<<<<<< HEAD
                        Rating = s.Rating.HasValue ? (double?)s.Rating.Value : null,
=======
                        Rating = (double?)s.Rating,   // ✅ explicit cast
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                        ViewsCount = s.ViewsCount,
                        TotalSeasons = s.Episodes != null
                            ? s.Episodes.Select(e => e.SeasonNumber).Distinct().Count()
                            : 0,
                        TotalEpisodes = s.Episodes != null ? s.Episodes.Count : 0
                    })
                    .ToListAsync();

                var viewModel = new GuestHomeViewModel
                {
                    FeaturedMovies = featuredMovies,
                    LatestMovies = latestMovies,
                    PopularMovies = popularMovies,
                    TrendingShows = trendingShows,
                    TotalMovies = await _context.Movies.CountAsync(m => m.IsActive),
                    TotalShows = await _context.Shows.CountAsync(s => s.IsActive),
                    TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
                    RecommendedForYou = latestMovies.Take(6).ToList()
                };

                return View("~/Views/Guest/Home/Index.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GuestHomeController.Index: {ex.Message}");

                var emptyViewModel = new GuestHomeViewModel
                {
                    FeaturedMovies = new List<MovieViewModel>(),
                    LatestMovies = new List<MovieViewModel>(),
                    PopularMovies = new List<MovieViewModel>(),
                    TrendingShows = new List<ShowViewModel>(),
                    TotalMovies = 0,
                    TotalShows = 0,
                    TotalUsers = 0,
                    RecommendedForYou = new List<MovieViewModel>()
                };

                return View("~/Views/Guest/Home/Index.cshtml", emptyViewModel);
            }
        }

        // Static pages
        public IActionResult About()
        {
            return View("~/Views/Guest/Home/About.cshtml");
        }

        public IActionResult Contact()
        {
            return View("~/Views/Guest/Home/Contact.cshtml");
        }

<<<<<<< HEAD
        // This method will be redirected by the route to GuestAnimeController
=======
        // Redirect methods
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
        public IActionResult Anime()
        {
            return RedirectToAction("Index", "GuestAnime");
        }

<<<<<<< HEAD
        // Redirect methods for backward compatibility
=======
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
        public IActionResult Movies()
        {
            return RedirectToAction("Index", "GuestMovies");
        }

        public IActionResult Shows()
        {
            return RedirectToAction("Index", "GuestShows");
        }

<<<<<<< HEAD
        public async Task<IActionResult> MovieDetails(int id)
        {
            return RedirectToAction("Details", "GuestMovies", new { id = id });
        }

        public async Task<IActionResult> ShowDetails(int id)
        {
            return RedirectToAction("Details", "GuestShows", new { id = id });
=======
        public IActionResult MovieDetails(int id)
        {
            return RedirectToAction("Details", "GuestMovies", new { id });
        }

        public IActionResult ShowDetails(int id)
        {
            return RedirectToAction("Details", "GuestShows", new { id });
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
        }
    }
}