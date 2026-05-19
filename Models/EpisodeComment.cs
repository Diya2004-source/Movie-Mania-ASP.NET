using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("EpisodeComments")]
    public class EpisodeComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EpisodeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Comment Date")]
        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;

        [Display(Name = "Like Count")]
        public int LikeCount { get; set; } = 0;

        [Display(Name = "Dislike Count")]
        public int DislikeCount { get; set; } = 0;

        public int? ParentCommentId { get; set; }

        // Navigation Properties
        [ForeignKey("EpisodeId")]
        public virtual Episode Episode { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual EpisodeComment ParentComment { get; set; }

        public virtual ICollection<EpisodeComment> Replies { get; set; } = new List<EpisodeComment>();
    }
}