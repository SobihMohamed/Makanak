using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Shared.Dto_s.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.PropertyMapper
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<CreatePropertyDto, Property>()
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyImages, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Property , PropertyDto>()
                .ForMember(dest => dest.PropertyStatus, opt => opt.MapFrom(src => src.PropertyStatus.ToString()))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType.ToString()))
                .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.Governorate.NameEn.ToString()))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom<UrlResolver<Property, PropertyDto>, string>(src => src.MainImageUrl));

            CreateMap<Property, PropertyDetailDto>()
                .ForMember(dest => dest.PropertyStatus, opt => opt.MapFrom(src => src.PropertyStatus.ToString()))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType.ToString()))
                .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.Governorate.NameEn.ToString()))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom<UrlResolver<Property, PropertyDetailDto>, string>(src => src.MainImageUrl));
        
            CreateMap<PropertyImage, PropertyImageDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<UrlResolver<PropertyImage, PropertyImageDto>, string>(src => src.ImageUrl));
           
            CreateMap<Property, AdminPropertyDto>()
            .IncludeBase<Property, PropertyDto>() // بيورث كل اللي فوق
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
            .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.Owner.Email));

            CreateMap<Property, AdminPropertyDetailDto>()
                .IncludeBase<Property, PropertyDetailDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.Owner.Email))
                .ForMember(dest => dest.OwnerPhoneNumber, opt => opt.MapFrom(src => src.Owner.PhoneNumber));

            CreateMap<UpdatePropertyDto, Property>()
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyImages, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyStatus, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
