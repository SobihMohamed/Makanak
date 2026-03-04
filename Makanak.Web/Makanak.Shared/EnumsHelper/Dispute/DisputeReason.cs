using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Shared.EnumsHelper.Dispute
{
    using System.ComponentModel;

    public enum DisputeReason
    {
        // Tenant Reasons
        [Description("Property is not as described or matches photos")]
        PropertyNotAsDescribed = 1,

        [Description("Check-in or key handover issue")]
        CheckInIssue = 2,

        [Description("Cleanliness issue")]
        CleanlinessIssue = 3,

        [Description("Host is unreachable")]
        HostUnreachable = 4,

        // Owner Reasons
        [Description("Damage to property or belongings")]
        DamageToProperty = 5,

        // Common
        [Description("Other")]
        Other = 99
    }
}
