using MediatR;

namespace Web.Application.Projects.Commands
{
    public record CreateProjectCommand(string Name, string? Description, Guid OwnerId) : IRequest<Guid>;
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        public Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
