using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.EnumsHelper.User;
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

namespace Makanak.Services.Services
{
    public class AuthService(IUnitOfWork unitOfWork, IUserRepository userRepository , UserManager<ApplicationUser> userManager ,
        IAttachementServices attachementServices , IConfiguration configuration ,
        IMapper mapper , IEmailService emailService) 
        : IAuthService
    {
        public async Task<AuthModelDto> LoginAsync(LoginDto loginDto)
        {
            //check if user exist by email
            var User = await userManager.FindByEmailAsync(loginDto.Email);
            if (User == null)
            {
                //Throw New UserNotFoundException 
                throw new Exception("User not found");
            }
            var isPasswordValid = await userManager.CheckPasswordAsync(User!, loginDto.Password);
            if (!isPasswordValid)
            {
                //Throw New UnAuthorizedException
                throw new Exception("Invalid Password");
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
                return new AuthModelDto
                {
                    Message = "Email is already registered",
                    IsAuthenticated = false
                };
            }
            // Map RegisterDto to ApplicationUser
            var mappedUser =  mapper.Map< RegisterDto,ApplicationUser>(registerDto);

            // create user
            var result = await userManager.CreateAsync(mappedUser, registerDto.Password);
            if (!result.Succeeded)
            {
                await userManager.DeleteAsync(mappedUser);

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthModelDto
                {
                    Message = $"User registration failed: {errors}",
                    IsAuthenticated = false
                };
            }
            
            // Assign Role to User
            var roleResult = await userManager.AddToRoleAsync(mappedUser, registerDto.UserType.ToString());
            if (!roleResult.Succeeded) 
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return new AuthModelDto
                {
                    Message = $"Role assignment failed: {errors}",
                    IsAuthenticated = false
                };
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
                throw new Exception("User not found");
            }
            if (updateProfileDto.ProfilePicture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                  await attachementServices.DeleteImage(user.ProfilePictureUrl);
                }
                string imagePath = await attachementServices.UploadImageAsync(updateProfileDto.ProfilePicture , $"{user.Id}");
                user.ProfilePictureUrl = imagePath;
            }
            mapper.Map(updateProfileDto, user); // assign values and save in user
            // update the user
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) 
            {
                var errors = string.Join(", " , result.Errors.Select(e=>e.Description));
                throw new Exception("Profile Update Faild " + errors);
            }
            var currentUserMapper = mapper.Map<ApplicationUser, CurrentUserDto>(user);
            if (currentUserMapper == null)
                throw new Exception("Error During Mapping");
            return currentUserMapper;
        }
        public async Task<CurrentUserDto> GetUserProfileAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
              // Not Found Exception User
              throw new Exception("User not found");
            }
            var currentUserDto = mapper.Map<ApplicationUser, CurrentUserDto>(user);
            if(currentUserDto == null)
               throw new Exception("Mapping failed");
            return currentUserDto;
        }
        public async Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var UserOtpRepo = unitOfWork.GetRepo<UserOtp,int>();
            
            var Specification = new VerifyUserOtpSpecification(resetPasswordDto.Email, resetPasswordDto.Otp);

            var UserOtp = await UserOtpRepo.GetByIdWithSpecificationsAsync(Specification);

            if (UserOtp == null)
            {
                throw new Exception("Invalid Otp");
            }
            if (UserOtp.ExpirationTime < DateTime.UtcNow)
            {
                throw new Exception("Expired Otp");
            }

            if(await userManager.HasPasswordAsync(user!))
            {
                await userManager.RemovePasswordAsync(user!);
            }
            var resetPassResult = await userManager.AddPasswordAsync(user!, resetPasswordDto.NewPassword);

            if (!resetPassResult.Succeeded)
            {
                var errors = string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
                throw new Exception("Password Reset Failed: " + errors);
            }

            UserOtp.IsUsed = true;
            UserOtpRepo.Update(UserOtp);

            await unitOfWork.SaveChangesAsync();

            var newToeken = await GenerateJwtToken(user!);
            return new AuthModelDto
            {
                Message = "Password Reset Successful",
                Name = user!.UserName!,
                Email = user.Email!,
                IsAuthenticated = true,
                Token = newToeken.Token,
                ExpiresOn = newToeken.ExpiresOn
            };

        }
        public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            // check if exist email 
            var user = await userManager.FindByEmailAsync(forgetPasswordRequestDto.Email);
            if (user == null)
            {
                // Throw New UserNotFoundException
                throw new Exception("User not found");
            }
            // get the specification 
            var Specification = new UserOtpSpecification(user.Email!);

            // get Repo 
            var userOtpRepo = unitOfWork.GetRepo<UserOtp,int>();

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
                Email = user.Email!,
                OtpCode = newOtp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                UserId = user.Id
            };

            userOtpRepo.AddAsync(userOtp);
            await unitOfWork.SaveChangesAsync();

            // send the otp to user email
            await emailService.SendEmailAsync(user.Email!, "Password Reset OTP", $"Your OTP code is: {newOtp}");

            return true;


        }
        public async Task<bool> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email)
        {
            // get user 
            var user  = await userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            // check status
            if (user.UserStatus == UserStatus.Pending || user.UserStatus == UserStatus.Active)
            {
                throw new Exception("You cannot update your identity while it is pending or Active.");
            }

            // check if national id is already used
            var isDublicated = await userRepository.IsUserNationalIdExistAsync(verifyIdentityDto.NationalId! , user.Id);
            if (isDublicated) throw new Exception("National ID is already in use by another user.");

            // upload front & back image
            string frontImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageFrontUrl!, $"{user.Id}");
            string backImagePath = await attachementServices.UploadImageAsync(verifyIdentityDto.NationalIdImageBackUrl!, $"{user.Id}");

            // update user info
            user.NationalId = verifyIdentityDto.NationalId;
            user.NationalIdImageFrontUrl = frontImagePath;
            user.NationalIdImageBackUrl = backImagePath;
            user.UserStatus = UserStatus.Pending;

            var result = await userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception("Identity Verification Failed: " + errors);
            }

            return true;
        }
        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var email = verifyOtpDto.Email;
            var otp = verifyOtpDto.Otp;

            var userOtpRepo = unitOfWork.GetRepo<UserOtp,int>();

            var specification = new VerifyUserOtpSpecification(email, otp);

            var existOtp = await userOtpRepo.GetByIdWithSpecificationsAsync(specification);

            if (existOtp == null) 
            {
                throw new Exception("Invalid Otp");
            }
            if (existOtp.ExpirationTime < DateTime.UtcNow)
            {
                throw new Exception("Expired Otp");
            }

            return true;

        }
        private async Task<(string Token , DateTime ExpiresOn)> GenerateJwtToken(ApplicationUser user)
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
            return (new JwtSecurityTokenHandler().WriteToken(Token),expireTime);
        }
    }
}