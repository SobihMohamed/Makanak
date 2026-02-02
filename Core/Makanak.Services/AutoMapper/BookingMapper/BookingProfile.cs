using AutoMapper;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.EnumsHelper.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.BookingMapper
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
            .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.Title))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name))
            .ForMember(d => d.TenantImage, o => o.MapFrom(s => s.Tenant.ProfilePictureUrl))
            .ForMember(d => d.CommissionPaid, o => o.MapFrom(s => s.CommissionPaid))
            .ForMember(d => d.PropertyMainImage, o => o.MapFrom(s => s.Property.MainImageUrl));

            CreateMap<Booking, BookingDetailDto>()
                .IncludeBase<Booking, BookingDto>()
                .ForMember(dest => dest.CheckInQrCode, opt => opt.MapFrom(src =>
                    (src.Status == BookingStatus.Confirmed || src.Status == BookingStatus.Completed)
                    ? src.CheckInQrCode
                    : null)) 
                .ForMember(d => d.GalleryImages, o => o.MapFrom(s =>
                    s.Property.PropertyImages.Select(x => x.ImageUrl).ToList()));

            CreateMap<CreateBookingDto, Booking>();
        }
    }
}
