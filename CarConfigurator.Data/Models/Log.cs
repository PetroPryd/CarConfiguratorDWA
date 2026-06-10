using System;
using System.Collections.Generic;

namespace CarConfigurator.Data.Models;

public partial class Log
{
    public int IdLog { get; set; }

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;
}
