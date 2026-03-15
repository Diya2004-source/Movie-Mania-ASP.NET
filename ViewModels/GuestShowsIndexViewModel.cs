using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class GuestShowsIndexViewModel
    {
        public List<ShowViewModel> Shows { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public string CurrentGenre { get; set; } = "All";
        public string CurrentSort { get; set; } = "latest";
        public int TotalShows { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}