using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Booking
{
    public enum BookingStatus
    {
        //دي الحالة الافتراضية أول ما المستأجر يدوس "احجز"
        PendingOwnerApproval = 0,

        // لو المالك رفض الطلب.
        RejectedByOwner = 1,

        // المالك وافق، ومستنيين المستأجر يدفع العربون.
        PendingPayment = 2,

        // حاول يدفع وفشل (Stripe رجع Error).
        PaymentFailed = 3,


        // (هي دي الـ Confirmed) تم الدفع بنجاح.
        PaymentReceived = 4,

        // الإلغاء: سواء من المالك أو المستأجر بعد الدفع
        Cancelled = 5,

        //المستأجر وصل وعمل Scan.
        CheckedIn = 6 ,

        // المستأجر مشي والحجز قفل.
        Completed = 7 ,

        //فيه مشكلة أو خناقة لسه بتتحل
        Disputed = 8,
    }
}
