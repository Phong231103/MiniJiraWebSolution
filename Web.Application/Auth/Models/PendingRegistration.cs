namespace Web.Application.Auth.Models
{
    public class PendingRegistration
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Plainpassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public int OtpType { get; set; }
        public int OtpFaildeAttemp { get; set; }
    }
}
