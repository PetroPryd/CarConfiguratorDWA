using WebApp.ViewModels.Configurations;

namespace WebApp.ViewModels.Users
{
    public class UserConfigurationsViewModel
    {
        public UserSummaryViewModel User { get; set; } = null!;
        public List<ConfigurationSummaryViewModel> Configurations { get; set; } = new();
    }
}
