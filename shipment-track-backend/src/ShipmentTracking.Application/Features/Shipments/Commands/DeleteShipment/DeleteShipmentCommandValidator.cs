using System;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Shipments.Commands.DeleteShipment;

/// <summary>
/// Validator for <see cref="DeleteShipmentCommand"/>.
/// </summary>
public sealed class DeleteShipmentCommandValidator : AbstractValidator<DeleteShipmentCommand>
{
    public DeleteShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEqual(Guid.Empty);
    }
}
