namespace ShipmentTracking.WebApi.Contracts.Users;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role);

public sealed record CreateUserResponse
{
    public string UserId { get; init; } = string.Empty;
}

public sealed record UpdateUserRequest(
    string? Email,
    string? FirstName,
    string? LastName,
    string? Role,
    string? Status);

public sealed record UpdateUserRolesRequest(IReadOnlyCollection<string> Roles);
