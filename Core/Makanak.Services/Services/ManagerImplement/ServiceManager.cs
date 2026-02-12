using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.AmenityService;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.Cashing;
using Makanak.Abstraction.IServices.DisputeService;
using Makanak.Abstraction.IServices.GovernorateService;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServices.PaymentService;
using Makanak.Abstraction.IServices.PropertyService;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Abstraction.IServices.ReviewService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.Identity;
using Makanak.Services.Services.Admin;
using Makanak.Services.Services.AmenityImplement;
using Makanak.Services.Services.Auth;
using Makanak.Services.Services.BookingImplement;
using Makanak.Services.Services.DisputeImplement;
using Makanak.Services.Services.GovernorateImplement;
using Makanak.Services.Services.NotificationImplement;
using Makanak.Services.Services.PaymentImplement;
using Makanak.Services.Services.PropertyImplement;
using Makanak.Services.Services.ReviewImplement;
using Makanak.Shared.Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace Makanak.Services.Services.ManagerImplement
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEmailService> _emailService;
        private readonly Lazy<IAttachementServices> _attachementServices;
        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<IAdminServices> _adminService;
        private readonly Lazy<IPropertyService> _propertyService;
        private readonly Lazy<IBookingService> _bookingService;
        private readonly Lazy<IPaymentService> _paymentService;
        private readonly Lazy<IReviewService> _reviewService;
        private readonly Lazy<IDisputeService> _disputeService;
        private readonly Lazy<INotificationService> _notificationService;
        private readonly Lazy<IAmenityService> _amenityService;
        private readonly Lazy<IGovernorateService> _governorateService;
        public ServiceManager(IUnitOfWork _Uow,IMapper mapper,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IOptions<StripeSettings> options,
            IRealTimeNotifier notifier,
            ICacheService cacheService
            )
        {
            _emailService = new Lazy<IEmailService>(() => new EmailServices(configuration));

            _attachementServices = new Lazy<IAttachementServices>(() => new AttachementServices());

            _notificationService = new Lazy<INotificationService>(() => new NotificationService(_Uow,notifier,mapper));

            _authService = new Lazy<IAuthService>(() => new AuthService(
                _Uow,
                userManager,
                _attachementServices.Value, 
                configuration,
                mapper,
                _emailService.Value ,
                _notificationService.Value
            ));

            _adminService = new Lazy<IAdminServices>(() => new AdminServices(_Uow, mapper, _emailService.Value,configuration,_notificationService.Value));
            
            _propertyService = new Lazy<IPropertyService>(() => new PropertyService(userManager,mapper,_attachementServices.Value, _Uow,_notificationService.Value));

            _paymentService = new Lazy<IPaymentService> (() => new PaymentServices(options));

            _bookingService = new Lazy<IBookingService>(() => new BookingService(_paymentService.Value,_Uow, mapper , userManager,_notificationService.Value));
            
            _reviewService = new Lazy<IReviewService>(() => new ReviewService(_Uow, mapper,_notificationService.Value));
           
            _disputeService = new Lazy<IDisputeService>(() => new DisputeService(_Uow,mapper,_attachementServices.Value,userManager,_notificationService.Value));
            
            _amenityService = new Lazy<IAmenityService>(() => new AmenityService(cacheService,_Uow,mapper));
            
            _governorateService = new Lazy<IGovernorateService>(() => new GovernorateService(cacheService, _Uow, mapper));


        }
        public IEmailService EmailService => _emailService.Value;
        public IAttachementServices AttachementServices => _attachementServices.Value;
        public IAuthService AuthService => _authService.Value;
        public IAdminServices AdminService => _adminService.Value;

        public IPropertyService PropertyServices => _propertyService.Value;
        public IPaymentService PaymentService => _paymentService.Value;
        public IBookingService BookingService=> _bookingService.Value;
        public IReviewService ReviewService=> _reviewService.Value;
        public IDisputeService DisputeService => _disputeService.Value;
        public IAmenityService AmenityService=> _amenityService.Value;
        public IGovernorateService GovernorateService => _governorateService.Value;
        public INotificationService NotificationService=> _notificationService.Value;
    }
}
