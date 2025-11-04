using System;

namespace ShipmentTracking.Domain.Common;

/// <summary>
/// Base type for all aggregate roots and entities within the domain model.
/// Provides identifier, auditing timestamps, and soft delete semantics.
/// </summary>
public abstract class BaseEntity
{
    private DateTimeOffset? _createdAtUtc;

    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the timestamp, in UTC, when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc
    {
        get => _createdAtUtc ??= DateTimeOffset.UtcNow;
        private set => _createdAtUtc = value;
    }

    /// <summary>
    /// Gets the timestamp, in UTC, when the entity was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the entity has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Marks the entity as newly created at the provided timestamp.
    /// </summary>
    /// <param name="timestampUtc">The UTC timestamp that represents creation time.</param>
    public void MarkCreated(DateTimeOffset timestampUtc)
    {
        CreatedAtUtc = timestampUtc;
    }

    /// <summary>
    /// Updates the last modified timestamp.
    /// </summary>
    /// <param name="timestampUtc">The UTC timestamp for the update.</param>
    public void MarkUpdated(DateTimeOffset timestampUtc)
    {
        UpdatedAtUtc = timestampUtc;
    }

    /// <summary>
    /// Performs a soft delete on the entity.
    /// </summary>
    public void SoftDelete()
    {
        IsDeleted = true;
        MarkUpdated(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Restores an entity that was previously soft deleted.
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        MarkUpdated(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Allows derived types or ORMs to set a known identifier.
    /// </summary>
    /// <param name="id">The identifier to assign.</param>
    protected void SetId(Guid id)
    {
        Id = id;
    }
}
