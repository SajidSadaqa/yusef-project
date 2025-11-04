namespace ShipmentTracking.WebApi.Contracts.Users;

public sealed record UpdateUserRolesRequest(IReadOnlyCollection<string> Roles);
