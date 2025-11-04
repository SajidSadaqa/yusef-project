using System;

namespace ShipmentTracking.Application.Features.Ports.Dto;

/// <summary>
/// Data transfer object for port information.
/// </summary>
public sealed class PortDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? City { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
