using System.Text;

namespace Web.Infrastructure.Services
{
    public class DeviceIdentifierGenerator
    {
        public string GenerateDeviceIdentifier(string fingerprint, string browser, string operatingSystem, string? timezone)
        {
            // Combine the device metadata into a single string
            var combinedString = $"{fingerprint}-{browser}-{operatingSystem}-{timezone}";
            // Generate a hash of the combined string to create a unique identifier
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
