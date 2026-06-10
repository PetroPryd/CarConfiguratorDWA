using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Create
{
    public class CarComponentCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1_000_000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Component type is required.")]
        public int ComponentTypeId { get; set; }
    }
}
