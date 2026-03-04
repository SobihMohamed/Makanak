using Makanak.Domain.Models.DisputeEntities;
using Makanak.Shared.EnumsHelper.Dispute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Specifications.DisputesSpec
{
    public class PendingDisputeByBookingSpecification : BaseSpecifications<Dispute,int>
    {
        public PendingDisputeByBookingSpecification(int bookingId)
            : base(d => d.BookingId == bookingId && d.Status == DisputeStatus.Pending)
        {
        }
    }
}