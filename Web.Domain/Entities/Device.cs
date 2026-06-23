namespace Web.Domain.Entities
{
    public class Device
    {
        private readonly List<RefreshToken> _refreshTokens = [];

        private Device()
        {
        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        /// <summary>
        /// FingerprintJS VisitorId
        /// Unique theo (UserId, Fingerprint)
        /// </summary>
        public string Fingerprint { get; private set; } = string.Empty;

        public string Browser { get; private set; } = string.Empty;

        public string OperatingSystem { get; private set; } = string.Empty;

        public string UserAgent { get; private set; } = string.Empty;

        public string? LastIpAddress { get; private set; }

        public string? Timezone { get; private set; }

        public string? Language { get; private set; }

        public string? DeviceName { get; private set; }

        /// <summary>
        /// Còn phiên đăng nhập hợp lệ hay không
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Người dùng đã đánh dấu tin cậy
        /// </summary>
        public bool IsTrusted { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime? LastLogin { get; private set; }

        public virtual User? User { get; private set; }

        public IReadOnlyCollection<RefreshToken> RefreshTokens
            => _refreshTokens.AsReadOnly();

        #region Factory

        public static Device Create(
            Guid userId,
            string fingerprint,
            string browser,
            string operatingSystem,
            string userAgent,
            string? ipAddress,
            string? timezone,
            string? language,
            string? deviceName = null)
        {
            return new Device
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Fingerprint = fingerprint,
                Browser = browser,
                OperatingSystem = operatingSystem,
                UserAgent = userAgent,
                LastIpAddress = ipAddress,
                Timezone = timezone,
                Language = language,
                DeviceName = string.IsNullOrWhiteSpace(deviceName)
                    ? $"{browser} on {operatingSystem}"
                    : deviceName,
                IsActive = true,
                IsTrusted = false,
                CreatedDate = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };
        }

        #endregion

        #region Login

        public void Login(
            string? ipAddress,
            string? timezone,
            string? language)
        {
            LastLogin = DateTime.UtcNow;
            LastIpAddress = ipAddress;
            Timezone = timezone;
            Language = language;
            IsActive = true;
        }

        #endregion

        #region Device Info

        public void UpdateDeviceInfo(
            string browser,
            string operatingSystem,
            string userAgent)
        {
            Browser = browser;
            OperatingSystem = operatingSystem;
            UserAgent = userAgent;
        }

        public void Rename(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                throw new ArgumentException(
                    "Device name cannot be empty.");
            }

            DeviceName = deviceName.Trim();
        }

        #endregion

        #region Trust

        public void MarkAsTrusted()
        {
            IsTrusted = true;
        }

        public void RemoveTrust()
        {
            IsTrusted = false;
        }

        #endregion

        #region Session

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        #endregion

        #region Refresh Token

        public void AddRefreshToken(
            RefreshToken refreshToken)
        {
            _refreshTokens.Add(refreshToken);
        }

        #endregion
    }

    public class DeviceInfo
    {
        public string? IPAddress { get; set; }

        public string? OperatingSystem { get; set; }

        public string? Browser { get; set; }

        public string? UserAgent { get; set; }

        public string? DeviceName { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }
    }
}