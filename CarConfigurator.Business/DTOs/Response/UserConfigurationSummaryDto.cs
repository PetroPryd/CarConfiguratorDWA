using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Response
{
    public class UserConfigurationSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CarModelName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ComponentCount { get; set; }
    }
}
