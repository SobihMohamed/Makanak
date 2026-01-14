using AutoMapper;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Models.Identity;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.User;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(Dest => Dest.UserName , options => options.MapFrom(src => src.Email))
                .ForMember(Dest => Dest.UserStatus , options => options.MapFrom(src => UserStatus.Pending.ToString()))
                .ForMember(Dest => Dest.CreatedAt , options => options.MapFrom(src => DateTime.UtcNow));

            CreateMap<ApplicationUser, CurrentUserDto>()
                .ForMember(Dest => Dest.ProfilePictureUrl , options => options.MapFrom<UrlResolver<ApplicationUser,CurrentUserDto>, string>(src => src.ProfilePictureUrl))
                .ForMember(Dest => Dest.UserType, options => options.MapFrom(src => src.UserType.ToString()))
                .ForMember(Dest => Dest.UserStatus, options => options.MapFrom(src => src.UserStatus.ToString()))
                .ForMember(Dest => Dest.JoinAt, options => options.MapFrom(src => src.CreatedAt))
                .ForMember(Dest => Dest.Age, options => options.MapFrom(src => CalculateAge(src.DateOfBirth)));

            CreateMap<UpdateProfileDto, ApplicationUser>()
                .ForMember(Dest => Dest.ProfilePictureUrl, options => options.Ignore()) // when update database ignore Picture Because we handel it in class
                .ForAllMembers(Options => Options.Condition((src, dest, srcMember) => srcMember != null)); // to avoid replace by nulls



        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
