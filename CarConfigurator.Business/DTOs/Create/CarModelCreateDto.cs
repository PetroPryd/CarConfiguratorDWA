using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Create
{
    public class CarModelCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Manufacturer is required.")]
        [StringLength(100)]
        public string Manufacturer { get; set; } = null!;

        [Required(ErrorMessage = "Base price is required.")]
        [Range(0, 10_000_000, ErrorMessage = "Base price must be between 0 and 10,000,000.")]
        public decimal BasePrice { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
