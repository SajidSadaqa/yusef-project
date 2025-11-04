using System;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Application.Features.Shipments.Dto;

/// <summary>
/// Represents a status entry in the shipment timeline.
/// </summary>
public sealed class ShipmentStatusHistoryDto
{
    public Guid Id { get; init; }

    public ShipmentStatus Status { get; init; }

    public string? Description { get; init; }

    public string? Location { get; init; }

    public DateTimeOffset EventTimeUtc { get; init; }
}
