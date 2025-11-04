using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Models;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Generates and validates JWT access and refresh tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a new access token and refresh token pair.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="email">User email.</param>
    /// <param name="roles">User roles.</param>
    /// <param name="additionalClaims">Additional claims to embed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated tokens.</returns>
    Task<TokenResult> GenerateTokenPairAsync(
        string userId,
        string email,
        IReadOnlyCollection<string> roles,
        IDictionary<string, object>? additionalClaims,
        CancellationToken cancellationToken);

    /// <summary>
    /// Validates a refresh token and generates a new pair.
    /// </summary>
    /// <param name="refreshToken">Existing refresh token.</param>
    /// <param name="ipAddress">IP address performing the refresh.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed token pair.</returns>
    Task<TokenResult> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets principal from expired access token for rotation scenarios.
    /// </summary>
    /// <param name="token">Access token value.</param>
    /// <returns>User principal extracted from the token.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
