using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.PropertyEntities
{
    public class PropertyImage : BaseEntity<int>
    {
        public string ImageUrl { get; set; } = null!;
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;
    }
}
