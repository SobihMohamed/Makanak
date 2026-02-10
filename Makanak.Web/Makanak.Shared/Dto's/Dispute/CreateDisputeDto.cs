using Makanak.Shared.EnumsHelper.Dispute;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dispute
{
    public class CreateDisputeDto
    {
        public int BookingId { get; set; }
        public DisputeReason Reason { get; set; }
        public string Description { get; set; } = null!;
        public List<IFormFile>? Images { get; set; }
    }
}
