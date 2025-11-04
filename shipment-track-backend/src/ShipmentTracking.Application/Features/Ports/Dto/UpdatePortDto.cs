namespace ShipmentTracking.Application.Features.Ports.Dto;

public sealed record UpdatePortDto
{
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? City { get; init; }
    public bool IsActive { get; init; }
}
