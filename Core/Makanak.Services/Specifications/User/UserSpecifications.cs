using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Common.Params.User;
using Microsoft.AspNetCore.Identity;
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

        public UserSpecifications(UserParams userParams, bool isCount = false)
            : base(user =>
                // 1. فلتر الحالة (لو مبعتش هات كله، لو بعت هات اللي طلبته)
                (!userParams.Status.HasValue || user.UserStatus == userParams.Status) &&

                // 2. فلتر النوع (لو مبعتش هات كله، لو بعت Owner هات ملاك بس)
                (!userParams.Type.HasValue || user.UserType == userParams.Type) &&

                // 3. البحث (لو مبعتش هات كله، لو بعت ابحث في الاسم والايميل)
                (string.IsNullOrEmpty(userParams.Search)
                || user.Name.ToLower().Contains(userParams.Search)
                || user.Email.ToLower().Contains(userParams.Search))
            )
        {
            ApplyCommonQuery(userParams, isCount);
        }
        private void ApplyCommonQuery(UserParams userParams , bool isCount)
        {
            // if not count so we need data with pagenation and sorting
            // if count we just need the filter (where) part without pagenation and sorting
            if (!isCount)
            {
                // Sorting
                if (userParams.Sort != null)
                {
                    switch (userParams.Sort)
                    {
                        case SortingOptionsEnum.NameAsc:
                            AddOrderBy(user => user.Name);
                            break;
                        case SortingOptionsEnum.NameDesc:
                            AddOrderByDesc(user => user.Name);
                            break;
                        case SortingOptionsEnum.DateCreatedAsc:
                            AddOrderBy(user => user.CreatedAt);
                            break;
                        default:
                            AddOrderByDesc(user => user.CreatedAt);
                            break;
                    }
                }
                else
                {
                    // Default sorting
                    AddOrderByDesc(user => user.CreatedAt);
                }
                // Pagenation
                ApplyPagenation(userParams.PageSize, userParams.PageIndex);
            }
        }
    }
}
