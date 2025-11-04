using FluentValidation;

namespace ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentByTracking;

/// <summary>
/// Validator for <see cref="GetShipmentByTrackingQuery"/>.
/// </summary>
public sealed class GetShipmentByTrackingQueryValidator : AbstractValidator<GetShipmentByTrackingQuery>
{
    public GetShipmentByTrackingQueryValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .MaximumLength(16);
    }
}
