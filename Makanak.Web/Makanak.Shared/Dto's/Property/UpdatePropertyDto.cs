using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Property
{
     public class UpdatePropertyDto : CreatePropertyDto
    {
        public int Id { get; set; }
        public List<int>? AmenityIds { get; set; }
        public List<int>? DeletedImageIds { get; set; }

    }
}
