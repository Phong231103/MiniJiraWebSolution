using MediatR;
using Microsoft.AspNetCore.Http;
using Web.Application.Auth.DTOs;
using Web.Application.Auth.Enums;
using Web.Application.Auth.Models;
using Web.Application.Auth.Request;
using Web.Application.Auth.Response;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Primitives;
using Web.Domain.Repository;

namespace Web.Application.Auth.Commands
{
    public record VerifyOtpCommand(OtpInfo OtpInfo, DeviceMetadataRequest DeviceMetadata) : IRequest<Result<AuthResponse>>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<AuthResponse>>
    {
        public readonly ICacheService _cacheService;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IDeviceManagementService _deviceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerifyOtpCommandHandler(ICacheService cacheService, IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, IDeviceManagementService deviceService, IHttpContextAccessor httpContextAccessor)
        {
            _cacheService = cacheService;
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _deviceService = deviceService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"otp_{request.OtpInfo.OtpType}_{request.OtpInfo.OtpId}";

            var pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);

            switch (request.OtpInfo.OtpType)
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
            if (pendingData.Otp != request.OtpInfo.Otp)
            {
                // Tăng số lần thử sai
                pendingData.OtpFaildeAttemp++;
                await _cacheService.SetAsync(cacheKey, pendingData, TimeSpan.FromMinutes(5), cancellationToken);

                return Error.Validation("Auth.InvalidOtp", "Invalid OTP.");
            }

            if (request.OtpInfo.OtpType == (int)OtpCodeType.FirstTimeRegistration)
            {
                var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

                var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

                var user = User.Create(
                pendingData.Username,
                pendingData.Email,
                pendingData.Plainpassword,
                _passwordHasher);

                var device = Device.Create(
                    user.Id,
                    request.DeviceMetadata.Fingerprint,
                    request.DeviceMetadata.Browser,
                    request.DeviceMetadata.OperatingSystem,
                    userAgent,
                    ip,
                    request.DeviceMetadata.Timezone
                );

                _context.Users.Add(user);
                _context.Devices.Add(device);

                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (request.OtpInfo.OtpType == (int)OtpCodeType.NewDeviceVerification)
            {
                var user = _context.Users.SingleOrDefault(x => x.Email == pendingData.Email);

                var newDevice = Device.Create(
                    user!.Id,
                    pendingData.Username,

                    );

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
