using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(User user)
        {
            SymmetricSecurityKey key = new(ConfigProvider.GetJwtKeyBytes());
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role.ToString())
            ];

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
