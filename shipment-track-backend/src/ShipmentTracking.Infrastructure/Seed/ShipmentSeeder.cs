using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.Entities;
using ShipmentTracking.Domain.ValueObjects;
using ShipmentTracking.Domain.Enums;
using ShipmentTracking.Infrastructure.Persistence;

namespace ShipmentTracking.Infrastructure.Seed;

/// <summary>
/// Seeds sample shipment data for testing and demonstration.
/// </summary>
public sealed class ShipmentSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ShipmentSeeder> _logger;
    private readonly IReferenceNumberGenerator _referenceNumberGenerator;

    public ShipmentSeeder(
        ApplicationDbContext dbContext,
        ILogger<ShipmentSeeder> logger,
        IReferenceNumberGenerator referenceNumberGenerator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _referenceNumberGenerator = referenceNumberGenerator;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Shipments.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Shipments already exist, skipping seeding");
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var shipments = new List<Shipment>();

        // Create sample shipments with different statuses
        var shipment1 = Shipment.Create(
            TrackingNumber.Create("VTX-202411-0001"),
            await _referenceNumberGenerator.GenerateAsync(cancellationToken),
            "REF-001",
            Port.Create("JEDSA"),
            Port.Create("AMMAN"),
            Weight.FromDecimal(150.5m),
            Volume.FromDecimal(2.3m),
            null, // customerId
            now.AddDays(-5), // departure
            now.AddDays(2), // arrival
            "Jeddah Port",
            "Electronics shipment",
            now.AddDays(-7));

        shipment1.AppendStatus(ShipmentStatus.OnVessel, "Loaded onto vessel MV Ocean Express", "Jeddah Port", now.AddDays(-4), null);
        shipments.Add(shipment1);

        var shipment2 = Shipment.Create(
            TrackingNumber.Create("VTX-202411-0002"),
            await _referenceNumberGenerator.GenerateAsync(cancellationToken),
            "REF-002",
            Port.Create("SHANG"),
            Port.Create("JEDSA"),
            Weight.FromDecimal(75.0m),
            Volume.FromDecimal(1.1m),
            null,
            now.AddDays(-10),
            now.AddDays(-2),
            "Jeddah Port",
            "Machinery parts",
            now.AddDays(-12));

        shipment2.AppendStatus(ShipmentStatus.OnVessel, "Departed Shanghai", "Shanghai Port", now.AddDays(-10), null);
        shipment2.AppendStatus(ShipmentStatus.ArrivedToPort, "Arrived at destination port", "Jeddah Port", now.AddDays(-3), null);
        shipment2.AppendStatus(ShipmentStatus.OutForDelivery, "Out for delivery to customer", "Jeddah Warehouse", now.AddDays(-1), null);
        shipments.Add(shipment2);

        var shipment3 = Shipment.Create(
            TrackingNumber.Create("VTX-202411-0003"),
            await _referenceNumberGenerator.GenerateAsync(cancellationToken),
            "REF-003",
            Port.Create("DUBAI"),
            Port.Create("RIYAD"),
            Weight.FromDecimal(200.0m),
            Volume.FromDecimal(3.5m),
            null,
            now.AddDays(-8),
            now.AddDays(-1),
            "Riyadh Warehouse",
            "Construction materials",
            now.AddDays(-10));

        shipment3.AppendStatus(ShipmentStatus.OnVessel, "Departed Dubai", "Dubai Port", now.AddDays(-8), null);
        shipment3.AppendStatus(ShipmentStatus.ArrivedToPort, "Arrived at destination port", "Riyadh Port", now.AddDays(-2), null);
        shipment3.AppendStatus(ShipmentStatus.Delivered, "Successfully delivered to customer", "Customer Address", now.AddDays(-1), null);
        shipments.Add(shipment3);

        var shipment4 = Shipment.Create(
            TrackingNumber.Create("VTX-202411-0004"),
            await _referenceNumberGenerator.GenerateAsync(cancellationToken),
            "REF-004",
            Port.Create("AMMAN"),
            Port.Create("BEIRU"),
            Weight.FromDecimal(45.5m),
            Volume.FromDecimal(0.8m),
            null,
            now.AddDays(1),
            now.AddDays(5),
            "Amman Warehouse",
            "Medical supplies",
            now.AddDays(-2));

        // shipment4 is already created with Received status, no need to append again
        shipments.Add(shipment4);

        var shipment5 = Shipment.Create(
            TrackingNumber.Create("VTX-202411-0005"),
            await _referenceNumberGenerator.GenerateAsync(cancellationToken),
            "REF-005",
            Port.Create("RIYAD"),
            Port.Create("JEDSA"),
            Weight.FromDecimal(120.0m),
            Volume.FromDecimal(1.8m),
            null,
            now.AddDays(-3),
            now.AddDays(3),
            "Riyadh Port",
            "Textile products",
            now.AddDays(-5));

        shipment5.AppendStatus(ShipmentStatus.OnVessel, "Loaded and departed", "Riyadh Port", now.AddDays(-3), null);
        shipments.Add(shipment5);

        try
        {
            await _dbContext.Shipments.AddRangeAsync(shipments, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully seeded {Count} sample shipments", shipments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed shipments");
            throw;
        }
    }
}
