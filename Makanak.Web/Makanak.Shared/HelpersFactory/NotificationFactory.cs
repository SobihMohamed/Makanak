using Makanak.Domain.EnumsHelper.Notification;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Shared.Dto_s.Notification;
using Makanak.Shared.EnumsHelper.Dispute;
using Makanak.Shared.EnumsHelper.Property;

namespace Makanak.Shared.HelpersFactory
{
    public static class NotificationFactory
    {
        // --------------------------------------------------------
        // 1. مرحلة الطلب والموافقة (Booking Requests)
        // --------------------------------------------------------

        public static CreateNotificationDto BookingRequest(string ownerId, string tenantName, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "طلب حجز جديد 🏠",
                Message = $"المستخدم '{tenantName}' أرسل طلباً لحجز عقارك '{propertyName}'. يرجى مراجعة الطلب.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingRequest
            };
        }

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

        public static CreateNotificationDto BookingRejected(string tenantId, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "عذراً، تم رفض الطلب ❌",
                Message = $"للأسف، مالك العقار '{propertyName}' غير متاح حالياً لاستقبال هذا الحجز.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingRejected
            };
        }

        // --------------------------------------------------------
        // 2. مرحلة الدفع (Payment Flow)
        // --------------------------------------------------------

        public static CreateNotificationDto PaymentSuccess_ToTenant(string tenantId, string propertyName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "تم تأكيد الحجز بنجاح! 🎉",
                Message = $"تم الدفع بنجاح لعقار '{propertyName}'. يمكنك الآن رؤية العنوان بالكامل ورقم تواصل المالك.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingConfirmed
            };
        }

        public static CreateNotificationDto PaymentSuccess_ToOwner(string ownerId, string tenantName, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "حجز مؤكد جديد 💰",
                Message = $"قام '{tenantName}' بدفع العربون وتأكيد الحجز. يرجى التواصل معه للترتيب.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingConfirmed
            };
        }

        public static CreateNotificationDto PaymentFailed(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "فشل في عملية الدفع ⚠️",
                Message = "لم نتمكن من إتمام الدفع الخاص بحجزك الأخير. يرجى المحاولة مرة أخرى قبل انتهاء المهلة.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.PaymentFailed
            };
        }

        // --------------------------------------------------------
        // 3. الإلغاء وانتهاء الوقت (Cancellation)
        // --------------------------------------------------------

        public static CreateNotificationDto BookingCancelled(string targetUserId, string cancelledByWho, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = targetUserId,
                Title = "تم إلغاء الحجز 🚫",
                Message = $"قام '{cancelledByWho}' بإلغاء هذا الحجز. تم تحديث حالة الطلب.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCancelled
            };
        }

        public static CreateNotificationDto BookingExpired(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "انتهت مهلة الدفع ⏳",
                Message = "تم إلغاء طلب الحجز الخاص بك تلقائياً نظراً لعدم إتمام عملية السداد في الوقت المحدد.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCancelled
            };
        }

        public static CreateNotificationDto RefundStatusNotification(string tenantId, bool isRefunded, int bookingId)
        {
            string title = isRefunded ? "تم استرداد العربون 💸" : "إلغاء بدون استرداد ℹ️";

            string message = isRefunded
                ? "تم معالجة طلب الإلغاء وتطبيق سياسة الاسترجاع بنجاح. سيتم إرجاع المبلغ المستحق إلى حسابك خلال 7 إلى 14 يوم عمل حسب سياسة البنك."
                : "تم معالجة طلب الإلغاء. نعتذر، لا يحق لك استرداد العربون بناءً على سياسة الإلغاء الخاصة بالمنصة لتجاوز الوقت المسموح به للإلغاء المجاني.";

            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = title,
                Message = message,
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCancelled
            };
        }
        // --------------------------------------------------------
        // 4. التشيك إن والانتهاء والتقييم (Check-In & Reviews)
        // --------------------------------------------------------

        public static CreateNotificationDto BookingCompleted(string tenantId, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "كيف كانت إقامتك؟ ⭐",
                Message = "نرجو أن تكون قد استمتعت بإقامتك. لا تنسَ تقييم العقار لمساعدة الآخرين!",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.BookingCompleted
            };
        }

        public static CreateNotificationDto ReviewReceived(string ownerId, string tenantName, string propertyName, int reviewId)
        {
            return new CreateNotificationDto
            {
                UserId = ownerId,
                Title = "تقييم جديد للعقار ⭐",
                Message = $"قام '{tenantName}' بإضافة تقييم جديد لعقارك '{propertyName}'.",
                ReferenceId = reviewId.ToString(),
                NotificationType = NotificationType.ReviewReceived
            };
        }

        // --------------------------------------------------------
        // 5. إشعارات الأدمن والمراجعات (Admin & Approvals)
        // --------------------------------------------------------

        public static CreateNotificationDto NewPropertyRequest(string adminId, string ownerName, string propertyTitle, int propertyId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId,
                Title = "طلب إضافة عقار جديد 🏠",
                Message = $"المالك '{ownerName}' أضاف عقاراً جديداً بعنوان '{propertyTitle}' وينتظر المراجعة والموافقة.",
                ReferenceId = propertyId.ToString(),
                NotificationType = NotificationType.NewPropertyListing
            };
        }

        public static CreateNotificationDto DocumentVerificationRequest(string adminId, string userName, string requesterId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId,
                Title = "طلب توثيق حساب 🆔",
                Message = $"المستخدم '{userName}' قام برفع مستندات الهوية وينتظر المراجعة لتوثيق الحساب.",
                ReferenceId = requesterId,
                NotificationType = NotificationType.DocumentVerificationRequest
            };
        }

        public static CreateNotificationDto PropertyStatusUpdate(string ownerId, string propertyTitle, PropertyStatus status, string? reason = null)
        {
            string title = status switch
            {
                PropertyStatus.Accepted => "تمت الموافقة على عقارك ✅",
                PropertyStatus.Rejected => "تم رفض العقار ❌",
                PropertyStatus.Banned => "تم حظر العقار ⛔",
                PropertyStatus.Pending => "عقارك قيد المراجعة ⏳",
                _ => "تحديث في حالة العقار 🔔"
            };

            string message = status switch
            {
                PropertyStatus.Accepted => $"عقارك '{propertyTitle}' متاح الآن للحجز والظهور للمستخدمين.",
                PropertyStatus.Rejected => $"تم رفض نشر عقارك '{propertyTitle}'. السبب: {reason ?? "غير محدد"}",
                PropertyStatus.Banned => $"تم إخفاء وحظر عقارك '{propertyTitle}' لمخالفة الشروط. السبب: {reason}",
                PropertyStatus.Pending => $"تمت إعادة عقارك '{propertyTitle}' لحالة المراجعة من قبل الإدارة.",
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

        public static CreateNotificationDto UserStatusUpdate(string userId, UserStatus status, string? reason = null)
        {
            string title = status switch
            {
                UserStatus.Active => "تم توثيق حسابك ✅",
                UserStatus.Rejected => "فشل توثيق الحساب ❌",
                UserStatus.Banned => "تم حظر حسابك ⛔",
                UserStatus.Pending => "حسابك قيد المراجعة ⏳",
                _ => "تحديث في حالة الحساب 🔔"
            };

            string message = status switch
            {
                UserStatus.Active => "حسابك نشط وموثق الآن، يمكنك استخدام كافة مميزات المنصة بحرية.",
                UserStatus.Rejected => $"تم رفض مستندات التوثيق المقدمة. السبب: {reason}",
                UserStatus.Banned => $"تم حظر حسابك لمخالفة قوانين المنصة. السبب: {reason}",
                UserStatus.Pending => "جاري مراجعة بيانات هويتك من قبل الإدارة لتأكيد التوثيق.",
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

        // --------------------------------------------------------
        // 6. نظام الإنذارات (Strikes)
        // --------------------------------------------------------

        public static CreateNotificationDto StrikeAdded(string userId, int currentStrikes, bool isBanned)
        {
            string title = isBanned ? "تم إيقاف الحساب 🚫" : "تحذير: تلقيت إنذاراً إدارياً ⚠️";
            string message = isBanned
                ? "تم إيقاف حسابك مؤقتاً نظراً لتلقيك 3 إنذارات متتالية لمخالفة سياسات المنصة."
                : $"لقد تلقيت إنذاراً من الإدارة. رصيدك الحالي هو {currentStrikes} من أصل 3 إنذارات مسموح بها.";

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
            string title = isUnbanned ? "تم إعادة تفعيل حسابك ✅" : "تمت إزالة إنذار ✨";
            string message = isUnbanned
                ? "تمت إزالة أحد الإنذارات وإعادة تفعيل حسابك بنجاح. أهلاً بك مجدداً!"
                : $"تمت إزالة إنذار من سجل حسابك. رصيدك الحالي هو {currentStrikes} من أصل 3 إنذارات.";

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                ReferenceId = userId,
                NotificationType = NotificationType.StrikeRemoved
            };
        }

        // --------------------------------------------------------
        // 7. نظام النزاعات والمشاكل (Disputes)
        // --------------------------------------------------------

        public static CreateNotificationDto NewDisputeCreated(string adminId, string complainantName, int bookingId, int disputeId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId,
                Title = "تم فتح نزاع جديد ⚠️",
                Message = $"قام المستخدم '{complainantName}' بفتح نزاع بخصوص حجز سابق. يرجى التدخل والمراجعة.",
                ReferenceId = disputeId.ToString(),
                NotificationType = NotificationType.DisputeOpened
            };
        }

        public static CreateNotificationDto DisputeConcluded(string userId, int bookingId, int disputeId, DisputeStatus decision, string adminComment)
        {
            string statusText = decision == DisputeStatus.Resolved ? "تم حله" : "مرفوض";
            string icon = decision == DisputeStatus.Resolved ? "✅" : "❌";

            return new CreateNotificationDto
            {
                UserId = userId,
                Title = $"تحديث بخصوص النزاع: {statusText} {icon}",
                Message = $"تم إنهاء النزاع الخاص بك واتخاذ القرار بأنه '{statusText}'.\nرد الإدارة: {adminComment}",
                ReferenceId = disputeId.ToString(),
                NotificationType = NotificationType.DisputeResolved
            };
        }

        public static CreateNotificationDto DisputeCancelled(string adminId, string complainantName, int bookingId, int disputeId)
        {
            return new CreateNotificationDto
            {
                UserId = adminId,
                Title = "تم إلغاء النزاع 🗑️",
                Message = $"قام المستخدم '{complainantName}' بإلغاء النزاع الذي فتحه مؤخراً. لا توجد أي إجراءات إضافية مطلوبة.",
                ReferenceId = disputeId.ToString(),
                NotificationType = NotificationType.DisputeCancelled
            };
        }

        // --------------------------------------------------------
        // 8. خدمات التنبيهات التلقائية (Background Services)
        // --------------------------------------------------------

        public static CreateNotificationDto PaymentDeadlineWarning(string tenantId, int bookingId, int minutesLeft)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "تنبيه هام: مهلة الدفع قاربت على الانتهاء ⏳",
                Message = $"متبقي أقل من {minutesLeft} دقيقة لإلغاء طلب حجزك تلقائياً. يرجى إتمام الدفع الآن لضمان الحجز.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.PaymentWarning
            };
        }

        public static CreateNotificationDto CheckInReminder(string tenantId, string propertyTitle, int bookingId)
        {
            return new CreateNotificationDto
            {
                UserId = tenantId,
                Title = "رحلتك تبدأ غداً! 🧳",
                Message = $"نتمنى لك إقامة سعيدة ومريحة في '{propertyTitle}'. تأكد من مراجعة تفاصيل العنوان والوصول.",
                ReferenceId = bookingId.ToString(),
                NotificationType = NotificationType.CheckInReminder
            };
        }
    }
}