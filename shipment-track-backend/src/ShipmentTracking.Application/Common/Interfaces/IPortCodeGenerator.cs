using System.Threading;
using System.Threading.Tasks;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Service for generating unique port codes based on port name and country.
/// </summary>
public interface IPortCodeGenerator
{
    /// <summary>
    /// Generates a unique port code based on the port name and country.
    /// </summary>
    /// <param name="portName">The name of the port.</param>
    /// <param name="country">The country where the port is located.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A unique 5-character port code following UN/LOCODE standard.</returns>
    Task<string> GenerateAsync(string portName, string country, CancellationToken cancellationToken = default);
}
