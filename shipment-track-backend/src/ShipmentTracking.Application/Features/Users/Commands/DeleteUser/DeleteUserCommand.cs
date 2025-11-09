using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Command to delete a user.
/// </summary>
public sealed record DeleteUserCommand(string UserId) : IRequest;

/// <summary>
/// Handles <see cref="DeleteUserCommand"/>.
/// </summary>
public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IIdentityService _identityService;

    public DeleteUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteUserAsync(request.UserId, cancellationToken);

        if (!result.Succeeded)
        {
            throw new Common.Exceptions.ValidationException(result.Errors);
        }

        return Unit.Value;
    }
}
