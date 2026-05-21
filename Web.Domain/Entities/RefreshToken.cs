namespace Web.Domain.Entities
{
    // Domain/Entities/RefreshToken.cs
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime Expires { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime Created { get; private set; }
        public string? ReplacedByToken { get; private set; }

        // Khóa ngoại
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        // Constructor cho EF Core
        private RefreshToken() { }

        // Factory method để tạo Refresh Token mới
        public static RefreshToken Create(Guid userId, string token, DateTime expires)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                Expires = expires,
                IsRevoked = false,
                Created = DateTime.UtcNow
            };
        }

        // Hành vi: Thu hồi token
        public void Revoke()
        {
            IsRevoked = true;
        }

        // Kiểm tra còn hiệu lực không
        public bool IsActive => !IsRevoked && Expires > DateTime.UtcNow;
    }
}
