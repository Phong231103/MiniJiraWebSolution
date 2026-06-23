//using MediatR;
//using Web.Application.Auth.DTOs;
//using Web.Application.Common.Interfaces;
//using Web.Domain.Primitives;
//using Web.Domain.Repository;

//namespace Web.Application.Auth.Commands
//{
//    public class LoginCommand : IRequest<Result<AuthResponse>>
//    {
//        public string Email { get; init; } = string.Empty;
//        public string Password { get; init; } = string.Empty;
//        public LoginCommand(string email, string password)
//        {
//            Email = email;
//            Password = password;
//        }
//    }

//    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
//    {
//        private readonly IApplicationDbContext _context;
//        private readonly IPasswordHasher _passwordHasher;
//        private readonly IJwtTokenGenerator _token;

//        public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator token)
//        {
//            _context = context;
//            _passwordHasher = passwordHasher;
//            _token = token;
//        }

//        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
//        {
//            var user = _context.Users.Where(u => u.Email == request.Email);

//            if (!user.Any())
//            {
//                return Error.NotFound("404", "User not found");
//            }

//            if (!user.First().IsActive)
//            {
//                return Error.Unauthorized("401", "User is not allowed to login");
//            }

//            if (!user.First().IsLockedOut)
//            {
//                return Error.Unauthorized("401", "User is locked out. Please try again later.");
//            }

//            var checkPass = user.FirstOrDefault()?.ValidatePassword(request.Password, _passwordHasher);

//            if (checkPass == false)
//            {
//                user.First().RecordFailedLogin();

//                if (user.First().FailedLoginAttempts % 5 == 0)
//                {
//                    return Error.NotFound("404", "User is locked out due to multiple failed login attempts. Please try again later.");
//                }

//                if (user.First().FailedLoginAttempts == 15)
//                {
//                    return Error.NotFound("404", "User account is deactive. Contact with us for help");
//                }

//                return Error.Unauthorized("401", "Invalid password");
//            }

//            user.First().ResetLoginAttempts();

//            var token = _token.GenerateToken(user.First(), false);

//            var refreshToken = _token.GenerateRefreshToken();

//            var response = new AuthResponse
//            {
//                AccessToken = token,
//                RefreshToken = refreshToken,
//                Email = user.First().Email,
//                UserId = user.First().Id
//            };

//            return Result<AuthResponse>.Success(response, "Finish update personal information. Welcome");
//        }

//    }
//}
