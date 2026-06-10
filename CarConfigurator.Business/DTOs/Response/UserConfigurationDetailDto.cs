using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Response
{
    public class UserConfigurationDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CarModelName { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ConfigurationComponentDto> Components { get; set; } = new();
    }
}
