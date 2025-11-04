using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Validator for <see cref="RefreshTokenCommand"/>.
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();

        RuleFor(x => x.IpAddress)
            .MaximumLength(64);
    }
}
