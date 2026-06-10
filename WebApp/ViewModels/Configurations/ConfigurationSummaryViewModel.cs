namespace WebApp.ViewModels.Configurations
{
    public class ConfigurationSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CarModelName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ComponentCount { get; set; }
    }
}
