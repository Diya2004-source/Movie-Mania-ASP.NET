using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class GuestSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Movie> Movies { get; set; } = new();
        public List<Show> Shows { get; set; } = new();
        public int TotalResults { get; set; }
    }
}