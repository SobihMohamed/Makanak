using AutoMapper;
using Makanak.Domain.Models.BookingEntities;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Shared.Dto_s.Booking;
using Makanak.Shared.Dto_s.Property;
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
            .ForMember(d => d.PropertyMainImage, o => o.MapFrom<UrlResolver<Booking, BookingDto>, string>(s => s.Property.MainImageUrl));

            CreateMap<Booking, ScanQrResponseDto>()
             .ForMember(d => d.BookingId, o => o.MapFrom(s => s.Id))
             .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
             .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name)) //
             .ForMember(d => d.FrontIdentityImage, o => o.MapFrom(s => s.Tenant.NationalIdImageFrontUrl))
             .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.Title)) //
             .ForMember(d => d.Message, o => o.MapFrom(s => "Welcome! Please verify the tenant's identity before delivery."));


            CreateMap<PropertyImage, PropertyImageDto>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom<UrlResolver<PropertyImage, PropertyImageDto>, string>(s => s.ImageUrl));
           
            // 3. Mapping (TenantBookingDetailsDto)
            // =================================================================
            CreateMap<Booking, TenantBookingDetailsDto>()
                .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.Title))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))

                // الداتا الحساسة بتظهر بس لو الدفع تم
                .ForMember(dest => dest.OwnerPhoneNumber, opt => opt.MapFrom(src =>

                    IsPaid(src.Status) ? src.Owner.PhoneNumber : null)) // تأكد إن src.Owner موجودة أو استخدم src.Property.Owner

                .ForMember(dest => dest.PricePerNight, opt => opt.MapFrom(src => src.PricePerNight))

                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.AmountToPayToOwner))

                .ForMember(dest => dest.PlatformFee, opt => opt.MapFrom(src => src.CommissionPaid))

                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))

                .ForMember(dest => dest.ExactLocationUrl, opt => opt.MapFrom(src =>
                    IsPaid(src.Status) ? $"http://maps.google.com/?q={src.Property.Latitude},{src.Property.Longitude}" : null))

                .ForMember(dest => dest.CheckInInstructions, opt => opt.MapFrom(src =>
                    IsPaid(src.Status) ? "يرجى التواصل مع المالك قبل الوصول بساعة وإظهار الـ QR Code" : "سيتم إظهار التعليمات بعد إتمام الدفع"))

                .ForMember(d => d.PropertyMainImage, o => o.MapFrom<UrlResolver<Booking, TenantBookingDetailsDto>, string>(s => s.Property.MainImageUrl))
                
                .ForMember(d => d.PropertyImages, o => o.MapFrom(s => s.Property.PropertyImages))
                
                .ForMember(dest => dest.CheckInQrCode, opt => opt.MapFrom(src =>
                    IsPaid(src.Status) ? src.CheckInQrCode : null));

            // 4. Mapping (OwnerBookingDetailsDto)

            CreateMap<Booking, OwnerBookingDetailsDto>()
                .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.Title))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PropertyMainImage, o => o.MapFrom<UrlResolver<Booking, OwnerBookingDetailsDto>, string>(s => s.Property.MainImageUrl))

                .ForMember(d => d.PropertyImages, o => o.MapFrom(s => s.Property.PropertyImages))
                // بيانات المستأجر
                .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name))
                .ForMember(d => d.TenantImage, o => o.MapFrom(s => s.Tenant.ProfilePictureUrl))
                .ForMember(d => d.TenantPhoneNumber, o => o.MapFrom(s => s.Tenant.PhoneNumber))

                // صورة البطاقة تظهر للمالك بس لو الحجز اتدفع عشان يطابقها
                .ForMember(d => d.TenantIdentityImage, o => o.MapFrom(s =>
                    IsPaid(s.Status) ? s.Tenant.NationalIdImageFrontUrl : null));

            CreateMap<CreateBookingDto, Booking>();
        }
        private static bool IsPaid(BookingStatus status)
        {
            return status == BookingStatus.PaymentReceived ||
                   status == BookingStatus.CheckedIn || 
                   status == BookingStatus.Completed;
        }
    }

}
