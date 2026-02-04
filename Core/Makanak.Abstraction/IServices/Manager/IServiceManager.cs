using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Abstraction.IServices.PropertyService;
using Makanak.Abstraction.IServices.ReviewService;
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
        IPaymentService PaymentService { get; }
        IBookingService BookingService{  get; }
        IReviewService ReviewService { get; }
    }
}
