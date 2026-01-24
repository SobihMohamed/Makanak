using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class PropertyDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MainImageUrl { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int MaxGuests { get; set; }
        public string Address { get; set; } = null!;
        public string PropertyStatus { get; set; } = null!;
        public string PropertyType { get; set; } = null!;

        
        public string GovernorateName { get; set; } = null!;
        public string AreaName { get; set; } = null!;

        public List<PropertyImageDto> PropertyImages { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
    }
}
