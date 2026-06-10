using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class ApplicationUser
{
    public int IdApplicationUser { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PwdHash { get; set; } = null!;

    public string PwdSalt { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<UserConfiguration> UserConfigurations { get; set; } = new List<UserConfiguration>();
}
