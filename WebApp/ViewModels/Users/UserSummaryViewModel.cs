namespace WebApp.ViewModels.Users
{
    public class UserSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int ConfigurationCount { get; set; }
    }
}
