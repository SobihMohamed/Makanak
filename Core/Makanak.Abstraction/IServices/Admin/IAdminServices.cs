using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Common.Params.User;
using Makanak.Shared.Dto_s.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Admin
{
    public interface IAdminServices
    {
        Task<Pagination<UserForApprovalDto>> GetAllUsersAsync(UserParams userParams);

        Task<UserVerificationDetailsDto> GetUserVerificationDetails(string userId);

        Task<bool> UpdateUserStatusAsync(UpdateUserStatusDto dto);

        Task<bool> UpdatePropertyStatus(UpdatePropertyStatusDto dto);

        Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
    }
}
