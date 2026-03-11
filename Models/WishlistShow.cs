using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("WishlistShows")]
    public class WishlistShow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ShowId { get; set; }

        [Display(Name = "Added Date")]
        [DataType(DataType.DateTime)]
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; }
    }
}