using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.WebApi.Contracts.Shipments;

public sealed record CreateShipmentRequest(
    string? CustomerReference,
    Guid? CustomerId,
    string OriginPort,
    string DestinationPort,
    decimal WeightKg,
    decimal VolumeCbm,
    DateTimeOffset? EstimatedDepartureUtc,
    DateTimeOffset? EstimatedArrivalUtc,
    string? CurrentLocation,
    string? Notes);

public sealed record UpdateShipmentRequest(
    Guid ShipmentId,
    string? CustomerReference,
    Guid? CustomerId,
    string OriginPort,
    string DestinationPort,
    decimal WeightKg,
    decimal VolumeCbm,
    DateTimeOffset? EstimatedDepartureUtc,
    DateTimeOffset? EstimatedArrivalUtc,
    string? CurrentLocation,
    string? Notes);

public sealed record AppendShipmentStatusRequest(
    ShipmentStatus Status,
    string? Description,
    string? Location,
    DateTimeOffset EventTimeUtc);
