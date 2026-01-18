using Makanak.Domain.EnumsHelper.User;
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

        public UserSpecifications(string Id)
            : base(user => user.Id == Id)
        {
        }

        public UserSpecifications(UserStatus userStatus)
            : base(user=>user.UserStatus == userStatus)
        {
            AddOrderBy(user => user.CreatedAt);
        }
    }
}
