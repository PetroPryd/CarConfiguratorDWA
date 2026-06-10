using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
