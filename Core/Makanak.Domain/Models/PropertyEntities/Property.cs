using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Domain.Models.ReviewEntities;
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
        public double AverageRating { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        public PropertyType PropertyType { get; set; }
        public PropertyStatus PropertyStatus { get; set; } = PropertyStatus.Pending;

        public int GovernorateId { get; set; }
        public virtual Governorate Governorate { get; set; } = null!;
        public string AreaName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? RejectedReason { get; set; } = string.Empty;
        public string OwnerId { get; set; } = null!;
        public virtual ApplicationUser Owner { get; set; } = null!;
        public decimal CommissionPercentage { get; set; } = 10; // Default commission is 10%

        public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
