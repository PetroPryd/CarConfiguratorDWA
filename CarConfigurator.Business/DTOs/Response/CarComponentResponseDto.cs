using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Response
{
    public class CarComponentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }

        public int ComponentTypeId { get; set; }
        public string ComponentTypeName { get; set; } = null!;
    }
}
