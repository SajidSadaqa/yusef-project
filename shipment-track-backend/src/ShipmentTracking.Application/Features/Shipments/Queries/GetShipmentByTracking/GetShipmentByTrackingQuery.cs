using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentByTracking;

/// <summary>
/// Query used to retrieve public shipment details by tracking number.
/// </summary>
public sealed record GetShipmentByTrackingQuery(string TrackingNumber) : IRequest<PublicTrackingDto>;

/// <summary>
/// Handles <see cref="GetShipmentByTrackingQuery"/>.
/// </summary>
public sealed class GetShipmentByTrackingQueryHandler : IRequestHandler<GetShipmentByTrackingQuery, PublicTrackingDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetShipmentByTrackingQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PublicTrackingDto> Handle(GetShipmentByTrackingQuery request, CancellationToken cancellationToken)
    {
        var trackingNumber = TrackingNumber.Create(request.TrackingNumber);

        var shipment = await _dbContext.Shipments
            .AsNoTracking()
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(
                s => !s.IsDeleted && s.TrackingNumber.Equals(trackingNumber),
                cancellationToken);

        if (shipment is null)
        {
            throw new NotFoundException(nameof(Shipment), request.TrackingNumber);
        }

        return _mapper.Map<PublicTrackingDto>(shipment);
    }
}
