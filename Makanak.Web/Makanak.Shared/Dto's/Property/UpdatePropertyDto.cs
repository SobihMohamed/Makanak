using Makanak.Shared.EnumsHelper.Property;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class UpdatePropertyDto
    {
        [MaxLength(200)]
        public string? Title { get; set; } 

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, double.MaxValue)]
        public decimal? PricePerNight { get; set; } 

        [Range(1, int.MaxValue)]
        public int? Area { get; set; } 

        [Range(0, 50)]
        public int? Bedrooms { get; set; } 

        [Range(0, 50)]
        public int? Bathrooms { get; set; } 

        [Range(1, 100)]
        public int? MaxGuests { get; set; } 

        public PropertyType? PropertyType { get; set; } 

        public int? GovernorateId { get; set; }

        [MaxLength(100)]
        public string? AreaName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public IFormFile? MainImage { get; set; }

        public List<IFormFile>? GalleryImages { get; set; }

        public List<int>? AmenityIds { get; set; }

        public List<int>? DeletedImageIds { get; set; }
    }
}
