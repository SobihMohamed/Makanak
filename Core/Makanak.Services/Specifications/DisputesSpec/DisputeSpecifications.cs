using Makanak.Domain.Models.DisputeEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Dispute_Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.DisputesSpec
{
    public class DisputeSpecifications : BaseSpecifications<Dispute,int>
    {
        public DisputeSpecifications(int id) : base(d => d.Id == id)
        {
            AddInclude(d => d.Booking);
            AddInclude(d => d.Booking.Property);
            AddInclude(d => d.Booking.Property.Owner);
            AddInclude(d => d.Booking.Tenant);
            AddInclude(d => d.Complainant);
            AddInclude(d => d.DisputeImages);
        }
        public DisputeSpecifications(DisputeParams disputeParams, string userId, string role)
        : base(d =>
            // 1. شرط الصلاحية (الأدمن يشوف كله، اليوزر يشوف حاجته)
            (role == "Admin" || d.ComplainantId == userId || d.Booking.TenantId == userId || d.Booking.Property.OwnerId == userId)
            &&
            // 2. فلتر البحث العام (Search) لو موجود
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
            AddInclude(d => d.DisputeImages);
            AddOrderByDesc(d => d.CreatedAt);

            ApplyPagenation(disputeParams.PageSize * (disputeParams.PageIndex - 1), disputeParams.PageSize);
        }
    }
}
