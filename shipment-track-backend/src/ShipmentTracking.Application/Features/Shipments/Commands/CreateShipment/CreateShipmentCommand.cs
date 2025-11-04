using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Application.Features.Shipments.Commands.CreateShipment;

/// <summary>
/// Command used to create a shipment record.
/// </summary>
public sealed record CreateShipmentCommand(CreateShipmentDto Payload) : IRequest<ShipmentDto>;

/// <summary>
/// Handles <see cref="CreateShipmentCommand"/>.
/// </summary>
public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITrackingNumberGenerator _trackingNumberGenerator;
    private readonly IReferenceNumberGenerator _referenceNumberGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateShipmentCommandHandler"/> class.
    /// </summary>
    public CreateShipmentCommandHandler(
        IApplicationDbContext dbContext,
        ITrackingNumberGenerator trackingNumberGenerator,
        IReferenceNumberGenerator referenceNumberGenerator,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _trackingNumberGenerator = trackingNumberGenerator;
        _referenceNumberGenerator = referenceNumberGenerator;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<ShipmentDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        // Generate tracking number
        var trackingNumberValue = await _trackingNumberGenerator.GenerateAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(trackingNumberValue))
        {
            throw new InvalidOperationException("Failed to generate tracking number.");
        }

        // Generate reference number
        var referenceNumber = await _referenceNumberGenerator.GenerateAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new InvalidOperationException("Failed to generate reference number.");
        }

        var trackingNumber = TrackingNumber.Create(trackingNumberValue);
        var originPort = Port.Create(payload.OriginPort!);
        var destinationPort = Port.Create(payload.DestinationPort!);
        var weight = Weight.FromDecimal(payload.WeightKg);
        var volume = Volume.FromDecimal(payload.VolumeCbm);

        var now = _dateTimeProvider.UtcNow;

        var shipment = Shipment.Create(
            trackingNumber,
            referenceNumber,
            payload.CustomerReference,
            originPort,
            destinationPort,
            weight,
            volume,
            payload.CustomerId,
            payload.EstimatedDepartureUtc,
            payload.EstimatedArrivalUtc,
            payload.CurrentLocation,
            payload.Notes,
            now);

        shipment.SetCreatedAudit(_currentUserService.UserId, now);
        shipment.SetUpdatedAudit(_currentUserService.UserId, now);

        _dbContext.Shipments.Add(shipment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ShipmentDto>(shipment);
    }
}
