using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.ComponentTypes
{
    public class ComponentTypeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
