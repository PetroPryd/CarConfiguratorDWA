using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class ComponentType
{
    public int IdComponentType { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CarComponent> CarComponents { get; set; } = new List<CarComponent>();
}
