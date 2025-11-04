using System;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Application.Features.Shipments.Dto;

/// <summary>
/// Represents the payload required to append a status update to a shipment.
/// </summary>
public sealed class AppendShipmentStatusDto
{
    public Guid ShipmentId { get; init; }

    public ShipmentStatus Status { get; init; }

    public string? Description { get; init; }

    public string? Location { get; init; }

    public DateTimeOffset EventTimeUtc { get; init; }
}
