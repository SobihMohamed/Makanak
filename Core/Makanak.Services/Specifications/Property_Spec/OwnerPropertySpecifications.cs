using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.EnumsHelper.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.Property_Spec
{
    public class OwnerPropertySpecifications : BaseSpecifications<Property,int>
    {
        public OwnerPropertySpecifications(string ownerId, OwnerPropertyParams propertyParams, bool isCount)
        : base(p =>
            p.OwnerId == ownerId &&

            !p.IsDeleted &&

            (string.IsNullOrEmpty(propertyParams.Search) ||
             p.Title.ToLower().Contains(propertyParams.Search)) && 
            (!propertyParams.FilterStatus.HasValue || propertyParams.FilterStatus == p.PropertyStatus)

        )
        {
            if (!isCount)
            {
                AddInclude(p => p.PropertyImages);
                AddInclude(p => p.Governorate);
                AddInclude(p => p.Amenities);

                // Sorting
                if (propertyParams.Sort.HasValue)
                {
                    switch (propertyParams.Sort)
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
                ApplyPagenation(propertyParams.PageSize, propertyParams.PageIndex);
            }
        }
    }
}