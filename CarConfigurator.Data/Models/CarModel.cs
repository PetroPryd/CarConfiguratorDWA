using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class CarModel
{
    public int IdCarModel { get; set; }

    public string Name { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public int Year { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<UserConfiguration> UserConfigurations { get; set; } = new List<UserConfiguration>();

    public virtual ICollection<CarComponent> CarComponents { get; set; } = new List<CarComponent>();
}
