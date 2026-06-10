using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class Image
{
    public int IdImage { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int? CarComponentId { get; set; }

    public int? CarModelId { get; set; }

    public virtual CarComponent? CarComponent { get; set; }

    public virtual CarModel? CarModel { get; set; }
}
