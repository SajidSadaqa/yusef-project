using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Shipments.Commands.CreateShipment;

/// <summary>
/// FluentValidation rules for <see cref="CreateShipmentCommand"/>.
/// </summary>
public sealed class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    private readonly IPortCatalogService _portCatalogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateShipmentCommandValidator"/> class.
    /// </summary>
    public CreateShipmentCommandValidator(IPortCatalogService portCatalogService)
    {
        _portCatalogService = portCatalogService;

        RuleFor(x => x.Payload.CustomerReference)
            .MaximumLength(128);

        RuleFor(x => x.Payload.WeightKg)
            .GreaterThan(0);

        RuleFor(x => x.Payload.VolumeCbm)
            .GreaterThan(0);

        RuleFor(x => x.Payload.OriginPort)
            .NotEmpty()
            .MaximumLength(5)
            .MustAsync(PortExistsAsync)
            .WithMessage("Origin port must exist in the port catalog.");

        RuleFor(x => x.Payload.DestinationPort)
            .NotEmpty()
            .MaximumLength(5)
            .MustAsync(PortExistsAsync)
            .WithMessage("Destination port must exist in the port catalog.")
            .NotEqual(x => x.Payload.OriginPort)
            .WithMessage("Destination port must differ from origin port.");

        RuleFor(x => x.Payload)
            .Must(payload => !payload.EstimatedDepartureUtc.HasValue || !payload.EstimatedArrivalUtc.HasValue ||
                             payload.EstimatedArrivalUtc >= payload.EstimatedDepartureUtc)
            .WithMessage("Estimated arrival must be after estimated departure.");
    }

    private async Task<bool> PortExistsAsync(string? portCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(portCode))
        {
            return false;
        }

        return await _portCatalogService.PortExistsAsync(portCode, cancellationToken);
    }
}
