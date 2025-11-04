using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.ValueObjects;
using ShipmentTracking.Infrastructure.Persistence;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Generates sequential shipment tracking numbers using the database sequence.
/// </summary>
public sealed class TrackingNumberGenerator : ITrackingNumberGenerator
{
    private readonly ApplicationDbContext _dbContext;

    public TrackingNumberGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var trackingNumber = await _dbContext.Database
            .SqlQueryRaw<string>("SELECT CONCAT('VTX-', TO_CHAR(CURRENT_DATE, 'YYYYMM'), '-', LPAD(nextval('tracking_number_seq')::text, 4, '0'));")
            .FirstAsync(cancellationToken);

        // Validate with domain rules before returning.
        return TrackingNumber.Create(trackingNumber).Value;
    }
}
