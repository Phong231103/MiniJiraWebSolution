namespace Web.Domain.Entities
{
    public class Device
    {
        private readonly List<RefreshToken> _refreshTokens = [];

        public sealed record DeviceMetadata(string Browser, string OperatingSystem, string UserAgent, string? Fingerprint, string? IpAddress, string? Timezone);

        private Device()
        {
        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string? Fingerprint { get; private set; }

        public string Browser { get; private set; } = string.Empty;

        public string OperatingSystem { get; private set; } = string.Empty;

        public string UserAgent { get; private set; } = string.Empty;

        public string? LastIpAddress { get; private set; }

        public string? Timezone { get; private set; }

        public string? DeviceName { get; private set; }

        /// <summary>
        /// Người dùng đã đánh dấu tin cậy
        /// </summary>
        public bool IsTrusted { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime LastSeen { get; private set; }

        public DateTime LastLogin { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public DateTime? RevokedAt { get; private set; }

        public string? RevokedReason { get; private set; }

        public string DeviceIdentifier { get; private set; }

        public virtual User? User { get; private set; }

        public IReadOnlyCollection<RefreshToken> RefreshTokens
            => _refreshTokens.AsReadOnly();

        #region Factory

        public static Device Create(
            Guid userId,
            string deviceIdentifier,
            string fingerprint,
            string browser,
            string operatingSystem,
            string userAgent,
            string? ipAddress,
            string? timezone,
            string? deviceName = null) => new Device
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DeviceIdentifier = deviceIdentifier,
                Fingerprint = fingerprint,
                Browser = browser,
                OperatingSystem = operatingSystem,
                UserAgent = userAgent,
                LastIpAddress = ipAddress,
                Timezone = timezone,
                DeviceName = deviceName ?? $"{browser} on {operatingSystem}",

                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,

                IsTrusted = false,

            };

        #endregion

        #region Authentication

        public void Login(DeviceMetadata deviceMetadata)
        {
            Browser = deviceMetadata.Browser;
            OperatingSystem = deviceMetadata.OperatingSystem;
            UserAgent = deviceMetadata.UserAgent;
            Fingerprint = deviceMetadata.Fingerprint;
            LastIpAddress = deviceMetadata.IpAddress;
            Timezone = deviceMetadata.Timezone;

            LastLogin = DateTime.UtcNow;
            LastSeen = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public void CheckOnActiveDevice(string? ipAddress)
        {
            LastSeen = DateTime.UtcNow;
            LastIpAddress = ipAddress;
            UpdatedDate = DateTime.UtcNow;
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

        public void UpdateFingerprint(string fingerprint)
        {
            Fingerprint = fingerprint;
            UpdatedDate = DateTime.UtcNow;
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
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveTrust()
        {
            IsTrusted = false;
            UpdatedDate = DateTime.UtcNow;
        }

        #endregion

        #region Refresh Token

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            ArgumentNullException.ThrowIfNull(refreshToken);

            _refreshTokens.Add(refreshToken);

            LastSeen = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public void RevokeAllRefreshTokens()
        {
            foreach (var token in _refreshTokens.Where(x => x.IsActive))
            {
                token.Revoke();
            }

            UpdatedDate = DateTime.UtcNow;
        }

        public bool HasActiveSession()
        {
            return _refreshTokens.Any(x => x.IsActive);
        }

        public RefreshToken? GetCurrentRefreshToken()
        {
            return _refreshTokens
                .SingleOrDefault(x => x.IsActive);
        }

        #endregion
    }
}