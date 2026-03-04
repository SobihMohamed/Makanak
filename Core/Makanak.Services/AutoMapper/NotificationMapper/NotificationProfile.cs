using AutoMapper;
using Makanak.Domain.EnumsHelper.Notification;
using Makanak.Domain.Models.NotifyEnities;
using Makanak.Shared.Dto_s.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.NotificationMapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<CreateNotificationDto, Notification>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.SenderId, opt => opt.Ignore()) // أو null مبدئياً
            .ForMember(dest => dest.NotificationType, opt => opt.MapFrom(src => src.NotificationType.ToString()));
           

            CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.NotificationType, opt => opt.MapFrom(src => src.NotificationType.ToString()))

            // Sender Name Logic
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
            
            string.IsNullOrEmpty(src.SenderId) ? "Makanak System" :

            src.Sender == null ? "User" :

            $"{src.Sender.UserType} : {src.Sender.Name}"))

            .ForMember(dest => dest.About, opt => opt.MapFrom(src => GetNotificationContext(src)));
        }
        private static string GetNotificationContext(Notification src)
        {
            return src.NotificationType switch
            {
                // Booking
                NotificationType.BookingRequest => $"New Request #{src.ReferenceId}",
                NotificationType.BookingApproved => $"Booking #{src.ReferenceId} Approved",
                NotificationType.BookingCancelled => $"Booking #{src.ReferenceId} Cancelled",

                // Payment
                NotificationType.PaymentReceiptUploaded => $"Receipt Review #{src.ReferenceId}",
                NotificationType.PaymentApproved => $"Payment Confirmed #{src.ReferenceId}",
                NotificationType.PaymentRejected => $"Payment Rejected #{src.ReferenceId}",

                // Disputes
                NotificationType.DisputeOpened => $"Dispute Case #{src.ReferenceId}",

                // Default
                _ => "Notification"
            };
        }
    }
}
