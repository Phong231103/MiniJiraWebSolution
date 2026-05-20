namespace Web.Application.Auth.Models
{
    public class PendingRegistration
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PlainPassword { get; set; } = string.Empty; // Chỉ lưu tạm để tạo hash sau
        public string FullName { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
