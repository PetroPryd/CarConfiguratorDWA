using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3–50 characters.")]
        [Display(Name = "Username")]
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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please repeat your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repeat password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
