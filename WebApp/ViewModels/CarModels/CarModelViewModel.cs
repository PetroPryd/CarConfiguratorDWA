namespace WebApp.ViewModels.CarModels
{
    public class CarModelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Manufacturer { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
