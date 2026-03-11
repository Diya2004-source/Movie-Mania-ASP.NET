// ViewModels/WishlistUpdateViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MovieMania.ViewModels
{
    public class WishlistUpdateViewModel
    {
        public int Id { get; set; }

        public int? Priority { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public bool? NotificationEnabled { get; set; }
    }
}