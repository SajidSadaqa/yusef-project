using System;
using System.Collections.Generic;

namespace ShipmentTracking.Application.Features.Auth.Dto;

/// <summary>
/// Represents the JWT access and refresh token pair returned to clients.
/// </summary>
public sealed class TokenResponseDto
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }

    public IDictionary<string, object> Claims { get; init; } = new Dictionary<string, object>();
}
