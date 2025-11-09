using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user.
/// </summary>
public sealed record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role) : IRequest<string>;

/// <summary>
/// Handles <see cref="CreateUserCommand"/>.
/// </summary>
public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            null, // phoneNumber
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new Common.Exceptions.ValidationException(result.Errors);
        }

        // Assign role to the newly created user
        var userId = result.Value!;
        var roleResult = await _identityService.AddUserToRoleAsync(userId, request.Role, cancellationToken);

        if (!roleResult.Succeeded)
        {
            throw new Common.Exceptions.ValidationException(roleResult.Errors);
        }

        return userId;
    }
}
