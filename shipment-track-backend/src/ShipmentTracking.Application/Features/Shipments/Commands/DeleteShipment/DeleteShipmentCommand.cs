using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Shipments.Commands.DeleteShipment;

/// <summary>
/// Command to soft delete a shipment.
/// </summary>
public sealed record DeleteShipmentCommand(Guid ShipmentId) : IRequest;

/// <summary>
/// Handles <see cref="DeleteShipmentCommand"/>.
/// </summary>
public sealed class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteShipmentCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new ForbiddenAccessException();
        }

        var shipment = await _dbContext.Shipments
            .FirstOrDefaultAsync(s => s.Id == request.ShipmentId && !s.IsDeleted, cancellationToken);

        if (shipment is null)
        {
            throw new NotFoundException(nameof(Shipment), request.ShipmentId);
        }

        shipment.SoftDelete();
        shipment.SetUpdatedAudit(_currentUserService.UserId, _dateTimeProvider.UtcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
