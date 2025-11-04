using System.Text.RegularExpressions;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// Validator for <see cref="ResetPasswordCommand"/>.
/// </summary>
public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(new Regex(PasswordPattern))
            .WithMessage("Password must be at least 8 characters and include upper, lower, digit and special characters.");
    }
}
