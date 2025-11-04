using System.Threading;
using System.Threading.Tasks;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Generates sequential tracking numbers following business rules.
/// </summary>
public interface ITrackingNumberGenerator
{
    /// <summary>
    /// Produces the next available tracking number.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated tracking number.</returns>
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
