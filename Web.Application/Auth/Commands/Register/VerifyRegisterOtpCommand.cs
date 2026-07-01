using MediatR;
using Microsoft.AspNetCore.Http;
using Web.Application.Auth.Models;
using Web.Application.Auth.Request;
using Web.Application.Auth.Response;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Primitives;
using Web.Domain.Repository;

namespace Web.Application.Auth.Commands.Register
{
    public record VerifyRegisterOtpCommand(string Otp, string RegistrationId, string OtpType, DeviceMetadataRequest DeviceMetadata) : IRequest<Result<AuthResponse>>;

    public class VerifyRegisterOtpCommandHandler : IRequestHandler<VerifyRegisterOtpCommand, Result<AuthResponse>>
    {
        public readonly ICacheService _cacheService;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeviceIdentifyGenerator _deviceIdentifyGenerator;

        public VerifyRegisterOtpCommandHandler(ICacheService cacheService, IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, IHttpContextAccessor httpContextAccessor, IDeviceIdentifyGenerator deviceIdentifyGenerator)
        {
            _cacheService = cacheService;
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _httpContextAccessor = httpContextAccessor;
            _deviceIdentifyGenerator = deviceIdentifyGenerator;
        }

        public async Task<Result<AuthResponse>> Handle(VerifyRegisterOtpCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"otp_{request.OtpType}_{request.RegistrationId}";

            var pendingData = await _cacheService.GetAsync<PendingRegistration>(cacheKey, cancellationToken);

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

            var user = User.Create(
            pendingData.Username,
            pendingData.Email,
            pendingData.Plainpassword,
            _passwordHasher);

            var deviceIdentify = _deviceIdentifyGenerator.GenerateDeviceIdentifier(
                request.DeviceMetadata.Fingerprint,
                request.DeviceMetadata.Browser,
                request.DeviceMetadata.OperatingSystem,
                request.DeviceMetadata.Timezone
            );

            // 5. Generate JWT Token trả về cho Client
            var token = _tokenGenerator.GenerateToken(user);

            var refreshTokenString = _tokenGenerator.GenerateRefreshToken();

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            var device = Device.Create(
                user.Id,
                deviceIdentify,
                request.DeviceMetadata.Fingerprint,
                request.DeviceMetadata.Browser,
                request.DeviceMetadata.OperatingSystem,
                userAgent,
                ip,
                request.DeviceMetadata.Timezone
            );

            var refreshToken = RefreshToken.Create(
                user.Id,
                device.Id,
                refreshTokenString
            );

            device.AddRefreshToken(refreshToken);

            _context.Users.Add(user);
            _context.Devices.Add(device);

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            var response = new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Email = user.Email,
                UserId = user.Id
            };

            return Result<AuthResponse>.Success(response, "Validate OTP Success");
        }
    }
}
