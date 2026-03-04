using Makanak.Domain.Models.DisputeEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Dispute_Params;
using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.DisputesSpec
{
    public class DisputeSpecifications : BaseSpecifications<Dispute, int>
    {

        public DisputeSpecifications(int id) : base(d => d.Id == id)
        {
            // 1. Includes for Authorization Checks & Property details
            AddInclude(d => d.Booking);
            AddInclude(d => d.Booking.Property);

            // defendant can be either Tenant or Owner, so we include both for authorization check
            AddInclude(d => d.Booking.Tenant);
            AddInclude(d => d.Booking.Property.Owner);

            // 2. Includes for DTO Mapping
            AddInclude(d => d.Complainant);
            AddInclude(d => d.DisputeImages);
        }
        public DisputeSpecifications(DisputeParams disputeParams, string userId, string role)
            : base(d =>
                // 1. شرط الصلاحية
                (role == "Admin" || d.ComplainantId == userId || d.Booking.TenantId == userId || d.Booking.Property.OwnerId == userId)
                &&
                // 2. البحث (Search)
                (string.IsNullOrEmpty(disputeParams.Search) || d.Booking.Property.Title.ToLower().Contains(disputeParams.Search))
                &&
                // 3. فلتر الحالة (Status)
                (!disputeParams.Status.HasValue || d.Status == disputeParams.Status)
                &&
                // 4. فلتر رقم الحجز
                (!disputeParams.BookingId.HasValue || d.BookingId == disputeParams.BookingId)
            )
        {
            AddInclude(d => d.Booking);
            AddInclude(d => d.Booking.Property);

            AddInclude(d => d.Booking.Tenant);
            AddInclude(d => d.Booking.Property.Owner);

            AddInclude(d => d.DisputeImages);
            AddInclude(d => d.Complainant);

            if (disputeParams.Sort.HasValue)
            {
                switch (disputeParams.Sort)
                {
                    case DisputeSortEnum.DateAsc:
                        AddOrderBy(d => d.CreatedAt);
                        break;
                    case DisputeSortEnum.StatusAsc:
                        AddOrderBy(d => d.Status);
                        break;
                    case DisputeSortEnum.DateDesc:
                    default:
                        AddOrderByDesc(d => d.CreatedAt);
                        break;
                }
            }
            else
            {
                AddOrderByDesc(d => d.CreatedAt);
            }

            ApplyPagenation(disputeParams.PageSize, disputeParams.PageIndex );
        }
    }
}
