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
        await using var command = _dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT CONCAT('VTX-', TO_CHAR(CURRENT_DATE, 'YYYYMM'), '-', LPAD(nextval('tracking_number_seq')::text, 4, '0'))";

        await _dbContext.Database.OpenConnectionAsync(cancellationToken);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        var trackingNumber = result?.ToString() ?? throw new InvalidOperationException("Failed to generate tracking number");

        // Validate with domain rules before returning.
        return TrackingNumber.Create(trackingNumber).Value;
    }
}
