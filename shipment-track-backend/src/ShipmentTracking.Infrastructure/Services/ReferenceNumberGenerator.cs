using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Generates unique shipment reference numbers using ULIDs with an `SHP-` prefix.
/// </summary>
public sealed class ReferenceNumberGenerator : IReferenceNumberGenerator
{
    private const int MaxAttempts = 10;
    private readonly IApplicationDbContext _dbContext;

    public ReferenceNumberGenerator(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var candidate = $"SHP-{Ulid.NewUlid()}";

            var exists = await _dbContext.Shipments
                .AsNoTracking()
                .AnyAsync(shipment => shipment.ReferenceNumber == candidate, cancellationToken);

            if (!exists)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Failed to generate a unique shipment reference number after multiple attempts.");
    }
}
