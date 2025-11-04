using System.Text.RegularExpressions;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.Register;

/// <summary>
/// Validator for <see cref="RegisterCommand"/>.
/// </summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(new Regex(PasswordPattern))
            .WithMessage("Password must be at least 8 characters and include upper, lower, digit and special characters.");

        RuleFor(x => x.FirstName)
            .MaximumLength(64);

        RuleFor(x => x.LastName)
            .MaximumLength(64);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(32);

        RuleFor(x => x.VerificationCallbackUrl)
            .MaximumLength(512);

        RuleFor(x => x.DashboardUrl)
            .MaximumLength(512);
    }
}
