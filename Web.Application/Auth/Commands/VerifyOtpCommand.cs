using MediatR;
using Web.Application.Auth.Models;
using Web.Application.Common.Interfaces;
using Web.Domain.Primitives;

namespace Web.Application.Auth.Commands
{
    public record VerifyOtpCommand(string RegistrationId, string Otp) : IRequest<Result<bool>>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<bool>>
    {
        public readonly ICacheService _cacheService;
        private readonly IApplicationDbContext _context;

        public VerifyOtpCommandHandler(ICacheService cacheService, IApplicationDbContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<Result<bool>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"reg_{request.RegistrationId}";

            var pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);

            // Nếu không tìm thấy -> OTP hết hạn hoặc RegistrationId giả
            if (pendingData is null)
            {
                return Error.Validation("Auth.OtpExpired", "OTP has expired or registration session not found.");
            }

            // 2. Kiểm tra OTP
            if (pendingData.Otp != request.Otp)
            {
                return Error.Validation("Auth.InvalidOtp", "Invalid OTP.");
            }

            return Result<bool>.Success(true, "OTP is valid");
        }
    }
}
