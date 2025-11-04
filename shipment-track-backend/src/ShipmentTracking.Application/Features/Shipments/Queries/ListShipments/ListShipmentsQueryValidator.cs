using FluentValidation;

namespace ShipmentTracking.Application.Features.Shipments.Queries.ListShipments;

/// <summary>
/// Validator for <see cref="ListShipmentsQuery"/>.
/// </summary>
public sealed class ListShipmentsQueryValidator : AbstractValidator<ListShipmentsQuery>
{
    public ListShipmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x)
            .Must(x => !x.FromDateUtc.HasValue || !x.ToDateUtc.HasValue || x.ToDateUtc >= x.FromDateUtc)
            .WithMessage("To date must be greater than or equal to from date.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(128);
    }
}
