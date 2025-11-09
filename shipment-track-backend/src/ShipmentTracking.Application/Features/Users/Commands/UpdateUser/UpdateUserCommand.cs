using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Command to update a user's information.
/// </summary>
public sealed record UpdateUserCommand(
    string UserId,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Role,
    string? Status) : IRequest;

/// <summary>
/// Handles <see cref="UpdateUserCommand"/>.
/// </summary>
public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateUserAsync(
            request.UserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Role,
            request.Status,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new Common.Exceptions.ValidationException(result.Errors);
        }

        return Unit.Value;
    }
}
