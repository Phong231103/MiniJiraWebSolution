using Web.Domain.Repository;

namespace Web.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        // Có thể hash từ chuỗi UserAgent + OS + DeviceName
        public string DeviceFingerprint { get; set; } = string.Empty;

        public string? DeviceName { get; set; }

        // Đối tượng chứa thông tin chi tiết của thiết bị (IP, OS, Browser...)
        public string? DeviceInfoJson { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Trạng thái phiên đăng nhập hiện tại
        public bool IsActive { get; set; } = true;

        // Ghi nhớ thiết bị
        public bool IsTrusted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        // Navigation property (nếu dùng EF Core)
        public virtual User? User { get; set; }

        public static Device Create(string username, string email, string plainPassword, IPasswordHasher passwordHasher)
        {
            var device = new Device
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email
            };

            // Tự băm và gán mật khẩu ngay khi tạo
            user.SetPassword(plainPassword, passwordHasher);
            return user;
        }

    }

    // Lớp mô tả thông tin thiết bị (dùng trong code, trước khi lưu thành JSON)
    public class DeviceInfo
    {
        public string? IPAddress { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Browser { get; set; }
        public string? UserAgent { get; set; }
    }
}