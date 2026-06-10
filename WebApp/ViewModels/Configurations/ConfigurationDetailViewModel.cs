namespace WebApp.ViewModels.Configurations
{
    public class ConfigurationDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CarModelName { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConfigurationComponentViewModel> Components { get; set; } = new();
    }
}
