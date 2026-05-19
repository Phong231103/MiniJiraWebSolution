using Web.Domain.Repository;

namespace Web.Infrastructure.Shared
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // WorkFactor càng cao càng bảo mật nhưng càng chậm
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
