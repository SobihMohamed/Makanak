using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Contracts.Repos
{
    public interface IUserRepository
    {
        Task<bool> IsUserNationalIdExistAsync(string nationalId,string excludedCurrentUserId);
    }
}
