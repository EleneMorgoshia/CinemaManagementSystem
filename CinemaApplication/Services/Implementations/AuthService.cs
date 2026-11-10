using CinemaApplication.Services.Interfaces;
using CinemaDomain.Enums;
using CinemaDomain.Models.DTOs;
using CinemaInfrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApplication.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration; // ამის დახმარებით ვკითხულობ appsettingsს
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        //creating token
        private async Task<AuthResponseDTO> GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expireMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"]);
            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };


            // Get user roles from ASP.NET Core Identity
            var roles = await _userManager.GetRolesAsync(user);

            // Add roles to JWT claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDTO> LoginUserAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            return await GenerateJwtToken(user);
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                throw new KeyNotFoundException("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                var message = string.Join(" ", result.Errors.Select(error => error.Description));
                throw new ArgumentException(message);
            }
            
        }
        
    }
}
