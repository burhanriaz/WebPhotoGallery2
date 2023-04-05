using Microsoft.AspNetCore.Identity;

namespace WebPhotoGallery.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegDate { get; set; }
    }
}
