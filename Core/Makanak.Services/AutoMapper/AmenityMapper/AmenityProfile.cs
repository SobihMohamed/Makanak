using AutoMapper;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.AmenityMapper
{
    public class AmenityProfile : Profile
    {
        public AmenityProfile()
        {
            CreateMap<Amenity, AmenityDto>();
        }
    }
}
