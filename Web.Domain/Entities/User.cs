namespace Web.Domain.Entities;

using Web.Domain.Common;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiry { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool PhoneNumberConfirmed { get; private set; }
    public string? AvatarUrl { get; set; }

    // 2. Password Service (Argon2/BCrypt)
    //public class PasswordService
    //{
    //    public string HashPassword(string password)
    //    {
    //        return BCrypt.Net.BCrypt.HashPassword(password, 14); // OWASP recommended
    //    }

    //    public bool Verify(string password, string hash)
    //    {
    //        return BCrypt.Net.BCrypt.Verify(password, hash);
    //    }
    //}
}



// 3. JWT Service (Production secure)
//public class JwtService
//{
//    public string GenerateToken(User user)
//    {
//        var token = new JwtSecurityToken(
//            claims: GetClaims(user),
//            expires: DateTime.UtcNow.AddMinutes(15), // Short-lived
//            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
//        );
//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}