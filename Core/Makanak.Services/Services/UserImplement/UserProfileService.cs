using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.Dto_s.User;
using Microsoft.AspNetCore.Identity;


namespace Makanak.Services.Services.UserImplement
{
    public class UserProfileService(
        UserManager<ApplicationUser> userManager,
        IAttachementServices attachementServices,
        IMapper mapper) : IUserProfileService
    {
        public async Task<CurrentUserDto> GetUserProfileAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw UserNotFoundException.ByEmail(email);
            }

            // automapper to map ApplicationUser to CurrentUserDto
            var currentUserDto = mapper.Map<ApplicationUser, CurrentUserDto>(user);

            return currentUserDto;
        }

        public async Task<CurrentUserDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw UserNotFoundException.ByEmail(email);
            }

            // images
            if (updateProfileDto.ProfilePicture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    await attachementServices.DeleteImage(user.ProfilePictureUrl);
                }

                string ProfilePicturePath = Path.Combine("Users", user.Id, "Profile");
                string imagePath = await attachementServices.UploadImageAsync(updateProfileDto.ProfilePicture, ProfilePicturePath);

                // save the new image path to the user entity
                user.ProfilePictureUrl = imagePath;
            }

            //map the updateProfileDto to the user entity
            mapper.Map(updateProfileDto, user);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Profile Update Failed", errors);
            }

            // return the updated user profile
            var currentUserMapper = mapper.Map<ApplicationUser, CurrentUserDto>(user);

            return currentUserMapper;
        }
    }
}