using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class ComponentCompatibility
{
    public int ComponentId { get; set; }

    public int CompatibleWithId { get; set; }

    public bool IsCompatible { get; set; }

    public virtual CarComponent CompatibleWith { get; set; } = null!;

    public virtual CarComponent Component { get; set; } = null!;
}
