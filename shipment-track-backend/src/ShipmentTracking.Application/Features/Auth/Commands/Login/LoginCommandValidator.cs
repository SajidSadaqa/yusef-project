using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validator for <see cref="LoginCommand"/>.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.IpAddress)
            .MaximumLength(64);
    }
}
