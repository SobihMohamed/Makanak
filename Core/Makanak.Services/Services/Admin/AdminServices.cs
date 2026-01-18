using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Dto_s.Admin;
using Microsoft.Extensions.Configuration;


namespace Makanak.Services.Services.Admin
{
    public class AdminServices(IUnitOfWork _unitOfWork , IMapper mapper,
        IEmailService emailService , IConfiguration configuration) : IAdminServices
    {
        public async Task<IEnumerable<UserForApprovalDto>> GetAllPendingUsersAsync()
        {
            // get the user repository
            var userRepository = _unitOfWork.GetRepo<ApplicationUser,string>();

            // generate the specifications for pending users
            var pendingUsersSpec = new UserSpecifications(UserStatus.Pending);

            // get all pending users
            var pendingUsers = await userRepository.GetAllWithSpecificationAsync(pendingUsersSpec);

            // map the users to UserForApprovalDto
            var pendingUsersDto = mapper.Map<IEnumerable<UserForApprovalDto>>(pendingUsers);

            // return the result
            return pendingUsersDto;
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
            if (Enum.TryParse<UserStatus>(dto.NewStatus, true, out var newStatus))
                user.UserStatus = newStatus;
            else
                throw new BadRequestException($"Invalid status: {dto.NewStatus}");
            
            user.RejectedReason = dto.RejectedReason;
            userRepository.Update(user);

            // save the changes
            var res = await _unitOfWork.SaveChangesAsync();

            if (res > 0)
            {
                try
                {
                    string emailBody = "";

                    if (!string.IsNullOrEmpty(dto.RejectedReason) && newStatus == UserStatus.Rejected)
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
    }
}
