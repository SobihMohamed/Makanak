using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class AdminPropertyDto : PropertyDto
    {
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
    }
}
