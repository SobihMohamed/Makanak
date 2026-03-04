using Makanak.Domain.Models.DisputeEntities;
using Makanak.Shared.Common.Params.Dispute_Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.DisputesSpec
{
    public class DisputeWithCountSpecification : BaseSpecifications<Dispute, int>
    {
        public DisputeWithCountSpecification(DisputeParams disputeParams, string userId, string role)
            : base(d =>
                (role == "Admin" || d.ComplainantId == userId || d.Booking.TenantId == userId || d.Booking.Property.OwnerId == userId)
                &&
                (string.IsNullOrEmpty(disputeParams.Search) || d.Booking.Property.Title.ToLower().Contains(disputeParams.Search))
                &&
                (!disputeParams.Status.HasValue || d.Status == disputeParams.Status)
                &&
                (!disputeParams.BookingId.HasValue || d.BookingId == disputeParams.BookingId)
            )
        {
        }
    }
}