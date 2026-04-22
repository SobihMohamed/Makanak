using Makanak.Shared.Dto_s.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.User
{
    public interface IUserProfileService
    {
        Task<CurrentUserDto> GetUserProfileAsync(string email);
        Task<CurrentUserDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto, string email);
    }
}
