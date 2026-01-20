using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Admin
{
    public class AdminDashboardStatsDto
    {
        public int TotalUsers { get; set; }
        
        public int AdminsCount { get; set; }
        public int TenantsCount { get; set; }
        public int OwnersCount { get; set; }

        public int PendingUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int RejectsCount { get; set; }
        public int BannedsCount { get; set; }
        public int NewsCount { get; set; }
    }
}
