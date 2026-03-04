using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Makanak.Shared.Dto_s.Booking
{
    public class CreateBookingDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 50, ErrorMessage = "Number of guests must be at least 1")]
        public int NumberOfGuests { get; set; }

        public string? SpecialRequests { get; set; }
    }
}
