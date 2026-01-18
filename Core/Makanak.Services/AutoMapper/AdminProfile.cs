using AutoMapper;
using Makanak.Domain.Models.Identity;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Shared.Dto_s.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            // Add your mappings here
            CreateMap<ApplicationUser, UserForApprovalDto>()
                .ForMember(Dest => Dest.UserId, options => options.MapFrom(src => src.Id))
                .ForMember(Dest => Dest.JoinAt, options => options.MapFrom(src => src.CreatedAt))
                .ForMember(Dest => Dest.UserStatus , options=> options.MapFrom(src=>src.UserStatus.ToString()))
                .ForMember(Dest => Dest.UserType , options=> options.MapFrom(src=>src.UserType.ToString()));

            CreateMap<ApplicationUser, UserVerificationDetailsDto>()
                .IncludeBase<ApplicationUser, UserForApprovalDto>()
                .ForMember(Dest => Dest.ProfilePictureUrl, options => options.MapFrom<UrlResolver<ApplicationUser, UserVerificationDetailsDto>, string>(src => src.ProfilePictureUrl))
                .ForMember(Dest => Dest.NationalIdImageFrontUrl , options => options.MapFrom<UrlResolver<ApplicationUser,UserVerificationDetailsDto>,string>(src => src.NationalIdImageFrontUrl))
                .ForMember(Dest => Dest.NationalIdImageBackUrl , options => options.MapFrom<UrlResolver<ApplicationUser,UserVerificationDetailsDto>,string>(src => src.NationalIdImageBackUrl));
        }

    }
}
