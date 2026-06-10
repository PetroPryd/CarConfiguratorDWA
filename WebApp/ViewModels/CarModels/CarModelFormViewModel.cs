using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.CarModels
{
    public class CarModelFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Manufacturer is required.")]
        [StringLength(100)]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; } = null!;

        [Required(ErrorMessage = "Base price is required.")]
        [Range(0, 10_000_000, ErrorMessage = "Base price must be between 0 and 10,000,000.")]
        [Display(Name = "Base price (€)")]
        public decimal BasePrice { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Image URL (optional)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload image")]
        public IFormFile? ImageFile { get; set; }
    }
}
