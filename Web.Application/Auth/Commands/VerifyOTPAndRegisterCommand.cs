using MediatR;
using Web.Application.Auth.DTOs;
using Web.Application.Auth.Models;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Primitives;
using Web.Domain.Repository;

namespace Web.Application.Auth.Commands
{
    public record VerifyOTPAndRegisterCommand(
    string RegistrationId,
    string Otp) : IRequest<Result<AuthResponse>>; // AuthResponse chứa Token

    public class VerifyOTPAndRegisterCommandHandler : IRequestHandler<VerifyOTPAndRegisterCommand, Result<AuthResponse>>
    {
        private readonly ICacheService _cacheService;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public VerifyOTPAndRegisterCommandHandler(
            ICacheService cacheService,
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator)
        {
            _cacheService = cacheService;
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResponse>> Handle(VerifyOTPAndRegisterCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu tạm từ Cache bằng RegistrationId
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

            // 3. OTP hợp lệ! Tạo User Entity và lưu vào DB chính thức
            var user = User.Create(
                pendingData.UserName,
                pendingData.Email,
                pendingData.FullName,
                pendingData.PlainPassword,
                _passwordHasher
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken); // Lúc này User mới có Id

            // 4. Dọn dẹp Cache (Để không thể dùng lại OTP)
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            // 5. Generate JWT Token trả về cho Client
            var token = _tokenGenerator.GenerateToken(user);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var response = new AuthResponse
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                RefreshToken = refreshToken
            };

            return Result<AuthResponse>.Success(response);
        }
    }
}
