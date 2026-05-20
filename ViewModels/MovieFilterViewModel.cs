using System;
using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class MovieFilterViewModel
    {
        public List<string> Genres { get; set; } = new();
        public List<int> Years { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public string? SelectedGenre { get; set; }
        public int? SelectedYear { get; set; }
        public string? SelectedLanguage { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}