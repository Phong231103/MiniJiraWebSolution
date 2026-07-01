namespace Web.Application.Common.Interfaces
{
    public interface IDeviceIdentifyGenerator
    {
        string GenerateDeviceIdentifier(string fingerprint, string browser, string operatingSystem, string? timezone);
    }
}
