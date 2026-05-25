using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Web.Application.Auth.DTOs;
using Web.Application.Common.Interfaces;
using Web.Domain.Primitives;

namespace Web.Application.Auth.Commands
{
    public record UserInformationAfterRegisCommand(string fullName, string phoneNumber, string avaURL) : IRequest<Result<AuthResponse>>;

    public class UserInformationAfterRegisCommandHandler : IRequestHandler<UserInformationAfterRegisCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IJwtTokenGenerator _token;

        public UserInformationAfterRegisCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContext, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _httpContext = httpContext;
            _token = jwtTokenGenerator;
        }

        public async Task<Result<AuthResponse>> Handle(UserInformationAfterRegisCommand request, CancellationToken cancellationToken)
        {
            var email = _httpContext.HttpContext?.User.FindFirst(ClaimTypes.Email)?.ToString();

            if (email == null)
            {
                return Error.Unauthorized("Unauthorized", "User is not authenticated.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return Error.NotFound("User Not Found", "User is Not Found.");
            }

            user.UpdateProfile(request.fullName, request.phoneNumber, request.avaURL);

            _context.Users.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var token = _token.GenerateToken(user, false);

            var refreshToken = _token.GenerateRefreshToken();

            var response = new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Email = user.Email,
                UserId = user.Id
            };

            return Result<AuthResponse>.Success(response, "Finish update personal information. Welcome");
        }
    }
}
