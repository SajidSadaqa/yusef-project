using System;

namespace ShipmentTracking.Application.Features.Shipments.Dto;

/// <summary>
/// Represents the payload required to update a shipment's details.
/// </summary>
public sealed class UpdateShipmentDto
{
    public Guid ShipmentId { get; init; }

    public string? CustomerReference { get; init; }

    public Guid? CustomerId { get; init; }

    public string? OriginPort { get; init; }

    public string? DestinationPort { get; init; }

    public decimal WeightKg { get; init; }

    public decimal VolumeCbm { get; init; }

    public DateTimeOffset? EstimatedDepartureUtc { get; init; }

    public DateTimeOffset? EstimatedArrivalUtc { get; init; }

    public string? CurrentLocation { get; init; }

    public string? Notes { get; init; }
}
