using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Authentication.Password;
using Makanak.Shared.Dto_s.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Services.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager , IAttachementServices attachementServices , IConfiguration configuration , IMapper mapper) : IAuthService
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
                string imagePath = await attachementServices.UploadImageAsync(updateProfileDto.ProfilePicture , $"{updateProfileDto.Name}_{email}");
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
        public Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            throw new NotImplementedException();
        }
        public Task<string> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            throw new NotImplementedException();
        }
        public Task<string> VerifyIdentityAsync(VerifyIdentityDto verifyIdentityDto, string email)
        {
            throw new NotImplementedException();
        }
        public Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            throw new NotImplementedException();
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
            var expireTime = DateTime.Now.AddHours(12);
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
