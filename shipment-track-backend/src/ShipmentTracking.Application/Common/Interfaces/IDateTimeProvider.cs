using System;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Provides a consistent time source for the application.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC timestamp.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
