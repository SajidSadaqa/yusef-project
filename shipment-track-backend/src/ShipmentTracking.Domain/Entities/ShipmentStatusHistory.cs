using System;
using ShipmentTracking.Domain.Common;
using ShipmentTracking.Domain.Enums;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.Entities;

/// <summary>
/// Represents an event in the shipment status timeline.
/// </summary>
public sealed class ShipmentStatusHistory : BaseEntity, IAuditableEntity
{
    private ShipmentStatusHistory()
    {
        // Required by EF Core
    }

    private ShipmentStatusHistory(
        Guid shipmentId,
        ShipmentStatus status,
        string? description,
        string? location,
        DateTimeOffset eventTimeUtc,
        Guid? addedByUserId)
    {
        ShipmentId = shipmentId;
        Status = status;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        EventTimeUtc = eventTimeUtc.ToUniversalTime();
        AddedByUserId = addedByUserId;
        SetCreatedAudit(addedByUserId, EventTimeUtc);
    }

    /// <summary>
    /// Gets the identifier of the shipment the history entry belongs to.
    /// </summary>
    public Guid ShipmentId { get; private set; }

    /// <summary>
    /// Navigation property to the parent shipment.
    /// </summary>
    public Shipment? Shipment { get; private set; }

    /// <summary>
    /// Gets the status applied at the event.
    /// </summary>
    public ShipmentStatus Status { get; private set; }

    /// <summary>
    /// Gets the optional description about the event.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the location associated with the event.
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Gets the timestamp for the status change.
    /// </summary>
    public DateTimeOffset EventTimeUtc { get; private set; }

    /// <summary>
    /// Gets the user identifier that added the status entry.
    /// </summary>
    public Guid? AddedByUserId { get; private set; }

    /// <inheritdoc/>
    public Guid? CreatedByUserId { get; private set; }

    /// <inheritdoc/>
    public Guid? UpdatedByUserId { get; private set; }

    /// <summary>
    /// Creates a new status history entry.
    /// </summary>
    public static ShipmentStatusHistory Create(
        Guid shipmentId,
        ShipmentStatus status,
        string? description,
        string? location,
        DateTimeOffset eventTimeUtc,
        Guid? addedByUserId)
    {
        if (shipmentId == Guid.Empty)
        {
            throw new DomainValidationException("Shipment identifier must be provided for status history entries.");
        }

        return new ShipmentStatusHistory(
            shipmentId,
            status,
            description,
            location,
            eventTimeUtc,
            addedByUserId);
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
