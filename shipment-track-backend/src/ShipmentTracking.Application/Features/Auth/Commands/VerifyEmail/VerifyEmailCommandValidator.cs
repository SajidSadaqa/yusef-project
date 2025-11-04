using System;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Auth.Commands.VerifyEmail;

/// <summary>
/// Validator for <see cref="VerifyEmailCommand"/>.
/// </summary>
public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
