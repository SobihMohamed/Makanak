using Makanak.Shared.EnumsHelper.Property;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class CreatePropertyDto
    {
        [Required(ErrorMessage = "Property title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Area must be greater than 0")]
        public int Area { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Number of bedrooms must be realistic")]
        public int Bedrooms { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Number of bathrooms must be realistic")]
        public int Bathrooms { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Max guests must be at least 1")]
        public int MaxGuests { get; set; }

        [Required(ErrorMessage = "Property Type is required")]
        public PropertyType PropertyType { get; set; } 

        // 📍 الموقع
        [Required(ErrorMessage = "Governorate is required")]
        public int GovernorateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AreaName { get; set; } = null!; 

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = null!; 

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }


        [Required(ErrorMessage = "Main image is required")]
        public IFormFile? MainImage { get; set; } = null!;
        public List<IFormFile>? GalleryImages { get; set; }
        public List<int>? AmenityIds { get; set; }
    }
}
