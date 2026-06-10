using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.CarComponents
{
    public class CarComponentFormViewModel
    {
        public int Id { get; set; } // hidden field, never displayed

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be 2–150 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1_000_000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
        [Display(Name = "Price (€)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "Image URL (optional)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload image")]
        public IFormFile? ImageFile { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        [Display(Name = "Stock quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Available for selection")]
        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Please select a component type.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a component type.")]
        [Display(Name = "Component type")]
        public int ComponentTypeId { get; set; }

        public IEnumerable<SelectListItem> ComponentTypes { get; set; } = new List<SelectListItem>();
    }
}
