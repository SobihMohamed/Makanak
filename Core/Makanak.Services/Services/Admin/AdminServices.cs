using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params;
using Makanak.Shared.Common.Params.User;
using Makanak.Shared.Dto_s.Admin;
using Makanak.Shared.EnumsHelper.Property;
using Microsoft.Extensions.Configuration;


namespace Makanak.Services.Services.Admin
{
    public class AdminServices(IUnitOfWork _unitOfWork , IMapper mapper,
        IEmailService emailService, IConfiguration configuration) : IAdminServices
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
            // get the user repository
            var userRepository = _unitOfWork.GetRepo<ApplicationUser, string>();

            // generate the specifications for the user to be updated
            var userSpec = new UserSpecifications(dto.UserId);

            // get the user with the specifications 
            var user = await userRepository.GetByIdWithSpecificationsAsync(userSpec);

            // if user not found return false
            if (user == null)
                throw  UserNotFoundException.ById(dto.UserId);

            // update the user status and rejected reason
            user.UserStatus = dto.NewStatus;

            user.RejectedReason = dto.RejectedReason;
            userRepository.Update(user);

            // save the changes
            var res = await _unitOfWork.SaveChangesAsync();

            if (res > 0)
            {
                try
                {
                    string emailBody = "";

                    if (!string.IsNullOrEmpty(dto.RejectedReason) && dto.NewStatus == UserStatus.Rejected)
                    {
                        emailBody = $"Dear {user.Name},<br/><br/>Your account verification status has been updated to <b>{dto.NewStatus}</b>.<br/><br/><b>Reason:</b> {dto.RejectedReason}<br/><br/>Best regards,<br/>Makanak Team";
                    }
                    else
                    {
                        emailBody = $"Dear {user.Name},<br/><br/>Your account verification status has been updated to <b>{dto.NewStatus}</b>.<br/><br/>Best regards,<br/>Makanak Team";
                    }

                    await emailService.SendEmailAsync(user.Email, "Makanak - Account Verification Update", emailBody);
                }
                catch (Exception ex)
                {
                    // Log the exception (logging mechanism not shown here
                    throw new BadRequestException($"Email Failed : " + ex.Message);
                }
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
            // get property repository
            var propRepo = _unitOfWork.GetRepo<Property, int>();

            // generate specifications for the property to be updated
            var propSpec = new PropertySpecifications(dto.PropertyId);

            // get the property with the specifications
            var property = await propRepo.GetByIdWithSpecificationsAsync(propSpec);

            // if property not found return false
            if (property == null)
                throw new PropertyNotFound(dto.PropertyId);

            // update the property status and rejected reason
            property.PropertyStatus = dto.NewStatus;
            property.RejectedReason= dto.RejectedReason;

            propRepo.Update(property);
            // save the changes
            var res = await _unitOfWork.SaveChangesAsync();
           
            if (res > 0)
            {
                try
                {
                    var ownerName = property.Owner.Name;
                    var ownerEmail = property.Owner.Email;
                    string emailBody = "";

                    if (!string.IsNullOrEmpty(dto.RejectedReason) && dto.NewStatus == PropertyStatus.Rejected)
                    {
                        emailBody = $"Dear {ownerName},<br/><br/>Your property listing <b>{property.Title}</b> has been <b>Rejected</b>.<br/><br/><b>Reason:</b> {dto.RejectedReason}<br/><br/>Best regards,<br/>Makanak Team";
                    }
                    else if (dto.NewStatus == PropertyStatus.Accepted)
                    {
                        emailBody = $"Dear {ownerName},<br/><br/>Congratulations! Your property listing <b>{property.Title}</b> has been <b>Accepted</b> and is now live on Makanak.<br/><br/>Best regards,<br/>Makanak Team";
                    }

                    await emailService.SendEmailAsync(ownerEmail, "Makanak - Property Status Update", emailBody);

                }
                catch (Exception ex)
                {
                    // Log the exception (logging mechanism not shown here
                    throw new BadRequestException($"Email Failed : " + ex.Message);
                }
                return true;
            }
            
            return false;

        }
    }
}
