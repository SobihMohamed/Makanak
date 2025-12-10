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
    public class AuthService(UserManager<ApplicationUser> userManager , IConfiguration configuration) : IAuthService
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
                Roles = roles as List<string> ?? new List<string>()
            };
            return AuthModel;
        }

        public Task<AuthModelDto> RegisterAsync(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }
        public Task<string> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentUserDto> GetUserProfileAsync(string email)
        {
            throw new NotImplementedException();
        }


        public Task<AuthModelDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            throw new NotImplementedException();
        }

        public Task<AuthModelDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto, string email)
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
