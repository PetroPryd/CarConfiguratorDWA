using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class CarComponent
{
    public int IdCarComponent { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public int ComponentTypeId { get; set; }

    public virtual ICollection<ComponentCompatibility> ComponentCompatibilityCompatibleWiths { get; set; } = new List<ComponentCompatibility>();

    public virtual ICollection<ComponentCompatibility> ComponentCompatibilityComponents { get; set; } = new List<ComponentCompatibility>();

    public virtual ComponentType ComponentType { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<CarModel> CarModels { get; set; } = new List<CarModel>();

    public virtual ICollection<UserConfiguration> UserConfigurations { get; set; } = new List<UserConfiguration>();
}
