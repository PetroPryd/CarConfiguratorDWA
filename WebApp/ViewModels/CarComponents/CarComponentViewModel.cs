namespace WebApp.ViewModels.CarComponents
{
    public class CarComponentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public int ComponentTypeId { get; set; }
        public string ComponentTypeName { get; set; } = null!;
    }
}
