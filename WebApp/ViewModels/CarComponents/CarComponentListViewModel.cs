using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.CarComponents
{
    public class CarComponentListViewModel
    {
        public List<CarComponentViewModel> Items { get; set; } = new();

        public string? Search { get; set; }
        public int? ComponentTypeId { get; set; }
        public SelectList? ComponentTypes { get; set; }

        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

}
