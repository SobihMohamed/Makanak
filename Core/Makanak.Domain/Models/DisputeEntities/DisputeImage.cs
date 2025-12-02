using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Models.DisputeEntities
{
    public class DisputeImage : BaseEntity<int>
    {
        public string ImageUrl { get; set; } = null!;
        public int DisputeId { get; set; }
        public virtual Dispute Dispute { get; set; } = null!;
    }
}
