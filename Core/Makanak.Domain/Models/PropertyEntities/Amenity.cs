using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.PropertyEntities
{
    public class Amenity : BaseEntity<int>
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? Icon{ get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
