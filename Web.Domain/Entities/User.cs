namespace Web.Domain.Entities;

using System;
using Web.Domain.Common;
using Web.Domain.Primitives;
using Web.Domain.Repository;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsProfileCompleted => !string.IsNullOrWhiteSpace(FullName) && (IsActive = true);

    // Navigation property
    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<Device> Devices { get; private set; } = new List<Device>();

    // --- Các thuộc tính bảo mật ---
    public string? PasswordHash { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public int LockoutCount { get; private set; } = 0;
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;


    private User() { }

    public static User Create(string username, string email, string plainPassword, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            Error.Required("400", "Password cannot be empty.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email
        };

        // Tự băm và gán mật khẩu ngay khi tạo
        user.SetPassword(plainPassword, passwordHasher);
        return user;
    }

    // Đặt mật khẩu
    public void SetPassword(string plainPassword, IPasswordHasher passwordHasher)
    {
        PasswordHash = passwordHasher.HashPassword(plainPassword);
    }

    // Xác thực mật khẩu
    public bool ValidatePassword(string plainPassword, IPasswordHasher passwordHasher)
    {
        if (PasswordHash is null)
        {
            return false;
        }

        return passwordHasher.VerifyPassword(plainPassword, PasswordHash);
    }

    public void UpdateProfile(string fullName, string? phoneNumber, string? avatarUrl)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AvatarUrl = avatarUrl;
        IsActive = true;
    }

    // Đăng nhập thất bại
    public void RecordFailedLogin(int maxAttempts = 5)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= maxAttempts)
        {
            // Lockout tăng dần (Exponential Backoff)
            int lockMinutes = Math.Min(15 * (int)Math.Pow(2, LockoutCount), 1440); // max 24h
            LockoutEnd = DateTime.UtcNow.AddMinutes(lockMinutes);
            LockoutCount++;
        }

        if (LockoutCount == 3)
        {
            IsActive = false; // Vô hiệu hóa tài khoản sau 3 lần lockout
        }
    }

    // Reset số lần đăng nhập thất bại
    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    // Thêm Refresh Token mới
    public void AddRefreshToken(Guid deviceId, string token, DateTime expires)
    {
        var refreshToken = RefreshToken.Create(Id, deviceId, token, expires);
        RefreshTokens.Add(refreshToken);
    }

    // Thu hồi Refresh Token cũ (Dùng trong luồng Rotation)
    public void RevokeRefreshToken(string token)
    {
        var existingToken = RefreshTokens.FirstOrDefault(t => t.Token == token);
        if (existingToken != null && existingToken.IsActive)
        {
            existingToken.Revoke();
        }
    }
}