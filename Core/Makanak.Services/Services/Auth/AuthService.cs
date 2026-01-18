using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.ResetPassword;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Makanak.Services.Services.Auth
{
    public class AuthService(IUnitOfWork unitOfWork, IUserRepository userRepository, UserManager<ApplicationUser> userManager,
        IAttachementServices attachementServices, IConfiguration configuration,
        IMapper mapper, IEmailService emailService)
        : IAuthService
    {
        public async Task<AuthModelDto> LoginAsync(LoginDto loginDto)
        {
            //check if user exist by email
            var User = await userManager.FindByEmailAsync(loginDto.Email);
            if (User == null)
            {
                //Throw New UserNotFoundException 
                throw new UserNotFoundException(loginDto.Email);
            }
            var isPasswordValid = await userManager.CheckPasswordAsync(User!, loginDto.Password);
            if (!isPasswordValid)
            {
                //Throw New UnAuthorizedException
                throw new UnauthorizedException();
            }
            //Generate JWT Token
            var Token = await GenerateJwtToken(User!);
            // GetRoles Of User
            var roles = await userManager.GetRolesAsync(User!);
            // Return AuthModelDto with Token and User Info
            var AuthModel = new AuthModelDto
            {
                Message = "Login Successful",
                Name = User!.UserName!,
                Email = User.Email!,
                IsAuthenticated = true,
                Token = Token.Token,
                ExpiresOn = Token.ExpiresOn,
                Roles = roles.ToList()
            };
            return AuthModel;
        }

        public async Task<AuthModelDto> RegisterAsync(RegisterDto registerDto)
        {
            var isEmailExist = await userManager.FindByEmailAsync(registerDto.Email);
            if (isEmailExist != null)
            {
                throw new BadRequestException("Email is already in use");
            }
            // Map RegisterDto to ApplicationUser
            var mappedUser = mapper.Map<RegisterDto, ApplicationUser>(registerDto);

            // create user
            var result = await userManager.CreateAsync(mappedUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("User Registration Failed", errors);
            }

            // Assign Role to User
            var roleResult = await userManager.AddToRoleAsync(mappedUser, registerDto.UserType.ToString());
            if (!roleResult.Succeeded)
            {
                // delete user 
                await userManager.DeleteAsync(mappedUser);

                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Assign Role Failed", errors);
            }

            // Generate JWT Token
            var token = await GenerateJwtToken(mappedUser);
            return new AuthModelDto
            {
                Message = "User registered successfully",
                Name = mappedUser.Name!,
                Email = mappedUser.Email!,
                IsAuthenticated = true,
                Token = token.Token,
                ExpiresOn = token.ExpiresOn,
                Roles = new List<string> { registerDto.UserType.ToString() }
            };
        }

        public async Task<CurrentUserDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Not Found Exception User
                throw new UserNotFoundException(email);
            }
            if (updateProfileDto.ProfilePicture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    await attachementServices.DeleteImage(user.ProfilePictureUrl);
                }
                string imagePath = await attachementServices.UploadImageAsync(updateProfileDto.ProfilePicture, $"{user.Id}");
                user.ProfilePictureUrl = imagePath;
            }
            mapper.Map(updateProfileDto, user); // assign values and save in user
            // update the user
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Profile Update Failed", errors);
            }

            var currentUserMapper = mapper.Map<ApplicationUser, CurrentUserDto>(user);
            if (currentUserMapper == null)
                throw new Exception("Error During Mapping");

            return currentUserMapper;
        }

        public async Task<string> InitiateEmailChangeAsync(ChangeEmailDto changeEmailDto, string currentEmail)
        {
            // get the user
            var user = await userManager.FindByEmailAsync(currentEmail);

            if (user == null) throw new UserNotFoundException(currentEmail);

            // verify current password
            var isPasswordValid = await userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);

            if (!isPasswordValid) throw new UnauthorizedException();

            // check if new email is already used
            var isEmailExist = await userManager.FindByEmailAsync(changeEmailDto.NewEmail);
            if (isEmailExist != null) throw new BadRequestException("Email is already in use");

            // generate otp 
            var otp = await GenerateAndSaveOtpAsync(user.Id, changeEmailDto.NewEmail);

            // send email to new email with token and otp
            await emailService.SendEmailAsync(changeEmailDto.NewEmail, "Confirm New Email", $"Use this code to verify your new email: {otp}");

            return otp;
        }

        public async Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto)
        {
            // verify otp 
            // true to burn it after verification because we don't want reuse after change data
            var otpRecord = await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, true);

            // get user
            var user = await userManager.FindByIdAsync(otpRecord.UserId);

            if (user == null) throw new UserNotFoundException(otpRecord.Email);

            // apply changes
            user.Email = otpRecord.Email;
            user.UserName = otpRecord.Email; // assuming username is same as email
            user.EmailConfirmed = true;
            user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
            user.NormalizedUserName = userManager.NormalizeName(user.UserName);

            // update database
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Email Change Failed", errors);
            }
            return true;
        }

        public async Task<CurrentUserDto> GetUserProfileAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Not Found Exception User
                throw new UserNotFoundException(email);
            }
            var currentUserDto = mapper.Map<ApplicationUser, CurrentUserDto>(user);
            if (currentUserDto == null)
                throw new Exception("Mapping failed");
            return currentUserDto;
        }

        public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                throw new UserNotFoundException(resetPasswordDto.Email);
            }

            await VerifyAndBurnOtpAsync(resetPasswordDto.Email, resetPasswordDto.Otp, true);

            if (await userManager.HasPasswordAsync(user!))
            {
                await userManager.RemovePasswordAsync(user!);
            }
            var resetPassResult = await userManager.AddPasswordAsync(user!, resetPasswordDto.NewPassword);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                throw new BadRequestException("Password Reset Failed", errors);
            }

            var newToeken = await GenerateJwtToken(user!);

            return new AuthModelDto
            {
                Message = "Password Reset Successful",
                Name = user!.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = newToeken.Token,
                ExpiresOn = newToeken.ExpiresOn,
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            };

        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            // check if exist email 
            var user = await userManager.FindByEmailAsync(forgetPasswordRequestDto.Email);
            if (user == null) throw new UserNotFoundException(forgetPasswordRequestDto.Email);

            // generate new otp
            var newOtp = await GenerateAndSaveOtpAsync(user.Id, forgetPasswordRequestDto.Email);

            // send the otp to user email
            await emailService.SendEmailAsync(user.Email!, "Password Reset OTP", $"Your OTP code is: {newOtp}");

            return true;

        }

        public async Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email)
        {
            // get user 
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) throw new UserNotFoundException(email);

            // check status
            if (user.UserStatus == UserStatus.Pending || user.UserStatus == UserStatus.Active)
                throw new BadRequestException("You cannot update your identity while it is pending or Active.");

            // check if national id is already used
            var isDublicated = await userRepository.IsUserNationalIdExistAsync(verifyIdentityDto.NationalId!, user.Id);
            if (isDublicated) throw new BadRequestException("National ID is already in use by another user.");

            // upload front & back image
            string frontImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageFrontUrl!, $"{user.Id}");
            string backImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageBackUrl!, $"{user.Id}");

            // update user info
            user.NationalId = verifyIdentityDto.NationalId;
            user.NationalIdImageFrontUrl = frontImagePath;
            user.NationalIdImageBackUrl = backImagePath;
            user.UserStatus = UserStatus.Pending;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("Identity Verification Failed: ", errors);
            }

            return true;
        }

        public async Task<bool> Logout(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }
            // For JWT, logout is typically handled on the client side by deleting the token.
            // Optionally, you can implement token blacklisting here if needed.
            return true;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, false);

            return true;
        }

        #region Private Methods        
        private async Task<(string Token, DateTime ExpiresOn)> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            // first create list of claims 
            var UserClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                UserClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //get the Security Key from app settings
            var SecurityKey = configuration.GetSection("JWTOptions:SecurityKey").Value;

            // create the Key and Signing Credentials
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            var SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            // expire time for the token
            var expireTime = DateTime.UtcNow.AddHours(12);
            // Create the token
            var Token = new JwtSecurityToken(
                issuer: configuration.GetSection("JWTOptions:Issuer").Value,
                audience: configuration.GetSection("JWTOptions:Audience").Value,
                claims: UserClaims,
                expires: expireTime,
                signingCredentials: SigningCredentials
            );
            return (new JwtSecurityTokenHandler().WriteToken(Token), expireTime);
        }

        private async Task<string> GenerateAndSaveOtpAsync(string UserId, string email)
        {
            // get the specification 
            var Specification = new UserOtpSpecification(email);

            // get Repo 
            var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();

            // get the user old otp by specification
            var oldUserOtp = await userOtpRepo.GetAllWithSpecificationAsync(Specification);

            // if exist update to used
            foreach (var otp in oldUserOtp)
            {
                otp.IsUsed = true;
                userOtpRepo.Update(otp);
            }

            // generate new otp
            var newOtp = new Random().Next(100000, 999999).ToString();

            // save it to db with expire time 5 minutes\
            var userOtp = new UserOtp
            {
                Email = email,
                OtpCode = newOtp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                UserId = UserId,
                LastModifiedBy = UserId,
                UpdatedAt = DateTime.UtcNow
            };

            userOtpRepo.AddAsync(userOtp);
            await unitOfWork.SaveChangesAsync();

            return newOtp;
        }

        private async Task<UserOtp> VerifyAndBurnOtpAsync(string email, string otp, bool burnIt)
        {

            var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();

            var specification = new UserOtpSpecification(email, otp);

            var existOtp = await userOtpRepo.GetByIdWithSpecificationsAsync(specification);

            if (existOtp == null)
            {
                throw new BadRequestException("Invalid Otp");
            }
            if (existOtp.ExpirationTime < DateTime.UtcNow)
            {
                throw new BadRequestException("Expired Otp");
            }
            // 3. Burn the OTP

            existOtp.IsUsed = burnIt;
            userOtpRepo.Update(existOtp);
            await unitOfWork.SaveChangesAsync();


            return existOtp;

        }
        #endregion
    }
}