using System;

namespace MovieMania.ViewModels
{
    public class MovieReviewViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfilePicture { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int HelpfulCount { get; set; }
        public bool IsHelpful { get; set; }
    }
}