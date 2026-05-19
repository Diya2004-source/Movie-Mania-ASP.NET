using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("MovieReview")]
    public class MovieReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string ReviewText { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        public int HelpfulCount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
    }
}