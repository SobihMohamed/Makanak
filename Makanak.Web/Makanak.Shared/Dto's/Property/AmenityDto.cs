using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
    public class AmenityDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? Icon { get; set; }
    }
}
