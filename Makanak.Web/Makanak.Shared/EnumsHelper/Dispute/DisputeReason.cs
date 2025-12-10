using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Dispute
{
    public enum DisputeReason
    {
        UnitNotAsDescribed = 1, // الشقة غير مطابقة للصور
        OwnerAskedForExtraMoney = 2, // المالك طلب فلوس زيادة
        UnitNotClean = 3, // الشقة غير نظيفة
        OwnerNoShow = 4, // المالك مجاش يسلم المفتاح
        Other = 5 // سبب آخر
    }
}
