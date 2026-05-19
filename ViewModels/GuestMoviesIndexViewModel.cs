using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class GuestMoviesIndexViewModel
    {
        public List<MovieViewModel> Movies { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public string CurrentGenre { get; set; } = "All";
        public string CurrentSort { get; set; } = "latest";
        public int TotalMovies { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}