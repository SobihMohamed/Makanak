using AutoMapper;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServicesContract.Token;
using Makanak.Domain.Exceptions;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Shared.Dto_s;
using Makanak.Shared.Dto_s.Authentication;
using Makanak.Shared.Dto_s.Token;

using Microsoft.AspNetCore.Identity;


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
       

    }
}