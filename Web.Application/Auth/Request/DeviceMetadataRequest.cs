namespace Web.Application.Auth.Request
{
    public sealed record DeviceMetadataRequest(string Fingerprint, string Browser, string OperatingSystem, string? Timezone);
}
