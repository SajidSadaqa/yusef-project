using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentHistory;

/// <summary>
/// Query returning the status history for a shipment.
/// </summary>
public sealed record GetShipmentHistoryQuery(Guid ShipmentId) : IRequest<IReadOnlyCollection<ShipmentStatusHistoryDto>>;

/// <summary>
/// Handles <see cref="GetShipmentHistoryQuery"/>.
/// </summary>
public sealed class GetShipmentHistoryQueryHandler
    : IRequestHandler<GetShipmentHistoryQuery, IReadOnlyCollection<ShipmentStatusHistoryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetShipmentHistoryQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<ShipmentStatusHistoryDto>> Handle(
        GetShipmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _dbContext.Shipments
            .AsNoTracking()
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.Id == request.ShipmentId && !s.IsDeleted, cancellationToken);

        if (shipment is null)
        {
            throw new NotFoundException(nameof(Shipment), request.ShipmentId);
        }

        var orderedHistory = shipment.StatusHistory
            .OrderBy(entry => entry.EventTimeUtc)
            .ToList();

        return _mapper.Map<IReadOnlyCollection<ShipmentStatusHistoryDto>>(orderedHistory);
    }
}
