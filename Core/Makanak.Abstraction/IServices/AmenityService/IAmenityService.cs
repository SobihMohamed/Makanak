using Makanak.Shared.Dto_s.Governorate;
using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.AmenityService
{
    public interface IAmenityService
    {
        Task<IReadOnlyList<AmenityDto>> GetAllAmenitiesAsync();
    }
}
