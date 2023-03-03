using BlogWebApi.Application.Interfaces.Services;
using BlogWebApi.Contracts.Commons;
using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using BlogWebApi.Domain.Entities;
using BlogWebApi.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApi.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public UserManager<User> UserManager { get; set; }
        private readonly JwtSettings _jwtSettings;

        public UserService(UserManager<User> userManager,
                                 IOptions<JwtSettings> jwtOptions)
        {
            this.UserManager = userManager;
            this._jwtSettings = jwtOptions.Value;
        }



        public async Task<DataResponseDTO<LoginResponseDTO>> Register (RegisterRequestDTO model)
        {
            var findUser = await UserManager.FindByEmailAsync(model.email);

            if (findUser != null)
                throw new BadRequestException("Email already exist", HttpStatusCode.BadRequest);

            var newUser = new User
            {
                FirstName = model.firstName,
                LastName = model.lastName,
                Email = model.email,
                UserName = model.email,
                CreateAt = DateTime.UtcNow,
                EmailConfirmed = true,
            };

            var result = await UserManager.CreateAsync(newUser, model.password);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault().Description, HttpStatusCode.BadRequest);

            await AddUserToRoleAsync(newUser, model.role);

            return GenerateToken(newUser);
        }



        public async Task<DataResponseDTO<LoginResponseDTO>> Login(LoginRequestDTO model)
        {
            var findUser = await UserManager.FindByEmailAsync(model.email);

            if (findUser == null)
                throw new BadRequestException("Invalid credential", HttpStatusCode.BadRequest);

            var result = await UserManager.CheckPasswordAsync(findUser, model.password);

            if (!result)
                throw new BadRequestException("Invalid credential", HttpStatusCode.BadRequest);

            return GenerateToken(findUser);
        }


        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            var addUserRole = await UserManager.AddToRoleAsync(user, roleName);

            if (!addUserRole.Succeeded)
                throw new BadRequestException("Error occur while adding user to role", HttpStatusCode.BadRequest);
        }


        public DataResponseDTO<LoginResponseDTO> GenerateToken(User user)
        {
            var signingCredential = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    SecurityAlgorithms.HmacSha256
                );

            DateTime Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            var userRoles = this.UserManager.GetRolesAsync(user).Result.ToList();
            IdentityOptions identityOptions = new IdentityOptions();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(identityOptions.ClaimsIdentity.RoleClaimType, role));
            }

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: Expires,
                claims: claims,
                signingCredentials: signingCredential
            );

            return new DataResponseDTO<LoginResponseDTO>(new LoginResponseDTO
            {
                token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                expiryTime = Expires.ToString()
            });
        }
    }
}