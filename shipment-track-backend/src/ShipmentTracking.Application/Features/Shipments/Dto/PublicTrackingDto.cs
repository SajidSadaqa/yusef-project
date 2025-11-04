using System;
using System.Collections.Generic;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Application.Features.Shipments.Dto;

/// <summary>
/// Represents shipment details exposed via the public tracking endpoint.
/// </summary>
public sealed class PublicTrackingDto
{
    public string TrackingNumber { get; init; } = string.Empty;

    public ShipmentStatus CurrentStatus { get; init; }

    public string? CurrentLocation { get; init; }

    public DateTimeOffset? EstimatedArrivalUtc { get; init; }

    public IReadOnlyCollection<ShipmentStatusHistoryDto> StatusHistory { get; init; } = Array.Empty<ShipmentStatusHistoryDto>();
}
