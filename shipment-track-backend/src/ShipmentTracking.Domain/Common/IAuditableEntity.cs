using System;

namespace ShipmentTracking.Domain.Common;

/// <summary>
/// Represents an entity that captures user auditing information.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets the identifier of the user that created the entity.
    /// </summary>
    Guid? CreatedByUserId { get; }

    /// <summary>
    /// Gets the identifier of the user that last updated the entity.
    /// </summary>
    Guid? UpdatedByUserId { get; }

    /// <summary>
    /// Sets the creation audit details.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="timestampUtc">The timestamp when the entity was created.</param>
    void SetCreatedAudit(Guid? userId, DateTimeOffset timestampUtc);

    /// <summary>
    /// Sets the modification audit details.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="timestampUtc">The timestamp when the entity was updated.</param>
    void SetUpdatedAudit(Guid? userId, DateTimeOffset timestampUtc);
}
