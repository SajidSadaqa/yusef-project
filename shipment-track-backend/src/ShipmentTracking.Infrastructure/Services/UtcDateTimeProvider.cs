using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Provides the current UTC time.
/// </summary>
public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
