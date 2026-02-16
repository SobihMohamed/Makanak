using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
namespace Makanak.Services.Specifications.Property_Spec
{
    public class AdminPropertySpecifications : BaseSpecifications<Property,int>
    {
        public AdminPropertySpecifications(AdminPropertyParams adminParams, bool isCount = false)
            : base(p =>
                // 1. (Pending, Accepted...)
                (!adminParams.Status.HasValue || p.PropertyStatus == adminParams.Status) &&

                // 2.  (Apartment, Villa...)
                (!adminParams.Type.HasValue || p.PropertyType == adminParams.Type) &&

                // 3. (Cairo, Giza...)
                (!adminParams.GovernorateId.HasValue || p.GovernorateId == adminParams.GovernorateId) &&

                (string.IsNullOrEmpty(adminParams.Search) ||
                 p.Title.ToLower().Contains(adminParams.Search) ||
                 p.AreaName.ToLower().Contains(adminParams.Search)) &&

                (!p.IsDeleted)
            )
        {
            if (!isCount)
            {
                AddInclude(p => p.PropertyImages);
                AddInclude(p => p.Governorate);
                AddInclude(p => p.Owner);


                if (adminParams.Sort.HasValue)
                {
                    switch (adminParams.Sort)
                    {
                        case SortingOptionsEnum.PriceAsc:
                            AddOrderBy(p => p.PricePerNight);
                            break;
                        case SortingOptionsEnum.PriceDesc:
                            AddOrderByDesc(p => p.PricePerNight);
                            break;
                        case SortingOptionsEnum.NameAsc:
                            AddOrderBy(p => p.Title);
                            break;
                        case SortingOptionsEnum.NameDesc:
                            AddOrderByDesc(p => p.Title);
                            break;
                        case SortingOptionsEnum.DateCreatedAsc: 
                            AddOrderBy(p => p.CreatedAt);
                            break;
                        case SortingOptionsEnum.DateCreatedDesc: 
                        default:
                            AddOrderByDesc(p => p.CreatedAt);
                            break;
                    }
                }
                else
                {
                    AddOrderByDesc(p => p.CreatedAt);
                }

                // Pagination
                ApplyPagenation(adminParams.PageSize, adminParams.PageIndex);
            }
        }
    }
}
