using MediatR;
using Web.Application.Auth.DTOs;
using Web.Application.Common.Interfaces;
using Web.Domain.Primitives;
using Web.Domain.Repository;

namespace Web.Application.Auth.Commands
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var userInfor = _context.Users.Where(u => u.Email == request.Email);

            if (!userInfor.Any())
            {
                return Error.NotFound("404", "User not found");
            }

            var checkPass = userInfor.FirstOrDefault()?.ValidatePassword(request.Password, _passwordHasher);

            if (checkPass == false)
            {
                return Error.Unauthorized("401", "Invalid password");
            }


        }

    }
}
