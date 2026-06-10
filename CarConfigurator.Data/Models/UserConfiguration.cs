using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class UserConfiguration
{
    public int IdUserConfiguration { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public int CarModelId { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CarModel CarModel { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<CarComponent> CarComponents { get; set; } = new List<CarComponent>();
}
