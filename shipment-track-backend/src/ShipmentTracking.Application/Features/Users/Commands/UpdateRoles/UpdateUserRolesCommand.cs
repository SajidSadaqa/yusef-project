using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Users.Commands.UpdateRoles;

/// <summary>
/// Command to overwrite the roles assigned to a user.
/// </summary>
public sealed record UpdateUserRolesCommand(string UserId, IReadOnlyCollection<string> Roles) : IRequest;

/// <summary>
/// Handles <see cref="UpdateUserRolesCommand"/>.
/// </summary>
public sealed class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand>
{
    private readonly IIdentityService _identityService;

    public UpdateUserRolesCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateUserRolesAsync(request.UserId, request.Roles, cancellationToken);

        if (!result.Succeeded)
        {
            throw new ShipmentTracking.Application.Common.Exceptions.ValidationException(result.Errors);
        }

        return Unit.Value;
    }
}
