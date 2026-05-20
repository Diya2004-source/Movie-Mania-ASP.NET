namespace MovieMania.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Genre { get; set; }
        public int? Duration { get; set; }
        public string? Language { get; set; }
        public double? Rating { get; set; }
        public int ViewsCount { get; set; }
        public bool IsInWishlist { get; set; }
    }

    // IMPORTANT: This file should ONLY contain MovieViewModel
    // Do not add MovieReviewViewModel or MovieRatingViewModel here
}