using System;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Shipments.Commands.AppendStatus;

/// <summary>
/// Validator for <see cref="AppendStatusCommand"/>.
/// </summary>
public sealed class AppendStatusCommandValidator : AbstractValidator<AppendStatusCommand>
{
    public AppendStatusCommandValidator()
    {
        RuleFor(x => x.Payload.ShipmentId)
            .NotEmpty();

        RuleFor(x => x.Payload.EventTimeUtc)
            .NotEqual(default(DateTimeOffset));

        RuleFor(x => x.Payload.Description)
            .MaximumLength(512);

        RuleFor(x => x.Payload.Location)
            .MaximumLength(256);
    }
}
