using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.NotificationService;
using Makanak.Abstraction.IServicesContract.Token;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.ResetPassword;
using Makanak.Services.Services.NotificationImplement;
using Makanak.Services.Specifications.User;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.Token;
using Makanak.Shared.Dto_s.User;
using Makanak.Shared.HelpersFactory;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Makanak.Services.Services.Auth
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper) : IAuthService
    {
        public async Task<AuthModelDto> LoginAsync(LoginDto loginDto)
        {
            // 1. Check if user exists
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new UnauthorizedException();
            }

            // 2. Check password
            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedException();
            }

            // 3. Get Roles
            var roles = await userManager.GetRolesAsync(user);

            // 4. Generate Token using the TokenService
            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList()
            };
            var tokenResponse = await tokenService.CreateTokenAsync(tokenRequest);

            // 5. Return Response
            return new AuthModelDto
            {
                Message = "Login Successful",
                Name = user.Name ?? user.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = tokenResponse.Token,
                ExpiresOn = tokenResponse.ExpireOn,
                Roles = roles.ToList()
            };
        }

        public async Task<AuthModelDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Check email existance
            var isEmailExist = await userManager.FindByEmailAsync(registerDto.Email);
            if (isEmailExist != null)
            {
                throw new BadRequestException("Email is already in use");
            }

            // 2. Map and Create User
            var mappedUser = mapper.Map<RegisterDto, ApplicationUser>(registerDto);
            var result = await userManager.CreateAsync(mappedUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException("User Registration Failed", errors);
            }

            // 3. Assign Role
            var roleResult = await userManager.AddToRoleAsync(mappedUser, registerDto.UserType.ToString());
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(mappedUser); // Rollback
                var errors = roleResult.Errors.Select(e => e.Description);
                throw new BadRequestException("Assign Role Failed", errors);
            }

            // 4. Generate Token using the new TokenService
            var tokenRequest = new TokenRequestDto
            {
                UserId = mappedUser.Id, // id is generated after user creation
                UserName = mappedUser.UserName!,
                Email = mappedUser.Email!,
                Roles = new List<string> { registerDto.UserType.ToString() }
            };
            var tokenResponse = await tokenService.CreateTokenAsync(tokenRequest);

            // 5. Return Response
            return new AuthModelDto
            {
                Message = "User registered successfully",
                Name = mappedUser.Name!,
                Email = mappedUser.Email!,
                IsAuthenticated = true,
                Token = tokenResponse.Token,
                ExpiresOn = tokenResponse.ExpireOn,
                Roles = tokenRequest.Roles.ToList()
            };
        }

        public async Task<bool> LogoutAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw UserNotFoundException.ByEmail(email);
            }

            // For JWT, logout is handled client-side. 
            // Blacklisting logic can be added here if needed.
            return true;
        }
        #region

        //public async Task<CurrentUserDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto, string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        // Not Found Exception User
        //        throw  UserNotFoundException.ByEmail(email);
        //    }
        //    if (updateProfileDto.ProfilePicture != null)
        //    {
        //        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        //        {
        //            await attachementServices.DeleteImage(user.ProfilePictureUrl);
        //        }
        //        string ProfilePicturePath = Path.Combine("Users",user.Id,"Profile");
        //        string imagePath = await attachementServices.UploadImageAsync(updateProfileDto.ProfilePicture, ProfilePicturePath);
        //        user.ProfilePictureUrl = imagePath;
        //    }
        //    mapper.Map(updateProfileDto, user); // assign values and save in user
        //    // update the user
        //    var result = await userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        throw new BadRequestException("Profile Update Failed", errors);
        //    }

        //    var currentUserMapper = mapper.Map<ApplicationUser, CurrentUserDto>(user);
        //    if (currentUserMapper == null)
        //        throw new Exception("Error During Mapping");

        //    return currentUserMapper;
        //}

        //public async Task<string> InitiateEmailChangeAsync(ChangeEmailDto changeEmailDto, string currentEmail)
        //{
        //    // get the user
        //    var user = await userManager.FindByEmailAsync(currentEmail);

        //    if (user == null) throw UserNotFoundException.ByEmail(currentEmail);

        //    // verify current password
        //    var isPasswordValid = await userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);

        //    if (!isPasswordValid) throw new BadRequestException("Incorrect current password");

        //    // check if new email is already used
        //    var isEmailExist = await userManager.FindByEmailAsync(changeEmailDto.NewEmail);
        //    if (isEmailExist != null) throw new BadRequestException("Email is already in use");

        //    // generate otp 
        //    var otp = await GenerateAndSaveOtpAsync(user.Id, changeEmailDto.NewEmail);

        //    // send email to new email with token and otp
        //    await emailService.SendEmailAsync(
        //        changeEmailDto.NewEmail,
        //        "Confirm New Email - Makanak",
        //        $"Use this code to verify your new email: {otp}");

        //    // 2. Send Security Alert to the OLD email (تنبيه أمني للإيميل القديم لحماية الحساب)
        //    await emailService.SendEmailAsync(
        //        currentEmail,
        //        "Security Alert: Email Change Requested",
        //        $"Hello,\nWe received a request to change the email associated with your Makanak account to " +
        //        $"({changeEmailDto.NewEmail}).\n\nIf you made this request, you can safely ignore this email." +
        //        $"\nIf you did NOT make this request, please contact our support team immediately to secure your account."
        //    );
        //    return otp;
        //}

        //public async Task<bool> ConfirmEmailChangeAsync(VerifyOtpDto verifyOtpDto)
        //{
        //    // verify otp 
        //    // true to burn it after verification because we don't want reuse after change data
        //    var otpRecord = await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, true);

        //    // get user
        //    var user = await userManager.FindByIdAsync(otpRecord.UserId);

        //    if (user == null) throw UserNotFoundException.ByEmail(otpRecord.Email);

        //    // apply changes
        //    user.Email = otpRecord.Email;
        //    user.UserName = otpRecord.Email; // assuming username is same as email
        //    user.EmailConfirmed = true;
        //    user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
        //    user.NormalizedUserName = userManager.NormalizeName(user.UserName);

        //    // update database
        //    var result = await userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        throw new BadRequestException("Email Change Failed", errors);
        //    }
        //    return true;
        //}

        //public async Task<CurrentUserDto> GetUserProfileAsync(string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        // Not Found Exception User
        //        throw UserNotFoundException.ByEmail(email);
        //    }
        //    var currentUserDto = mapper.Map<ApplicationUser, CurrentUserDto>(user);
        //    if (currentUserDto == null)
        //        throw new Exception("Mapping failed");
        //    return currentUserDto;
        //}

        //public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        //{
        //    var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        //    if (user == null)
        //    {
        //        throw UserNotFoundException.ByEmail(resetPasswordDto.Email);
        //    }

        //    await VerifyAndBurnOtpAsync(resetPasswordDto.Email, resetPasswordDto.Otp, true);

        //    if (await userManager.HasPasswordAsync(user!))
        //    {
        //        await userManager.RemovePasswordAsync(user!);
        //    }
        //    var resetPassResult = await userManager.AddPasswordAsync(user!, resetPasswordDto.NewPassword);

        //    if (!resetPassResult.Succeeded)
        //    {
        //        var errors = resetPassResult.Errors.Select(e => e.Description);
        //        throw new BadRequestException("Password Reset Failed", errors);
        //    }

        //    var newToeken = await GenerateJwtToken(user!);

        //    return new AuthModelDto
        //    {
        //        Message = "Password Reset Successful",
        //        Name = user!.UserName!,
        //        Email = user.Email!,
        //        IsAuthenticated = true,
        //        Token = newToeken.Token,
        //        ExpiresOn = newToeken.ExpiresOn,
        //        Roles = (await userManager.GetRolesAsync(user)).ToList()
        //    };

        //}

        //public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        //{
        //    // check if exist email 
        //    var user = await userManager.FindByEmailAsync(forgetPasswordRequestDto.Email);
        //    if (user == null) throw UserNotFoundException.ByEmail(forgetPasswordRequestDto.Email);

        //    // generate new otp
        //    var newOtp = await GenerateAndSaveOtpAsync(user.Id, forgetPasswordRequestDto.Email);

        //    // send the otp to user email
        //    await emailService.SendEmailAsync(user.Email!, "Password Reset OTP", $"Your OTP code is: {newOtp}");

        //    return true;

        //}

        //public async Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email)
        //{
        //    // get user 
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null) throw UserNotFoundException.ByEmail(email);

        //    // check status
        //    if (user.UserStatus == UserStatus.Pending || user.UserStatus == UserStatus.Active)
        //        throw new BadRequestException("You cannot update your identity while it is pending or Active.");

        //    // get user repository
        //    var userRepository = unitOfWork.GetRepo<ApplicationUser, string>();

        //    // generate specification to check national id
        //    var userSpecification = new UserSpecifications(verifyIdentityDto.NationalId! , user.Id);

        //    // check if national id is already used
        //    var isDuplicated = await userRepository.GetByIdWithSpecificationsAsync(userSpecification);
        //    if (isDuplicated != null) throw new BadRequestException("National ID is already in use by another user.");

        //    // delete other images
        //    if (!string.IsNullOrEmpty(user.NationalIdImageFrontUrl))
        //        await attachementServices.DeleteImage(user.NationalIdImageFrontUrl);

        //    if (!string.IsNullOrEmpty(user.NationalIdImageBackUrl))
        //        await attachementServices.DeleteImage(user.NationalIdImageBackUrl);

        //    // upload front & back image
        //    var ImagePath = Path.Combine("Users", user.Id, "Identity");

        //    string frontImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageFrontUrl!, ImagePath);
        //    string backImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageBackUrl!, ImagePath);

        //    // update user info
        //    user.NationalId = verifyIdentityDto.NationalId;
        //    user.NationalIdImageFrontUrl = frontImagePath;
        //    user.NationalIdImageBackUrl = backImagePath;
        //    user.UserStatus = UserStatus.Pending;

        //    var result = await userManager.UpdateAsync(user);

        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        throw new BadRequestException("Identity Verification Failed: ", errors);
        //    }

        //    var admins = await userManager.GetUsersInRoleAsync("Admin");

        //    foreach (var admin in admins)
        //    {
        //        await notificationService.SendNotificationAsync(
        //            NotificationFactory.DocumentVerificationRequest(
        //                admin.Id,
        //                user.Name,
        //                user.Id
        //            )
        //        );
        //    }
        //    return true;
        //}
        //public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        //{
        //    await VerifyAndBurnOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp, false);

        //    return true;
        //}

        //public async Task<AuthModelDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto , string email)
        //{
        //    //first get the user
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        throw UserNotFoundException.ByEmail(email);
        //    // then change the password
        //    var result = await userManager
        //        .ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        throw new BadRequestException("Password Change Failed", errors);
        //    }

        //    var newToken = await GenerateJwtToken(user);

        //    var roles = await userManager.GetRolesAsync(user);

        //    return new AuthModelDto
        //    {
        //        Message = "Password Changed Successfully",
        //        Name = user.UserName!,
        //        Email = user.Email!,
        //        IsAuthenticated = true,
        //        Token = newToken.Token,
        //        ExpiresOn = newToken.ExpiresOn,
        //        Roles = roles.ToList()
        //    };

        //}

        #region Private Methods        
        //private async Task<(string Token, DateTime ExpiresOn)> GenerateJwtToken(ApplicationUser user)
        //{
        //    var roles = await userManager.GetRolesAsync(user);
        //    // first create list of claims 
        //    var UserClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.UserName!),
        //        new Claim(ClaimTypes.Email, user.Email!),
        //        new Claim(ClaimTypes.NameIdentifier, user.Id)
        //    };

        //    // Add roles to claims
        //    foreach (var role in roles)
        //    {
        //        UserClaims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    //get the Security Key from app settings
        //    var SecurityKey = configuration.GetSection("JWTOptions:SecurityKey").Value;

        //    // create the Key and Signing Credentials
        //    var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        //    var SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        //    // expire time for the token
        //    var expireTime = DateTime.UtcNow.AddHours(12);
        //    // Create the token
        //    var Token = new JwtSecurityToken(
        //        issuer: configuration.GetSection("JWTOptions:Issuer").Value,
        //        audience: configuration.GetSection("JWTOptions:Audience").Value,
        //        claims: UserClaims,
        //        expires: expireTime,
        //        signingCredentials: SigningCredentials
        //    );
        //    return (new JwtSecurityTokenHandler().WriteToken(Token), expireTime);
        //}

        //private async Task<string> GenerateAndSaveOtpAsync(string UserId, string email)
        //{
        //    // get the specification 
        //    var Specification = new UserOtpSpecifications(email);

        //    // get Repo 
        //    var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();

        //    // get the user old otp by specification
        //    var oldUserOtp = await userOtpRepo.GetAllWithSpecificationAsync(Specification);

        //    // if exist update to used
        //    foreach (var otp in oldUserOtp)
        //    {
        //        otp.IsUsed = true;
        //        userOtpRepo.Update(otp);
        //    }

        //    // generate new otp
        //    var newOtp = new Random().Next(100000, 999999).ToString();

        //    // save it to db with expire time 5 minutes\
        //    var userOtp = new UserOtp
        //    {
        //        Email = email,
        //        OtpCode = newOtp,
        //        ExpirationTime = DateTime.UtcNow.AddMinutes(1),
        //        IsUsed = false,
        //        UserId = UserId,
        //        LastModifiedBy = UserId,
        //        UpdatedAt = DateTime.UtcNow
        //    };

        //    userOtpRepo.AddAsync(userOtp);
        //    await unitOfWork.SaveChangesAsync();

        //    return newOtp;
        //}

        //private async Task<UserOtp> VerifyAndBurnOtpAsync(string email, string otp, bool burnIt)
        //{

        //    var userOtpRepo = unitOfWork.GetRepo<UserOtp, int>();

        //    var specification = new UserOtpSpecifications(email);

        //    var existOtp = await userOtpRepo.GetByIdWithSpecificationsAsync(specification);

        //    // check if not exist 
        //    if (existOtp == null)
        //    {
        //        throw new BadRequestException("Invalid Otp");
        //    }

        //    // check if expired
        //    if (existOtp.ExpirationTime < DateTime.UtcNow)
        //    {
        //        existOtp.IsUsed = true;
        //        userOtpRepo.Update(existOtp);
        //        await unitOfWork.SaveChangesAsync();
        //        throw new BadRequestException("Expired Otp");
        //    }

        //    // check if invalid 
        //    if (existOtp.OtpCode != otp)
        //    {
        //        existOtp.FailedAttempts += 1;
        //        if (existOtp.FailedAttempts >= 3)
        //        {
        //            existOtp.IsUsed = true;
        //            userOtpRepo.Update(existOtp);
        //            await unitOfWork.SaveChangesAsync();
        //            throw new BadRequestException("Maximum attempts reached. Please request a new OTP.");
        //        }
        //        userOtpRepo.Update(existOtp);
        //        await unitOfWork.SaveChangesAsync();
        //        throw new BadRequestException($"Invalid Otp. You have {3 - existOtp.FailedAttempts} attempts left.");
        //    }

        //    // if valid and burn it after verification
        //    existOtp.IsUsed = burnIt;
        //    userOtpRepo.Update(existOtp);
        //    await unitOfWork.SaveChangesAsync();

        //    return existOtp;

        //}
        #endregion

        #endregion


    }
}