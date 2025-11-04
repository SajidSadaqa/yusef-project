namespace ShipmentTracking.Domain.Enums;

/// <summary>
/// Represents the lifecycle states for a shipment.
/// </summary>
public enum ShipmentStatus
{
    Received = 0,
    Packed = 1,
    AtOriginPort = 2,
    OnVessel = 3,
    ArrivedToPort = 4,
    CustomsCleared = 5,
    OutForDelivery = 6,
    Delivered = 7,
    Returned = 8,
    Cancelled = 9
}
