using Makanak.Domain.EnumsHelper.Notification;
using Makanak.Shared.Dto_s.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.HelpersFactory
{
    public static class NotificationFactory
    {
        public static CreateNotificationDto BookingRequest(string ownerId, string tenantName, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "طلب حجز جديد 🏠",
                Message = $"المستخدم {tenantName} طلب حجز عقارك '{propertyName}'. يرجى اتخاذ إجراء.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingRequest
            };
        }

        // لما المالك يوافق (PendingPayment) -> ابعت للمستأجر يدفع
        public static CreateNotificationDto BookingApprovedForPayment(string tenantId, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "تمت الموافقة على طلبك! ✅",
                Message = $"وافق مالك '{propertyName}' على طلبك. أمامك 20 دقيقة لإتمام الدفع وتأكيد الحجز.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingApproved
            };
        }

        // لما المالك يرفض (RejectedByOwner) -> ابعت للمستأجر
        public static CreateNotificationDto BookingRejected(string tenantId, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "عذراً، تم رفض الطلب ❌",
                Message = $"للأسف، مالك العقار '{propertyName}' غير متاح حالياً لاستقبال هذا الحجز.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingRejected // ضيفها في الـ Enum
            };
        }

        // --------------------------------------------------------
        // 2. مرحلة الدفع (Payment Flow) - اللحظة الحاسمة
        // --------------------------------------------------------

        // (PaymentReceived) -> للمستأجر (إظهار التفاصيل)
        public static CreateNotificationDto PaymentSuccess_ToTenant(string tenantId, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "تم تأكيد الحجز بنجاح! 🎉",
                Message = $"تم الدفع بنجاح لـ '{propertyName}'. الآن يمكنك رؤية العنوان ورقم المالك.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingConfirmed // ضيفها
            };
        }

        // (PaymentReceived) -> للمالك (فلوسك جاية)
        public static CreateNotificationDto PaymentSuccess_ToOwner(string ownerId, string tenantName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "حجز مؤكد جديد 💰",
                Message = $"قام {tenantName} بدفع العربون وتأكيد الحجز. تواصل معه للترتيب.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingConfirmed
            };
        }

        // (PaymentFailed) -> للمستأجر
        public static CreateNotificationDto PaymentFailed(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "فشل في عملية الدفع ⚠️",
                Message = "لم نتمكن من إتمام الدفع. يرجى المحاولة مرة أخرى قبل انتهاء المهلة.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.PaymentFailed // ضيفها
            };
        }

        // --------------------------------------------------------
        // 3. الإلغاء وانتهاء الوقت (Cancellation)
        // --------------------------------------------------------

        // لما حد يلغي للتاني
        public static CreateNotificationDto BookingCancelled(string targetUserId, string cancelledByWho, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = targetUserId,
                Title = "تم إلغاء الحجز 🚫",
                Message = $"قام {cancelledByWho} بإلغاء الحجز رقم {bookingId}.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCancelled
            };
        }

        // لما الوقت يخلص (Auto Cancel)
        public static CreateNotificationDto BookingExpired(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "انتهت مهلة الدفع ⏳",
                Message = $"تم إلغاء الحجز رقم {bookingId} تلقائياً لعدم السداد في الوقت المحدد.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCancelled
            };
        }

        // --------------------------------------------------------
        // 4. التشيك إن والانتهاء (Check-In / Completed)
        // --------------------------------------------------------

        public static CreateNotificationDto BookingCompleted(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "كيف كانت إقامتك؟ ⭐",
                Message = "نرجو أن تكون استمتعت بإقامتك. لا تنسَ تقييم العقار!",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCompleted
            };
        }
    }
}
