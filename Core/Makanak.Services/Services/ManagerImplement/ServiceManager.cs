using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Booking;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.PropertyService;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.Identity;
using Makanak.Services.Services.Admin;
using Makanak.Services.Services.Auth;
using Makanak.Services.Services.BookingImplement;
using Makanak.Services.Services.PropertyImplement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
        public ServiceManager(IUnitOfWork _Uow,IMapper mapper,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _emailService = new Lazy<IEmailService>(() => new EmailServices(configuration));

            _attachementServices = new Lazy<IAttachementServices>(() => new AttachementServices());

            _authService = new Lazy<IAuthService>(() => new AuthService(
                _Uow,
                userManager,
                _attachementServices.Value, 
                configuration,
                mapper,
                _emailService.Value 
            ));

            _adminService = new Lazy<IAdminServices>(() => new AdminServices(_Uow, mapper, _emailService.Value,configuration));
            
            _propertyService = new Lazy<IPropertyService>(() => new PropertyService(mapper,AttachementServices, _Uow));

            _bookingService = new Lazy<IBookingService>(() => new BookingService(_Uow, mapper , userManager));
        }
        public IEmailService EmailService => _emailService.Value;
        public IAttachementServices AttachementServices => _attachementServices.Value;
        public IAuthService AuthService => _authService.Value;
        public IAdminServices AdminService => _adminService.Value;

        public IPropertyService PropertyServices => _propertyService.Value;
        public IBookingService BookingService=> _bookingService.Value;
    }
}
