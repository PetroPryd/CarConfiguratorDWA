using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Profile
{
    // Username is read-only
    public class ProfileViewModel
    {
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Must be a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Must be a valid phone number.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
    }
}
