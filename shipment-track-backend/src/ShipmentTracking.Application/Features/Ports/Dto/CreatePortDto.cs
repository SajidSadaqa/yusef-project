namespace ShipmentTracking.Application.Features.Ports.Dto;

/// <summary>
/// Data transfer object for creating a new port.
/// </summary>
public sealed class CreatePortDto
{
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? City { get; init; }
    public string? Code { get; init; } // Optional: admin can override auto-generated code
}
