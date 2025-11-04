using System;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentHistory;

/// <summary>
/// Validator for <see cref="GetShipmentHistoryQuery"/>.
/// </summary>
public sealed class GetShipmentHistoryQueryValidator : AbstractValidator<GetShipmentHistoryQuery>
{
    public GetShipmentHistoryQueryValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEqual(Guid.Empty);
    }
}
