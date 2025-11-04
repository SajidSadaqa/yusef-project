using System;
using ShipmentTracking.Domain.Common;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.Entities;

/// <summary>
/// Represents an audit log entry for sensitive operations.
/// </summary>
public sealed class AuditLog : BaseEntity
{
    private AuditLog()
    {
        // EF Core
    }

    private AuditLog(
        string action,
        string entityName,
        Guid? entityId,
        Guid? userId,
        string? details,
        string? originIp,
        string? correlationId,
        DateTimeOffset occurredAtUtc)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new DomainValidationException("Audit action must be provided.");
        }

        if (string.IsNullOrWhiteSpace(entityName))
        {
            throw new DomainValidationException("Entity name must be provided for audit logs.");
        }

        Action = action.Trim();
        EntityName = entityName.Trim();
        EntityId = entityId;
        UserId = userId;
        Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim();
        OriginIp = originIp;
        CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? null : correlationId.Trim();
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
        MarkCreated(OccurredAtUtc);
    }

    /// <summary>
    /// Gets the action performed.
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the entity name that was impacted.
    /// </summary>
    public string EntityName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the optional entity identifier.
    /// </summary>
    public Guid? EntityId { get; private set; }

    /// <summary>
    /// Gets the optional user identifier who initiated the action.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Gets extended audit detail.
    /// </summary>
    public string? Details { get; private set; }

    /// <summary>
    /// Gets the origin IP address.
    /// </summary>
    public string? OriginIp { get; private set; }

    /// <summary>
    /// Gets the correlation identifier.
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Gets the occurrence timestamp.
    /// </summary>
    public DateTimeOffset OccurredAtUtc { get; private set; }

    /// <summary>
    /// Creates an audit log entry.
    /// </summary>
    public static AuditLog Create(
        string action,
        string entityName,
        Guid? entityId,
        Guid? userId,
        string? details,
        string? originIp,
        string? correlationId,
        DateTimeOffset occurredAtUtc)
        => new(action, entityName, entityId, userId, details, originIp, correlationId, occurredAtUtc);
}
