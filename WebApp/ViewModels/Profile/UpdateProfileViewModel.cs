using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Profile
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Must be a valid email address.")]
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Must be a valid phone number.")]
        public string? Phone { get; set; }
    }
}
