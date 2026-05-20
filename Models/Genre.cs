using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Genres")]
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Genre name must be between 2 and 50 characters")]
        [Display(Name = "Genre Name")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
        public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}