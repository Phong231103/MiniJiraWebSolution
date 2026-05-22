namespace Web.Application.Auth.DTOs
{
    public class SpecialTokenResponse
    {
        public string EmailToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
