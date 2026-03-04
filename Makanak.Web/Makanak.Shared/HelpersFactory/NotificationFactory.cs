using Makanak.Domain.EnumsHelper.Notification;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Shared.Dto_s.Notification;
using Makanak.Shared.EnumsHelper.Dispute;
using Makanak.Shared.EnumsHelper.Property;

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


        // 2. مرحلة الدفع (Payment Flow)

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

        // 5. التقييمات (Reviews)
        public static CreateNotificationDto ReviewReceived(string ownerId, string tenantName, string propertyName, int reviewId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "تقييم جديد للعقار ⭐",
                Message = $"قام {tenantName} بإضافة تقييم جديد لعقارك '{propertyName}'.",
                ReferenceId = reviewId.ToString(), // عشان يفتح التقييم يشوفه
                NotificationType = NotificationType.ReviewReceived // ضيفها في الـ Enum
            };
        }

        // 6. Admin for new Property
        public static CreateNotificationDto NewPropertyRequest(string adminId, string ownerName, string propertyTitle, int propertyId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId, // رايح للأدمن
                Title = "طلب إضافة عقار جديد 🏠",
                Message = $"المالك {ownerName} أضاف عقار '{propertyTitle}' وينتظر الموافقة.",
                ReferenceId = propertyId.ToString(), // عشان الأدمن يدوس يفتح صفحة العقار
                NotificationType = NotificationType.NewPropertyListing
            };
        }

        // Admin For Identity
        public static CreateNotificationDto DocumentVerificationRequest(string adminId, string userName, string requesterId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId, // رايح للأدمن
                Title = "طلب توثيق حساب 🆔",
                Message = $"المستخدم {userName} قام برفع مستندات الهوية وينتظر المراجعة.",
                ReferenceId = requesterId, // عشان الأدمن يفتح بروفايل اليوزر
                NotificationType = NotificationType.DocumentVerificationRequest
            };
        }

        public static CreateNotificationDto PropertyStatusUpdate(string ownerId, string propertyTitle, PropertyStatus status, string? reason = null)
        {
            // تحديد العنوان والرسالة بناءً على الحالة
            string title = status switch
            {
                PropertyStatus.Accepted => "تمت الموافقة على عقارك ✅",
                PropertyStatus.Rejected => "تم رفض العقار ❌",
                PropertyStatus.Banned => "تم حظر العقار ⛔",
                PropertyStatus.Pending => "تم تعليق العقار للمراجعة ⏳",
                _ => "تحديث في حالة العقار 🔔"
            };

            string message = status switch
            {
                PropertyStatus.Accepted => $"عقارك '{propertyTitle}' متاح الآن للحجز.",
                PropertyStatus.Rejected => $"تم رفض عقارك '{propertyTitle}'. السبب: {reason ?? "غير محدد"}",
                PropertyStatus.Banned => $"تم حظر عقارك '{propertyTitle}' لمخالفة الشروط. السبب: {reason}",
                PropertyStatus.Pending => $"تم إعادة عقارك '{propertyTitle}' لحالة المراجعة.",
                _ => $"تم تغيير حالة عقارك '{propertyTitle}' إلى {status}."
            };

            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = title,
                Message = message,
                ReferenceId = "",
                NotificationType = NotificationType.PropertyStatusChanged
            };
        }

        // دالة عامة لتحديث حالة المستخدم
        public static CreateNotificationDto UserStatusUpdate(string userId, UserStatus status, string? reason = null)
        {
            string title = status switch
            {
                UserStatus.Active => "تم توثيق/تفعيل حسابك ✅",
                UserStatus.Rejected => "فشل توثيق الحساب ❌",
                UserStatus.Banned => "تم حظر حسابك ⛔",
                UserStatus.Pending => "حسابك قيد المراجعة ⏳",
                _ => "تحديث في حالة الحساب 🔔"
            };

            string message = status switch
            {
                UserStatus.Active => "حسابك نشط الآن ويمكنك استخدام كافة المميزات.",
                UserStatus.Rejected => $"تم رفض المستندات المقدمة. السبب: {reason}",
                UserStatus.Banned => $"تم حظر حسابك لمخالفة القوانين. السبب: {reason}",
                UserStatus.Pending => "جاري مراجعة بيانات حسابك من قبل الإدارة.",
                _ => $"حالة حسابك الآن هي: {status}."
            };

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                ReferenceId = userId,
                NotificationType = NotificationType.UserStatusChanged
            };
        }
       
        public static CreateNotificationDto StrikeAdded(string userId, int currentStrikes, bool isBanned)
        {
            string title = isBanned ? "Account Suspended 🚫" : "Warning: Strike Received ⚠️";
            string message = isBanned
                ? "Your account has been suspended due to receiving 3 strikes."
                : $"You have received a strike from the administration. You currently have {currentStrikes}/3 strikes.";

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                ReferenceId = userId,
                NotificationType = NotificationType.StrikeAdded
            };
        }

        public static CreateNotificationDto StrikeRemoved(string userId, int currentStrikes, bool isUnbanned)
        {
            string title = isUnbanned ? "Account Reactivated ✅" : "Strike Removed ✨";
            string message = isUnbanned
                ? "A strike was removed and your account has been reactivated!"
                : $"A strike was removed from your account. You currently have {currentStrikes}/3 strikes.";

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                ReferenceId = userId,
                NotificationType = NotificationType.StrikeRemoved
            };
        }
        
        public static CreateNotificationDto NewDisputeCreated(string adminId, string complainantName, int bookingId, int disputeId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId,
                Title = "New Dispute Opened ⚠️",
                Message = $"User '{complainantName}' has raised a dispute regarding Booking #{bookingId}. Action required.",
                ReferenceId = disputeId.ToString(),
                NotificationType = NotificationType.DisputeOpened
            };
        }

        public static CreateNotificationDto DisputeConcluded(string userId, int bookingId, int disputeId, DisputeStatus decision, string adminComment)
        {
            string statusText = decision == DisputeStatus.Resolved ? "Resolved" : "Rejected";

            string icon = decision == DisputeStatus.Resolved ? "✅" : "❌";

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = $"Dispute Update: {statusText} {icon}",
                Message = $"The dispute regarding Booking #{bookingId} has been closed as '{statusText}'.\nAdmin Comment: {adminComment}",
                ReferenceId = disputeId.ToString(), 
                NotificationType = NotificationType.DisputeResolved
            };
        }

        public static CreateNotificationDto DisputeCancelled(string adminId, string complainantName, int bookingId, int disputeId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId, // رايح للأدمن
                Title = "Dispute Cancelled 🗑️",
                Message = $"User '{complainantName}' has cancelled the dispute regarding Booking #{bookingId}. No further action is required.",
                ReferenceId = disputeId.ToString(),
                NotificationType = NotificationType.DisputeCancelled
            };
        }
        // ---7 . Background Services 
        public static CreateNotificationDto PaymentDeadlineWarning(string tenantId, int bookingId, int minutesLeft)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "تنبيه هام: مهلة الدفع قاربت على الانتهاء ⏳",
                Message = $"متبقي أقل من {minutesLeft} دقيقة لإلغاء حجزك تلقائياً. يرجى الدفع الآن.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.PaymentWarning 
            };
        }

        // 2. تذكير بموعد الوصول (قبلها بيوم)
        public static CreateNotificationDto CheckInReminder(string tenantId, string propertyTitle, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "رحلتك تبدأ غداً! 🧳",
                Message = $"نتمنى لك إقامة سعيدة في '{propertyTitle}'. تأكد من مراجعة تفاصيل الوصول.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.CheckInReminder 
            };
        }
    }
}
