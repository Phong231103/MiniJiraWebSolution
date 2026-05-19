using MediatR;
using Web.Application.Common.Interfaces;
using Web.Domain.Primitives;

namespace Web.Application.Auth.Commands
{
    public class RegisterUserCommand : IRequest<Result<bool>>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public RegisterUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<bool>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exsitEmail = _context.Users.Any(u => u.Email == request.Email);

            if (exsitEmail == true)
            {
                return Error.Exist("Exist", "Email đã tồn tại");
            }


        }
    }

}
