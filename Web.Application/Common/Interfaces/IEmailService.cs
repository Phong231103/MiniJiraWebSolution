using Web.Domain.Primitives;

namespace Web.Application.Common.Interfaces
{
    public interface IEmailService
    {
        // Trả về Result để Handler biết việc gửi email thành công hay thất bại
        Task<Result> SendOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default);
    }
}
