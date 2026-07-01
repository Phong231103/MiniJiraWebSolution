using Web.Domain.Entities;

namespace Web.Application.Auth.Response
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public RefreshToken RefreshToken { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
