using AutoMapper;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Shared.Dto_s.Governorate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.GovernorateMapper
{
    public class GovernorateProfile : Profile
    {
        public GovernorateProfile()
        {
            CreateMap<Governorate, GovernorateDto>();
        }
    }
}
