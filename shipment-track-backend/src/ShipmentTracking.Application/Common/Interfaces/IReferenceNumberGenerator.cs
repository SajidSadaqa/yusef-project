using System.Threading;
using System.Threading.Tasks;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Generates unique shipment reference numbers that can be shared externally.
/// </summary>
public interface IReferenceNumberGenerator
{
    /// <summary>
    /// Produces a unique reference number value.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated reference number.</returns>
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
