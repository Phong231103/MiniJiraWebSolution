namespace Web.Domain.Entities;

using System;
using Web.Domain.Common;
using Web.Domain.Repository;

public class User : BaseEntity
{
    public Guid Id { get; private set; } // Bỏ dấu ? đi, PK không được null
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; } // Thêm ? vì có thể không có SĐT
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation property
    public ICollection<Role> Roles { get; private set; } = new List<Role>();

    // --- Các thuộc tính bảo mật ---
    public string? PasswordHash { get; private set; } // Có thể null nếu user chưa set pass
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiry { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    private User() { }

    public static User Create(string username, string email, string fullName, string plainPassword, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            FullName = fullName,
            IsActive = true
        };

        // Tự băm và gán mật khẩu ngay khi tạo
        user.SetPassword(plainPassword, passwordHasher);
        return user;
    }

    // Hành vi: Đặt mật khẩu
    public void SetPassword(string plainPassword, IPasswordHasher passwordHasher)
    {
        PasswordHash = passwordHasher.HashPassword(plainPassword);
    }

    // Hành vi: Xác thực mật khẩu
    public bool ValidatePassword(string plainPassword, IPasswordHasher passwordHasher)
    {
        if (PasswordHash is null) return false;
        return passwordHasher.VerifyPassword(plainPassword, PasswordHash);
    }

    // Hành vi: Đăng nhập thất bại
    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5) // Khóa tài khoản sau 5 lần
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
    }

    // Hành vi: Reset số lần đăng nhập thất bại
    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }
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