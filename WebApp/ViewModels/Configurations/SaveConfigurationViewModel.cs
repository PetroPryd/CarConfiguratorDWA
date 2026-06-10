using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Configurations
{
    public class SaveConfigurationViewModel
    {
        [Required(ErrorMessage = "Please name your configuration.")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Configuration name")]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
