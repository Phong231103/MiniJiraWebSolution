namespace Web.Application.Auth.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Plainpassword { get; set; } = string.Empty;
    }
}
