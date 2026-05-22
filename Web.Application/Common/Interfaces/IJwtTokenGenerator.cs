using Web.Domain.Entities;

namespace Web.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, bool isEmailToken);

        string GenerateRefreshToken();
    }
}
