using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Infrastructure.Identity;
using ShipmentTracking.Infrastructure.Options;
using ShipmentTracking.Infrastructure.Persistence;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Generates and manages JWT access and refresh tokens.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        ApplicationDbContext dbContext,
        IOptions<JwtOptions> jwtOptions,
        IDateTimeProvider dateTimeProvider,
        UserManager<ApplicationUser> userManager,
        ILogger<TokenService> logger)
    {
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<TokenResult> GenerateTokenPairAsync(
        string userId,
        string email,
        IReadOnlyCollection<string> roles,
        IDictionary<string, object>? additionalClaims,
        CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var (claims, claimsDictionary) = BuildClaims(userId, email, roles, additionalClaims);
        var accessToken = CreateJwtToken(claims, now);

        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new SecurityTokenException("User identifier is not a valid GUID.");
        }

        await RemoveStaleRefreshTokensAsync(userGuid, cancellationToken);

        var (refreshToken, refreshTokenValue) = CreateRefreshToken(userGuid, null, userGuid, now);
        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return BuildTokenResult(accessToken, now.AddMinutes(_jwtOptions.AccessTokenMinutes), refreshTokenValue, claimsDictionary);
    }

    public async Task<TokenResult> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var hashedToken = HashToken(refreshToken);
        var existingToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(token => token.TokenHash == hashedToken, cancellationToken);

        if (existingToken is null)
        {
            _logger.LogWarning("Attempt to use an unknown refresh token.");
            throw new SecurityTokenException("Invalid refresh token.");
        }

        if (!existingToken.IsActive)
        {
            _logger.LogWarning("Attempt to use inactive refresh token {TokenId}.", existingToken.Id);
            throw new SecurityTokenException("Refresh token is no longer active.");
        }

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == existingToken.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Refresh token {TokenId} references missing user {UserId}.", existingToken.Id, existingToken.UserId);
            throw new SecurityTokenException("User associated with refresh token does not exist.");
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        var additionalClaims = new Dictionary<string, object?>
        {
            ["firstName"] = user.FirstName ?? string.Empty,
            ["lastName"] = user.LastName ?? string.Empty,
            ["emailConfirmed"] = user.EmailConfirmed,
        };

        var now = _dateTimeProvider.UtcNow;
        var (claims, claimsDictionary) = BuildClaims(user.Id.ToString(), user.Email!, roles, additionalClaims);
        var accessToken = CreateJwtToken(claims, now);

        existingToken.Revoke("Rotated", now, ipAddress, existingToken.UserId);
        var (newRefreshToken, refreshValue) = CreateRefreshToken(existingToken.UserId, ipAddress, existingToken.UserId, now);
        existingToken.SetReplacementToken(newRefreshToken.TokenHash);

        await RemoveStaleRefreshTokensAsync(existingToken.UserId, cancellationToken);
        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return BuildTokenResult(accessToken, now.AddMinutes(_jwtOptions.AccessTokenMinutes), refreshValue, claimsDictionary);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

        var parameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        return tokenHandler.ValidateToken(token, parameters, out _);
    }

    private (List<Claim> Claims, Dictionary<string, object> ClaimsDictionary) BuildClaims(
        string userId,
        string email,
        IReadOnlyCollection<string> roles,
        IDictionary<string, object?>? additionalClaims)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("uid", userId)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["uid"] = userId,
            ["email"] = email,
            ["roles"] = roles.ToArray(),
        };

        if (additionalClaims is not null)
        {
            foreach (var kvp in additionalClaims)
            {
                if (kvp.Value is not null)
                {
                    var value = kvp.Value;
                    if (value is bool boolValue)
                    {
                        dictionary[kvp.Key] = boolValue;
                    }
                    else
                    {
                        dictionary[kvp.Key] = value;
                    }

                    claims.Add(new Claim(kvp.Key, value!.ToString()!));
                }
            }
        }

        return (claims, dictionary);
    }

    private string CreateJwtToken(IEnumerable<Claim> claims, DateTimeOffset now)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(_jwtOptions.AccessTokenMinutes).UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private (RefreshToken Token, string Value) CreateRefreshToken(
        Guid userId,
        string? ipAddress,
        Guid? createdByUserId,
        DateTimeOffset issuedAtUtc)
    {
        var buffer = RandomNumberGenerator.GetBytes(64);
        var tokenValue = Convert.ToBase64String(buffer);
        var tokenHash = HashToken(tokenValue);

        var refreshToken = RefreshToken.Create(
            userId,
            tokenHash,
            issuedAtUtc.AddDays(_jwtOptions.RefreshTokenDays),
            ipAddress,
            createdByUserId);

        return (refreshToken, tokenValue);
    }

    private async Task RemoveStaleRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        var staleTokens = await _dbContext.RefreshTokens
            .Where(token => token.UserId == userId &&
                            (token.RevokedAtUtc.HasValue || token.ExpiresAtUtc <= now))
            .ToListAsync(cancellationToken);

        if (staleTokens.Count > 0)
        {
            _dbContext.RefreshTokens.RemoveRange(staleTokens);
        }
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private static TokenResult BuildTokenResult(
        string accessToken,
        DateTimeOffset expiresAt,
        string refreshToken,
        Dictionary<string, object> claims)
        => new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAtUtc = expiresAt,
            Claims = claims
        };
}
