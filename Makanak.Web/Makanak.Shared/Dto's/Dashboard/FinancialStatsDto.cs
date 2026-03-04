using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dashboard
{
    public class FinancialStatsDto
    {
        public decimal TotalBookingVolume { get; set; } // إجمالي قيمة الحجوزات اللي تمت عن طريق المنصة

        // 2. فلوسك إنت (اللي دخلت حسابك البنكي فعلياً)
        public decimal TotalPlatformEarnings { get; set; } 

        // 3. فلوس الملاك (للعلم والإحصائيات فقط)
        public decimal TotalCashExpectedByOwners { get; set; }
    }
}
