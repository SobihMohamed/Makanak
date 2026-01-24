using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string MainImageUrl { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string AreaName { get; set; } = null!; 
        public string PropertyStatus { get; set; } = null!; 
        public string PropertyType { get; set; } = null!;   
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
    }
}
