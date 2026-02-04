using System.ComponentModel.DataAnnotations;

namespace Makanak.Shared.Dto_s.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int BookingId { get; set; } 

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
