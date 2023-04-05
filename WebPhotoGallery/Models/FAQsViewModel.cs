using System.ComponentModel.DataAnnotations;

namespace WebPhotoGallery.Models
{
    public class FAQsViewModel
    {
        [Required(ErrorMessage = "Question is required")]
        [StringLength(500, ErrorMessage = "Must be between 10 and 500 characters", MinimumLength = 10)]
        public string Question { get; set; }
    }

    public class FAQsAnswerViewModel
    {
        public int QuestionID { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        [StringLength(500, ErrorMessage = "Must be between 10 and 500 characters", MinimumLength = 10)]
        public string Answer { get; set; }
    }
}
