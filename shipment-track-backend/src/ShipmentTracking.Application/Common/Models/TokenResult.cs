using System;
using System.Collections.Generic;

namespace ShipmentTracking.Application.Common.Models;

/// <summary>
/// Represents the outcome of generating tokens for a user.
/// </summary>
public sealed class TokenResult
{
    /// <summary>
    /// Gets the access token value.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets the refresh token value.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Gets the access token expiration timestamp.
    /// </summary>
    public required DateTimeOffset AccessTokenExpiresAtUtc { get; init; }

    /// <summary>
    /// Gets additional claims applied.
    /// </summary>
    public IDictionary<string, object> Claims { get; init; } = new Dictionary<string, object>();
}
