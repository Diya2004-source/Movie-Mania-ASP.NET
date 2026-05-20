namespace MovieMania.ViewModels
{
    public class WishlistUpdateViewModel
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty; // "Movie" or "Show"
        public bool IsAdding { get; set; } = true;
    }

    public class WishlistItemViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? Genre { get; set; }
        public int? ReleaseYear { get; set; }
        public double? Rating { get; set; }
        public DateTime AddedDate { get; set; }
    }
}