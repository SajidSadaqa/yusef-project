using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Validator for <see cref="ForgotPasswordCommand"/>.
/// </summary>
public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.ResetCallbackUrl)
            .MaximumLength(512);
    }
}
