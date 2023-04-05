using System.ComponentModel.DataAnnotations;

namespace WebPhotoGallery.Models
{
    public class FAQS
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Question is required")]
        [StringLength(255, ErrorMessage = "Must be between 10 and 500 characters", MinimumLength = 10)]
        public string Question { get; set; }
        public string? Answer { get; set; }
    }

    
}
