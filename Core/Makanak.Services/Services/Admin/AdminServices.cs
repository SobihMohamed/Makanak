using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.Services.NotificationImplement;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Common.Params.User;
using Makanak.Shared.Dto_s.Admin;
using Makanak.Shared.EnumsHelper.Property;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Stripe;


namespace Makanak.Services.Services.Admin
{
    public class AdminServices(IUnitOfWork _unitOfWork , IMapper mapper,
        IEmailService emailService, IConfiguration configuration , INotificationService notificationService) : IAdminServices
    {
        public async Task<Pagination<UserForApprovalDto>> GetAllUsersAsync(UserParams userParams)
        {
            // get the user repository
            var userRepository = _unitOfWork.GetRepo<ApplicationUser,string>();

            // generate the specifications for pending users
            // is count = false so , it apply pagination & sorting & search which get the data only 
            var UsersSpec = new UserSpecifications(userParams,  false);

            // get all pending users
            var Users = await userRepository.GetAllWithSpecificationAsync(UsersSpec);

            // get total count of pending users
            var totalCountSpec = new UserSpecifications(userParams, true);

            // get the count
            var totalCount = await userRepository.CountAsync(totalCountSpec);

            // map the users to UserForApprovalDto
            var UsersDto = mapper.Map<IReadOnlyList<UserForApprovalDto>>(Users);

            // return the result
            return new Pagination<UserForApprovalDto>(userParams.PageIndex,userParams.PageSize,totalCount,UsersDto);
        }
      
        public async Task<bool> UpdateUserStatusAsync(UpdateUserStatusDto dto)
        {
            var userRepository = _unitOfWork.GetRepo<ApplicationUser, string>();
            var userSpec = new UserSpecifications(dto.UserId);
            var user = await userRepository.GetByIdWithSpecificationsAsync(userSpec);

            if (user == null)
                throw UserNotFoundException.ById(dto.UserId);


            user.UserStatus = dto.NewStatus;

            if (dto.NewStatus == UserStatus.Rejected || dto.NewStatus == UserStatus.Banned)
                user.RejectedReason = dto.RejectedReason;
            else
                user.RejectedReason = null;

            userRepository.Update(user);
            var res = await _unitOfWork.SaveChangesAsync();

            if (res > 0)
            {
                try
                {
                    string emailBody = dto.NewStatus switch
                    {
                        UserStatus.Active => $"Congratulations {user.Name}! Your account is now <b>Active</b>.",
                        UserStatus.Rejected => $"Your account verification was <b>Rejected</b>.<br/>Reason: {dto.RejectedReason}",
                        UserStatus.Banned => $"⛔ Your account has been <b>BANNED</b>.<br/>Reason: {dto.RejectedReason}",
                        _ => $"Your account status changed to {dto.NewStatus}."
                    };

                    await emailService.SendEmailAsync(user.Email, $"Makanak - Account Update: {dto.NewStatus}", emailBody);

                    // 2. الإشعار
                    await notificationService.SendNotificationAsync(
                        NotificationFactory.UserStatusUpdate(
                            dto.UserId,
                            dto.NewStatus, 
                            dto.RejectedReason
                        )
                    );
                }
                catch (Exception) { /* Log */ }

                return true;
            }
            return false;
        }

        public async Task<UserVerificationDetailsDto> GetUserVerificationDetails(string userId)
        {
            // get the user repository
            var userRepository = _unitOfWork.GetRepo<ApplicationUser, string>();

            // generate the specifications for the user with verification details
            var userSpec = new UserSpecifications(userId);

            // get the user with the specifications
            var user = await userRepository.GetByIdWithSpecificationsAsync(userSpec);

            // map the user to UserVerificationDetailsDto
            var userVerificationDetailsDto = mapper.Map<UserVerificationDetailsDto>(user);

            // return the result
            return userVerificationDetailsDto;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            var userRepo = _unitOfWork.GetRepo<ApplicationUser, string>();

            async Task<int> GetCount(UserStatus? status = null, UserTypes? type = null)
            {
                var parameters = new UserParams
                {
                    Status = status,
                    Type = type
                };
                // isCount = true to get only the count without pagination and sorting
                var spec = new UserSpecifications(parameters, isCount: true);
                return await userRepo.CountAsync(spec);
            }
            var stats = new AdminDashboardStatsDto
            {
             
                TotalUsers = await GetCount(status: null, type: null),
                
                PendingUsers = await GetCount(status: UserStatus.Pending),
                ActiveUsers = await GetCount(status: UserStatus.Active),
                RejectsCount = await GetCount(status: UserStatus.Rejected),
                BannedsCount = await GetCount(status: UserStatus.Banned), 
                NewsCount = await GetCount(status: UserStatus.New),

                OwnersCount = await GetCount(type: UserTypes.Owner),
                AdminsCount = await GetCount(type: UserTypes.Admin),
                TenantsCount = await GetCount(type: UserTypes.Tenant)
            };

            return stats;
        }

        public async Task<bool> UpdatePropertyStatus(UpdatePropertyStatusDto dto)
        {
            var propRepo = _unitOfWork.GetRepo<Property, int>();

            var propSpec = new PropertySpecifications(dto.PropertyId);
            var property = await propRepo.GetByIdWithSpecificationsAsync(propSpec);

            if (property == null) throw new PropertyNotFound(dto.PropertyId);

            property.PropertyStatus = dto.NewStatus;

            if (dto.NewStatus == PropertyStatus.Rejected || dto.NewStatus == PropertyStatus.Banned)
                property.RejectedReason = dto.RejectedReason;
            else
                property.RejectedReason = null; // تنظيف

            propRepo.Update(property);
            var res = await _unitOfWork.SaveChangesAsync();

            if (res > 0)
            {
                try
                {
                    if (property.Owner != null)
                    {
                        string emailBody = dto.NewStatus switch
                        {
                            PropertyStatus.Accepted =>
                                $"Dear {property.Owner.Name},<br/>Your property <b>{property.Title}</b> is now <b>Live</b>! 🎉",

                            PropertyStatus.Rejected =>
                                $"Dear {property.Owner.Name},<br/>Your property <b>{property.Title}</b> was <b>Rejected</b>.<br/>Reason: {dto.RejectedReason}",

                            PropertyStatus.Banned =>
                                $"⚠️ ALERT: Your property <b>{property.Title}</b> has been <b>BANNED</b>.<br/>Reason: {dto.RejectedReason}<br/>Please contact support.",

                            PropertyStatus.Pending =>
                                $"Dear {property.Owner.Name},<br/>Your property <b>{property.Title}</b> is back under review.",

                            _ => "" // Default case
                        };

                        if (!string.IsNullOrEmpty(emailBody))
                        {
                            string subject = $"Makanak - Property Status: {dto.NewStatus}";
                            await emailService.SendEmailAsync(property.Owner.Email, subject, emailBody);
                        }
                    }

                    await notificationService.SendNotificationAsync(
                        NotificationFactory.PropertyStatusUpdate(
                            property.OwnerId,
                            property.Title,
                            dto.NewStatus,         
                            dto.RejectedReason
                        )
                    );
                }
                catch (Exception ex)
                {
                    // _logger.LogError(...);
                }
                return true;
            }

            return false;
        }
    }
}
