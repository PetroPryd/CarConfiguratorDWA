using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Response
{
    public class ConfigurationComponentDto
    {
        public string Name { get; set; } = null!;
        public string ComponentTypeName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
