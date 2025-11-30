using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.PropertyEntities
{
    public class Property : BaseEntity<int>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? MainImageUrl { get; set; }
        public decimal PricePerNight { get; set; }
        public int Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms{ get; set; }
        public int MaxGuests { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        public PropertyType PropertyType { get; set; }
        
        public int GovernorateId { get; set; }
        public virtual Governorate Governorate { get; set; } = null!;
        public string AreaName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string OwnerId { get; set; } = null!;
        public virtual ApplicationUser Owner { get; set; } = null!;
       
        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        //public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
