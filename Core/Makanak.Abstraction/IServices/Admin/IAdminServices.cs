using Makanak.Shared.Dto_s.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Admin
{
    public interface IAdminServices
    {
        Task<IEnumerable<UserForApprovalDto>> GetAllPendingUsersAsync();

        Task<UserVerificationDetailsDto> GetUserVerificationDetails(string userId);

        Task<bool> UpdateUserStatusAsync(UpdateUserStatusDto dto);
    }
}
