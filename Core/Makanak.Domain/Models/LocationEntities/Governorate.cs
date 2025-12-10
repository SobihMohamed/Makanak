using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.LocationEntities
{
    public class Governorate : BaseEntity<int>
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
    }
}
