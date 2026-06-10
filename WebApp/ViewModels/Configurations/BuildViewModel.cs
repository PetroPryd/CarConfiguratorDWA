using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.CarComponents;

namespace WebApp.ViewModels.Configurations
{
    public class BuildViewModel
    {
        public List<CarComponentViewModel> Components { get; set; } = new();
        public decimal ComponentsSubtotal { get; set; }
        public SaveConfigurationViewModel Save { get; set; } = new();

        public int? SelectedModelId { get; set; }
        public string? SelectedModelName { get; set; }
        public decimal? SelectedModelBasePrice { get; set; }

        // For choosing/changing the model from the build page (fallback to the gallery).
        public IEnumerable<SelectListItem> CarModels { get; set; } = new List<SelectListItem>();

        public bool HasModel => SelectedModelId.HasValue;
        public decimal EstimatedTotal => (SelectedModelBasePrice ?? 0) + ComponentsSubtotal;
    }
}
