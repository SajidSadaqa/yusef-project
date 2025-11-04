using System;
using ShipmentTracking.Domain.Common;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.Entities;

/// <summary>
/// Represents a persistent refresh token issued to an identity user.
/// </summary>
public sealed class RefreshToken : BaseEntity, IAuditableEntity
{
    private RefreshToken()
    {
        // Required by EF Core
    }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc,
        string? createdByIp,
        Guid? createdByUserId)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("Refresh token must be associated with a valid user.");
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainValidationException("Refresh token value cannot be empty.");
        }

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new DomainValidationException("Refresh token expiry must be in the future.");
        }

        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc.ToUniversalTime();
        CreatedByIp = createdByIp;

        SetCreatedAudit(createdByUserId, DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Gets the identifier for the user who owns the token.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the hashed refresh token value.
    /// </summary>
    public string TokenHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the token expires.
    /// </summary>
    public DateTimeOffset ExpiresAtUtc { get; private set; }

    /// <summary>
    /// Gets the optional IP address that issued the token.
    /// </summary>
    public string? CreatedByIp { get; private set; }

    /// <summary>
    /// Gets the optional IP address that revoked the token.
    /// </summary>
    public string? RevokedByIp { get; private set; }

    /// <summary>
    /// Gets the timestamp when the token was revoked, if applicable.
    /// </summary>
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    /// <summary>
    /// Gets the reason the token was revoked.
    /// </summary>
    public string? RevocationReason { get; private set; }

    /// <summary>
    /// Gets the replacement token hash when this token is rotated.
    /// </summary>
    public string? ReplacedByTokenHash { get; private set; }

    /// <inheritdoc/>
    public Guid? CreatedByUserId { get; private set; }

    /// <inheritdoc/>
    public Guid? UpdatedByUserId { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the token is currently active.
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    /// <summary>
    /// Gets a value indicating whether the token has expired.
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;

    /// <summary>
    /// Gets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAtUtc.HasValue;

    /// <summary>
    /// Creates a new refresh token instance.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="token">The opaque token value.</param>
    /// <param name="expiresAtUtc">The expiry timestamp.</param>
    /// <param name="createdByIp">The IP address that issued the token.</param>
    /// <param name="createdByUserId">The user identifier that issued the token.</param>
    /// <returns>A new <see cref="RefreshToken"/>.</returns>
    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc,
        string? createdByIp,
        Guid? createdByUserId)
        => new(userId, tokenHash, expiresAtUtc, createdByIp, createdByUserId);

    /// <summary>
    /// Revokes the token.
    /// </summary>
    /// <param name="reason">Reason for revocation.</param>
    /// <param name="revokedAtUtc">Revocation timestamp.</param>
    /// <param name="revokedByIp">IP address initiating revocation.</param>
    /// <param name="revokedByUserId">User initiating revocation.</param>
    public void Revoke(string reason, DateTimeOffset revokedAtUtc, string? revokedByIp, Guid? revokedByUserId)
    {
        if (IsRevoked)
        {
            throw new DomainValidationException("Refresh token has already been revoked.");
        }

        RevokedAtUtc = revokedAtUtc.ToUniversalTime();
        RevokedByIp = revokedByIp;
        RevocationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        SetUpdatedAudit(revokedByUserId, RevokedAtUtc.Value);
    }

    /// <summary>
    /// Links the token to its replacement value during rotation.
    /// </summary>
    /// <param name="replacementTokenHash">Replacement token hash.</param>
    public void SetReplacementToken(string replacementTokenHash)
    {
        if (string.IsNullOrWhiteSpace(replacementTokenHash))
        {
            throw new DomainValidationException("Replacement token value cannot be empty.");
        }

        ReplacedByTokenHash = replacementTokenHash.Trim();
    }

    /// <inheritdoc/>
    public void SetCreatedAudit(Guid? userId, DateTimeOffset timestampUtc)
    {
        CreatedByUserId = userId;
        MarkCreated(timestampUtc);
    }

    /// <inheritdoc/>
    public void SetUpdatedAudit(Guid? userId, DateTimeOffset timestampUtc)
    {
        UpdatedByUserId = userId;
        MarkUpdated(timestampUtc);
    }
}
