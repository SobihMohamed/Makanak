using Makanak.Shared.Dto_s.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.DashboardService
{
    public interface IAdminDashboardService
    {
        Task<UserStatsDto> GetUserStatsAsync();
        Task<PropertyStatsDto> GetPropertyStatsAsync();
        Task<FinancialStatsDto> GetFinancialStatsAsync();
        Task<BookingStatsDto> GetBookingStatsAsync();

    }
}
