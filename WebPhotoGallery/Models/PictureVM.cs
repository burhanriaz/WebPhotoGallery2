using System.ComponentModel.DataAnnotations;

namespace WebPhotoGallery.Models
{
    public class PictureVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Must be between 5 and 500 characters", MinimumLength = 5)]
        public string Description { get; set; }

    }
}
