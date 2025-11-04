using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Auth.Commands.VerifyEmail;

/// <summary>
/// Command to confirm a user's email address.
/// </summary>
public sealed record VerifyEmailCommand(string UserId, string Token) : IRequest;

/// <summary>
/// Handles <see cref="VerifyEmailCommand"/>.
/// </summary>
public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly IIdentityService _identityService;

    public VerifyEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);

        if (!result.Succeeded)
        {
            throw new ShipmentTracking.Application.Common.Exceptions.ValidationException(result.Errors);
        }

        return Unit.Value;
    }
}
