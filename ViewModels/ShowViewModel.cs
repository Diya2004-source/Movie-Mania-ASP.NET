//using System.Collections.Generic;

//namespace MovieMania.ViewModels
//{
//    public class ShowViewModel
//    {
//        public int Id { get; set; }
//        public string Title { get; set; } = string.Empty;
//        public string? Description { get; set; }
//        public string? ThumbnailUrl { get; set; }
//        public string? PosterUrl { get; set; }
//        public int? ReleaseYear { get; set; }
//        public string? Genre { get; set; }
//        public double? Rating { get; set; }
//        public int ViewsCount { get; set; }
//        public int TotalSeasons { get; set; }
//        public int TotalEpisodes { get; set; }
//        public bool IsInWishlist { get; set; }
//    }

//    public class ShowDetailsViewModel
//    {
//        public ShowViewModel Show { get; set; } = new();
//        public List<EpisodeViewModel> Episodes { get; set; } = new();
//        public List<ShowViewModel> SimilarShows { get; set; } = new();
//        public bool IsInWishlist { get; set; }
//        public double AverageRating { get; set; }
//        public int TotalReviews { get; set; }
//        public Dictionary<int, List<EpisodeViewModel>> EpisodesBySeason { get; set; } = new();
//    }

//    public class EpisodeViewModel
//    {
//        public int Id { get; set; }
//        public int ShowId { get; set; }
//        public string Title { get; set; } = string.Empty;
//        public string? Description { get; set; }
//        public int SeasonNumber { get; set; }
//        public int EpisodeNumber { get; set; }
//        public int? Duration { get; set; }
//        public string? ThumbnailUrl { get; set; }
//        public string? VideoUrl { get; set; }
//        public DateTime? ReleaseDate { get; set; }
//        public bool IsActive { get; set; }
//    }
//}

using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class ShowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PosterUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Genre { get; set; }
        public double? Rating { get; set; }
        public int ViewsCount { get; set; }
        public int TotalSeasons { get; set; }
        public int TotalEpisodes { get; set; }
        public bool IsInWishlist { get; set; }
    }

    public class ShowDetailsViewModel
    {
        public Show Show { get; set; } = new();
        public List<Episode> Episodes { get; set; } = new();
        public List<Show> SimilarShows { get; set; } = new();
        public List<Show> RelatedShows { get; set; } = new();
        public bool IsInWishlist { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; } = new();
    }

    public class EpisodeViewModel
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public int? Duration { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsActive { get; set; }
    }
}