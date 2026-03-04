using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Dispute
{
    public class DisputeDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string PropertyName { get; set; } = null!;
        public string ComplainantName { get; set; } = null!; // مين المشتكي
        public string DefendantName { get; set; } = null!;   // مين المشتكى عليه (هنحسبها)
        public string Status { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? AdminComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<string> Images { get; set; } = new();
    }
}
