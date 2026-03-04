using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class BookingNotFoundByQrCode(string qrCode)
        : NotFoundException_Base($"Booking with QR Code '{qrCode}' was not found or is not in a confirmable state.")
    {
    }
}
