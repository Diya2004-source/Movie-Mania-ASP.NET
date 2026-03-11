using System;
using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class GuestHomeViewModel
    {
        public List<Movie> FeaturedMovies { get; set; } = new List<Movie>();
        public List<Movie> LatestMovies { get; set; } = new List<Movie>();
        public List<Movie> PopularMovies { get; set; } = new List<Movie>();
        public List<Show> TrendingShows { get; set; } = new List<Show>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }

    public class GuestMovieDetailsViewModel
    {
        public Movie Movie { get; set; } = new Movie();
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
    }

    public class GuestShowDetailsViewModel
    {
        public Show Show { get; set; } = new Show();
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; } = new Dictionary<int, List<Episode>>();
        public List<Show> RelatedShows { get; set; } = new List<Show>();
        public List<ShowReview> Reviews { get; set; } = new List<ShowReview>();
    }

    public class GuestSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Show> Shows { get; set; } = new List<Show>();
        public int TotalResults { get; set; }
    }
}