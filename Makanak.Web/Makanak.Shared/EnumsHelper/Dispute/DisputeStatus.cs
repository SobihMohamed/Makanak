using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Dispute
{
    public enum DisputeStatus
    {
        Pending = 1,   // جاري المراجعة (لسه واصلة للأدمن)
        Resolved = 2,  // اتحلت (تم الحكم فيها)
        Rejected = 3,  // اترفضت (الشكوى كيدية)
        Cancelled = 4  // الشاكي سحب الشكوى
    }
}
