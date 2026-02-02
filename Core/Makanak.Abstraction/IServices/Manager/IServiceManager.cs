using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.PropertyService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Manager
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        IAdminServices AdminService { get; }
        IEmailService EmailService { get; }
        IAttachementServices AttachementServices { get; }
        IPropertyService PropertyServices { get; }
        IBookingService BookingService{ get; }
    }
}
