using System;
using System.Collections.Generic;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Application.Features.Shipments.Dto;

/// <summary>
/// Represents the internal view of a shipment for staff and administrators.
/// </summary>
public sealed class ShipmentDto
{
    public Guid Id { get; init; }

    public string TrackingNumber { get; init; } = string.Empty;

    public string ReferenceNumber { get; init; } = string.Empty;

    public string? CustomerReference { get; init; }

    public Guid? CustomerId { get; init; }

    public string OriginPort { get; init; } = string.Empty;

    public string DestinationPort { get; init; } = string.Empty;

    public decimal WeightKg { get; init; }

    public decimal VolumeCbm { get; init; }

    public ShipmentStatus CurrentStatus { get; init; }

    public DateTimeOffset? EstimatedDepartureUtc { get; init; }

    public DateTimeOffset? EstimatedArrivalUtc { get; init; }

    public string? CurrentLocation { get; init; }

    public string? Notes { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public IReadOnlyCollection<ShipmentStatusHistoryDto> StatusHistory { get; init; } = Array.Empty<ShipmentStatusHistoryDto>();
}
