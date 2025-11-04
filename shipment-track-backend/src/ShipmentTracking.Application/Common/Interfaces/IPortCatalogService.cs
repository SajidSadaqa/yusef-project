using System.Threading;
using System.Threading.Tasks;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Provides access to reference data for ports.
/// </summary>
public interface IPortCatalogService
{
    /// <summary>
    /// Determines whether the provided port code exists in the catalog.
    /// </summary>
    /// <param name="portCode">Port code to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> PortExistsAsync(string portCode, CancellationToken cancellationToken = default);
}
