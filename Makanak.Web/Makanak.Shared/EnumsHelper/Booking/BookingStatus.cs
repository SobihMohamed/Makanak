using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Booking
{
    public enum BookingStatus
    {
        // 1. الحالة المبدئية: المستأجر اختار الشقة بس لسه مدفعش العربون
        PendingPayment = 1,

        // 2. الحالة المؤكدة: دفع العربون بنجاح (وظهرت له بيانات المالك)
        Confirmed = 2,

        // 3. (اختياري) لو المالك رفض الطلب (لو عاملين نظام موافقة يدوية)
        Rejected = 3,

        // 4. الإلغاء: سواء من المالك أو المستأجر بعد الدفع
        Cancelled = 4,

        // 5. الوصول: لما المستأجر يوصل ويعمل Scan للـ QR Code عند المالك
        CheckedIn = 5,

        // 6. الانتهاء: مدة الحجز خلصت والمستأجر مشي
        Completed = 6,

        // 7. نزاع: حصلت مشكلة وتم فتح شكوى (عشان نوقف أي تحويلات أو تقييمات مؤقتاً)
        Disputed = 7
    }
}
