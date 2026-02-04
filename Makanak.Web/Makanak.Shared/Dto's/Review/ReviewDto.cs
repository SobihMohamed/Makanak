using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Dto_s.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ReviewerName { get; set; }
        public string? ReviewerPhotoUrl { get; set; }
    }
}
