using System;
using System.Collections.Generic;
using System.Linq;
using ShipmentTracking.Domain.Common;
using ShipmentTracking.Domain.Enums;
using ShipmentTracking.Domain.Exceptions;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Domain.Entities;

/// <summary>
/// Aggregate root that encapsulates shipment details and lifecycle.
/// </summary>
public sealed class Shipment : BaseEntity, IAuditableEntity
{
    private readonly List<ShipmentStatusHistory> _statusHistory = [];

    private Shipment()
    {
        // Required by EF Core
    }

    private Shipment(
        TrackingNumber trackingNumber,
        string referenceNumber,
        Port originPort,
        Port destinationPort,
        Weight weightKg,
        Volume volumeCbm,
        Guid? customerId,
        DateTimeOffset? estimatedDepartureUtc,
        DateTimeOffset? estimatedArrivalUtc,
        string? currentLocation,
        string? notes)
    {
        TrackingNumber = trackingNumber;
        ReferenceNumber = referenceNumber;
        OriginPort = originPort;
        DestinationPort = destinationPort;
        WeightKg = weightKg;
        VolumeCbm = volumeCbm;
        CustomerId = customerId;
        EstimatedDepartureUtc = estimatedDepartureUtc?.ToUniversalTime();
        EstimatedArrivalUtc = estimatedArrivalUtc?.ToUniversalTime();
        CurrentLocation = currentLocation?.Trim();
        Notes = notes?.Trim();

        CurrentStatus = ShipmentStatus.Received;

        ValidateSchedule();
    }

    /// <summary>
    /// Gets the tracking number for the shipment.
    /// </summary>
    public TrackingNumber TrackingNumber { get; private set; } = TrackingNumber.Create("VTX-197001-0000");

    /// <summary>
    /// Gets the merchant-provided reference number.
    /// </summary>
    public string ReferenceNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the optional customer identifier who owns the shipment.
    /// </summary>
    public Guid? CustomerId { get; private set; }

    /// <summary>
    /// Gets the origin port.
    /// </summary>
    public Port OriginPort { get; private set; } = Port.Create("AAAAA");

    /// <summary>
    /// Gets the destination port.
    /// </summary>
    public Port DestinationPort { get; private set; } = Port.Create("BBBBB");

    /// <summary>
    /// Gets the weight of the shipment in kilograms.
    /// </summary>
    public Weight WeightKg { get; private set; } = Weight.FromDecimal(1m);

    /// <summary>
    /// Gets the shipment volume in cubic meters.
    /// </summary>
    public Volume VolumeCbm { get; private set; } = Volume.FromDecimal(0.001m);

    /// <summary>
    /// Gets the current lifecycle status of the shipment.
    /// </summary>
    public ShipmentStatus CurrentStatus { get; private set; }

    /// <summary>
    /// Gets the estimated departure timestamp.
    /// </summary>
    public DateTimeOffset? EstimatedDepartureUtc { get; private set; }

    /// <summary>
    /// Gets the estimated arrival timestamp.
    /// </summary>
    public DateTimeOffset? EstimatedArrivalUtc { get; private set; }

    /// <summary>
    /// Gets the last known location for the shipment.
    /// </summary>
    public string? CurrentLocation { get; private set; }

    /// <summary>
    /// Gets the additional notes associated with the shipment.
    /// </summary>
    public string? Notes { get; private set; }

    /// <inheritdoc/>
    public Guid? CreatedByUserId { get; private set; }

    /// <inheritdoc/>
    public Guid? UpdatedByUserId { get; private set; }

    /// <summary>
    /// Gets the timeline of status updates.
    /// </summary>
    public IReadOnlyCollection<ShipmentStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    /// <summary>
    /// Factory method for creating new shipments.
    /// </summary>
    /// <param name="trackingNumber">Tracking number.</param>
    /// <param name="referenceNumber">Merchant reference number.</param>
    /// <param name="originPort">Origin port.</param>
    /// <param name="destinationPort">Destination port.</param>
    /// <param name="weightKg">Weight in kg.</param>
    /// <param name="volumeCbm">Volume in cbm.</param>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="estimatedDepartureUtc">Estimated departure.</param>
    /// <param name="estimatedArrivalUtc">Estimated arrival.</param>
    /// <param name="currentLocation">Current location.</param>
    /// <param name="notes">Notes.</param>
    /// <param name="createdAtUtc">Timestamp representing creation time.</param>
    /// <returns>The created shipment.</returns>
    public static Shipment Create(
        TrackingNumber trackingNumber,
        string referenceNumber,
        Port originPort,
        Port destinationPort,
        Weight weightKg,
        Volume volumeCbm,
        Guid? customerId,
        DateTimeOffset? estimatedDepartureUtc,
        DateTimeOffset? estimatedArrivalUtc,
        string? currentLocation,
        string? notes,
        DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new DomainValidationException("Reference number is required.");
        }

