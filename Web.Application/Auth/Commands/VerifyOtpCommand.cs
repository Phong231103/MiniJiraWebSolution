using MediatR;
using Web.Application.Auth.DTOs;
using Web.Application.Auth.Enums;
using Web.Application.Auth.Models;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Primitives;
using Web.Domain.Repository;

namespace Web.Application.Auth.Commands
{
    public record VerifyOtpCommand(string OtpId, string Otp, int OtpType) : IRequest<Result<AuthResponse>>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<AuthResponse>>
    {
        public readonly ICacheService _cacheService;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IDeviceManagementService _deviceService;

        public VerifyOtpCommandHandler(ICacheService cacheService, IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, IDeviceManagementService deviceService)
        {
            _cacheService = cacheService;
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _deviceService = deviceService;
        }

        public async Task<Result<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"otp_{request.OtpType}_{request.OtpId}";

            var pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);

            switch (request.OtpType)
            {
                case (int)OtpCodeType.FirstTimeRegistration:
                    break;

                case (int)OtpCodeType.NewDeviceVerification:
                    pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);
                    break;
                case (int)OtpCodeType.ForgotPassword:
                    pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);
                    break;
                case (int)OtpCodeType.DeleteAccount:
                    pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);
                    break;
            }

            if (pendingData is null)
            {
                return Error.Validation("Auth.OtpExpired", "OTP has expired or session not found.");
            }

            if (pendingData.OtpFaildeAttemp >= 3)
            {
                return Error.Validation("Auth.OtpLocked", "OTP has been locked due to too many failed attempts.");
            }

            // Otp check
            if (pendingData.Otp != request.Otp)
            {
                // Tăng số lần thử sai
                pendingData.OtpFaildeAttemp++;
                await _cacheService.SetAsync(cacheKey, pendingData, TimeSpan.FromMinutes(5), cancellationToken);

                return Error.Validation("Auth.InvalidOtp", "Invalid OTP.");
            }

            if (request.OtpType == (int)OtpCodeType.FirstTimeRegistration)
            {
                var user = User.Create(
                pendingData.Username,
                pendingData.Email,
                pendingData.Plainpassword,
                _passwordHasher);

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (request.OtpType == (int)OtpCodeType.NewDeviceVerification)
            {
                var user = _context.Users.SingleOrDefault(x => x.Email == pendingData.Email);



                _deviceService.LoginOnNewDevice(user!.Id, )
            }
            // Dọn dẹp Cache (Để không thể dùng lại OTP)
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            // 5. Generate JWT Token trả về cho Client
            var token = _tokenGenerator.GenerateToken(user);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var response = new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Email = user.Email,
                UserId = user.Id
            };

            return Result<AuthResponse>.Success(response, "Validate OTP Success");
        }

            return Result<AuthResponse>.Success(true, "OTP is valid");
        }
}
}
