using Makanak.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.User
{
    public class UserSpecifications : BaseSpecifications<ApplicationUser, string>
    {
        public UserSpecifications(string nationalId, string excludedCurrentUserId)
            :base(user => user.NationalId == nationalId && user.Id != excludedCurrentUserId)
        {

        }
    }
}
