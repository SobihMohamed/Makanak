using AutoMapper;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.EnumsHelper.Booking;


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

            CreateMap<Booking, ScanQrResponseDto>()
             .ForMember(d => d.BookingId, o => o.MapFrom(s => s.Id))
             .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
             .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name)) //
             .ForMember(d => d.FrontIdentityImage, o => o.MapFrom(s => s.Tenant.NationalIdImageFrontUrl))
             .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.Title)) //
             .ForMember(d => d.Message, o => o.MapFrom(s => "Welcome! Please verify the tenant's identity before delivery."));

            CreateMap<Booking, BookingDetailDto>()
            .IncludeBase<Booking, BookingDto>() 
            .ForMember(d => d.GalleryImages, o => o.MapFrom(s =>
                s.Property.PropertyImages.Select(x => x.ImageUrl).ToList()))
            // (Sensitive Data)
            .ForMember(dest => dest.OwnerPhoneNumber, opt => opt.MapFrom(src =>
                IsPaid(src.Status) ? src.Property.Owner.PhoneNumber : null))

            .ForMember(dest => dest.ExactLocationUrl, opt => opt.MapFrom(src =>
                IsPaid(src.Status)
                ? $"https://www.google.com/maps/search/?api=1&query={src.Property.Latitude},{src.Property.Longitude}"
                : null))

            .ForMember(dest => dest.CheckInInstructions, opt => opt.MapFrom(src =>
                IsPaid(src.Status)
                ? "يرجى التواصل مع المالك قبل الوصول بساعة وإظهار الـ QR Code"
                : "سيتم إظهار التعليمات بعد إتمام الدفع"))

            .ForMember(dest => dest.CheckInQrCode, opt => opt.MapFrom(src =>
                IsPaid(src.Status) ? src.CheckInQrCode : null));

            CreateMap<CreateBookingDto, Booking>();
        }
        private static bool IsPaid(BookingStatus status)
        {
            return status == BookingStatus.PaymentReceived || status == BookingStatus.Completed;
        }
    }

}
