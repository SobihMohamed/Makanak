using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models.Identity;
using Makanak.Services.Services.Admin;
using Makanak.Services.Services.Auth;
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
        }
        public IEmailService EmailService => _emailService.Value;
        public IAttachementServices AttachementServices => _attachementServices.Value;
        public IAuthService AuthService => _authService.Value;
        public IAdminServices AdminService => _adminService.Value;

    }
}
