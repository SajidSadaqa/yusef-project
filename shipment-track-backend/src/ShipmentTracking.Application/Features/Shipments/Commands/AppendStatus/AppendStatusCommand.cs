using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Shipments.Commands.AppendStatus;

/// <summary>
/// Command used to append a status update to a shipment.
/// </summary>
public sealed record AppendStatusCommand(AppendShipmentStatusDto Payload) : IRequest<ShipmentDto>;

/// <summary>
/// Handles <see cref="AppendStatusCommand"/>.
/// </summary>
public sealed class AppendStatusCommandHandler : IRequestHandler<AppendStatusCommand, ShipmentDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AppendStatusCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ShipmentDto> Handle(AppendStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new ForbiddenAccessException();
        }

        var payload = request.Payload;

        var shipment = await _dbContext.Shipments
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == payload.ShipmentId && !s.IsDeleted, cancellationToken);

        if (shipment is null)
        {
            throw new NotFoundException(nameof(Shipment), payload.ShipmentId);
        }

        var eventTime = payload.EventTimeUtc.ToUniversalTime();

        // Create the new status history entity
        var statusHistory = ShipmentStatusHistory.Create(
            shipment.Id,
            payload.Status,
            payload.Description,
            payload.Location,
            eventTime,
            _currentUserService.UserId);

        // Add directly to DbSet to ensure proper entity state tracking
        _dbContext.ShipmentStatusHistories.Add(statusHistory);

        // Update shipment status
        var trackedShipment = await _dbContext.Shipments
            .FirstOrDefaultAsync(s => s.Id == payload.ShipmentId, cancellationToken);

        if (trackedShipment is not null)
        {
            trackedShipment.UpdateCurrentStatus(payload.Status);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Reload with status history for mapping to DTO
        var result = await _dbContext.Shipments
            .Include(s => s.StatusHistory)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == payload.ShipmentId, cancellationToken);

        return _mapper.Map<ShipmentDto>(result!);
    }
}
