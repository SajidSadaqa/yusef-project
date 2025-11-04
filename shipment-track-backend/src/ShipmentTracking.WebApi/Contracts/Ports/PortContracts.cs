namespace ShipmentTracking.WebApi.Contracts.Ports;

public sealed record CreatePortRequest(
    string Name,
    string Country,
    string? City,
    string? Code); // Optional: admin can override auto-generated code

public sealed record UpdatePortRequest(
    string Name,
    string Country,
    string? City,
    bool IsActive);
