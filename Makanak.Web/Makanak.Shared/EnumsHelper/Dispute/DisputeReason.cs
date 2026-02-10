using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Dispute
{
    public enum DisputeReason
    {
        PropertyNotAsDescribed = 1, // العقار غير مطابق للصور
        CheckInIssue = 2,           // مشكلة في الدخول (المفتاح مش موجود مثلاً)
        CleanlinessIssue = 3,       // المكان مش نضيف
        HostUnreachable = 4,        // المالك مبردش

        // أسباب للمالك
        DamageToProperty = 5,       // المستأجر كسر حاجة
        GuestDidNotLeave = 6,       // المستأجر ممشيش في الميعاد
        PartyOrNoise = 7,           // عمل دوشة وحفلات ممنوعة

        Other = 99// سبب آخر
    }
}
