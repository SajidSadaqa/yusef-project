using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the persistence context used by the application layer.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Shipment> Shipments { get; }

    DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<AuditLog> AuditLogs { get; }

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of changed entities.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
