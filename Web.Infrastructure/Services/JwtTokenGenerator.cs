using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;

namespace Web.Infrastructure.Services
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; } = 60; // Token hết hạn sau 60 phút
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        // Dùng IOptions<JwtSettings> để inject cấu hình vào
        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user, bool isEmailToken)
        {
            // 1. Tạo Signing Key từ chuỗi Secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Định nghĩa các Claims (Thông tin đính kèm trong Token)

            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Email, user.Email) };

            if (!isEmailToken)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.FullName));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                claims.Add(new Claim("ProfileCompleted", "true"));
                claims.Add(new Claim("TokenType", "FullyAcess"));

                // Thêm Roles vào claims (Nếu User có Roles)
                // Lưu ý: ClaimTypes.Role giúp .NET tự động map với [Authorize(Roles = "Admin")]
                if (user.Roles != null)
                {
                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.DisplayName));
                    }
                }
            }
            else
            {
                claims.Add(new Claim("ProfileCompleted", "false"));
                claims.Add(new Claim("TokenType", "Provisional"));
            }

            // 3. Tạo cấu trúc Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            // 4. Sinh Token và viết ra chuỗi string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            // Sinh chuỗi ngẫu nhiên 64 byte bằng class Cryptographic chuẩn của .NET
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