        var shipment = new Shipment(
            trackingNumber,
            referenceNumber.Trim(),
            originPort,
            destinationPort,
            weightKg,
            volumeCbm,
            customerId,
            estimatedDepartureUtc,
            estimatedArrivalUtc,
            currentLocation,
            notes);

        shipment.AppendStatus(
            ShipmentStatus.Received,
            "Shipment registered in the tracking platform.",
            string.IsNullOrWhiteSpace(currentLocation) ? originPort.Code : currentLocation,
            createdAtUtc.ToUniversalTime(),
            null);

        return shipment;
    }

    /// <summary>
    /// Updates non-status details of the shipment.
    /// </summary>
    public void UpdateDetails(
        string referenceNumber,
        Guid? customerId,
        Port originPort,
        Port destinationPort,
        Weight weightKg,
        Volume volumeCbm,
        DateTimeOffset? estimatedDepartureUtc,
        DateTimeOffset? estimatedArrivalUtc,
        string? currentLocation,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new DomainValidationException("Reference number is required.");
        }

        ReferenceNumber = referenceNumber.Trim();
        CustomerId = customerId;
        OriginPort = originPort;
        DestinationPort = destinationPort;
        WeightKg = weightKg;
        VolumeCbm = volumeCbm;
        EstimatedDepartureUtc = estimatedDepartureUtc?.ToUniversalTime();
        EstimatedArrivalUtc = estimatedArrivalUtc?.ToUniversalTime();
        CurrentLocation = string.IsNullOrWhiteSpace(currentLocation) ? null : currentLocation.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

        ValidateSchedule();
    }

    /// <summary>
    /// Appends a new status entry to the shipment timeline.
    /// </summary>
    /// <param name="status">The new shipment status.</param>
    /// <param name="description">Additional description for the status change.</param>
    /// <param name="location">Location of the status change.</param>
    /// <param name="eventTimeUtc">Timestamp for the status change.</param>
    /// <param name="updatedByUserId">User performing the update.</param>
    public void AppendStatus(
        ShipmentStatus status,
        string? description,
        string? location,
        DateTimeOffset eventTimeUtc,
        Guid? updatedByUserId)
    {
        if (_statusHistory.Count > 0)
        {
            var lastEntry = _statusHistory[^1];
            if (lastEntry.Status == status)
            {
                throw new DomainValidationException("Status is already the latest state for the shipment.");
            }

            if (eventTimeUtc < lastEntry.EventTimeUtc)
            {
                throw new DomainValidationException("Status event time cannot be earlier than the previous status update.");
            }
        }

        var historyEntry = ShipmentStatusHistory.Create(
            shipmentId: Id,
            status: status,
            description: description,
            location: location,
            eventTimeUtc: eventTimeUtc,
            addedByUserId: updatedByUserId);

        _statusHistory.Add(historyEntry);
        CurrentStatus = status;

        UpdatedByUserId = updatedByUserId;
        MarkUpdated(eventTimeUtc);
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

    /// <summary>
    /// Rebuilds the aggregate from persisted state.
    /// </summary>
    /// <param name="statusHistory">The status history collection.</param>
    public void LoadStatusHistory(IEnumerable<ShipmentStatusHistory> statusHistory)
    {
        _statusHistory.Clear();
        _statusHistory.AddRange(statusHistory.OrderBy(sh => sh.EventTimeUtc));

        if (_statusHistory.Count > 0)
        {
            CurrentStatus = _statusHistory[^1].Status;
        }
    }

    private void ValidateSchedule()
    {
        if (EstimatedArrivalUtc.HasValue && EstimatedDepartureUtc.HasValue &&
            EstimatedArrivalUtc < EstimatedDepartureUtc)
        {
            throw new DomainValidationException("Estimated arrival cannot be earlier than estimated departure.");
        }
    }
}
