using System.Threading;
using System.Threading.Tasks;
using AppValidationException = ShipmentTracking.Application.Common.Exceptions.ValidationException;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// Command for completing a password reset.
/// </summary>
public sealed record ResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest;

/// <summary>
/// Handles <see cref="ResetPasswordCommand"/>.
/// </summary>
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPasswordAsync(
            request.UserId,
            request.Token,
            request.NewPassword,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new AppValidationException(result.Errors);
        }

        return Unit.Value;
    }
}
