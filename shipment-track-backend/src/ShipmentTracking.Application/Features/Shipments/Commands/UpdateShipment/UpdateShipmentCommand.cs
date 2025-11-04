using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.Enums;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Application.Features.Shipments.Commands.UpdateShipment;

/// <summary>
/// Command used to update shipment metadata.
/// </summary>
public sealed record UpdateShipmentCommand(UpdateShipmentDto Payload) : IRequest<ShipmentDto>;

/// <summary>
/// Handles <see cref="UpdateShipmentCommand"/>.
/// </summary>
public sealed class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, ShipmentDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateShipmentCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<ShipmentDto> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var shipment = await _dbContext.Shipments
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.Id == payload.ShipmentId && !s.IsDeleted, cancellationToken);

        if (shipment is null)
        {
            throw new NotFoundException(nameof(Shipment), payload.ShipmentId);
        }

        shipment.UpdateDetails(
            payload.CustomerReference,
            payload.CustomerId,
            Port.Create(payload.OriginPort!),
            Port.Create(payload.DestinationPort!),
            Weight.FromDecimal(payload.WeightKg),
            Volume.FromDecimal(payload.VolumeCbm),
            payload.EstimatedDepartureUtc,
            payload.EstimatedArrivalUtc,
            payload.CurrentLocation,
            payload.Notes);

        shipment.SetUpdatedAudit(_currentUserService.UserId, _dateTimeProvider.UtcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ShipmentDto>(shipment);
    }
}
