using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Common.Params.Property_Params
{
    public class PropertyParams : QueryParams
    {
        public int? GovernorateId { get; set; }
        public PropertyType? Type { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public double? MaxDistance { get; set; } = 50; // km 

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public int? MinBedrooms { get; set; }
        public int? MinMaxGuests { get; set; }

        public List<int>? AmenityIds { get; set; } 
    }
}
