using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Infrastructure.Seed;

/// <summary>
/// Seeds initial port master data into the database.
/// </summary>
public sealed class PortSeeder
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<PortSeeder> _logger;

    public PortSeeder(
        IApplicationDbContext dbContext,
        IDateTimeProvider dateTimeProvider,
        ILogger<PortSeeder> logger)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingCount = await _dbContext.Ports.CountAsync(cancellationToken);
        if (existingCount > 0)
        {
            _logger.LogInformation("Port master data already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Seeding initial port master data...");

        var now = _dateTimeProvider.UtcNow;
        var ports = GetInitialPorts(now);

        _dbContext.Ports.AddRange(ports);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully seeded {Count} ports.", ports.Count);
    }

    private List<PortMaster> GetInitialPorts(DateTimeOffset createdAt)
    {
        var ports = new List<(string Code, string Name, string Country, string City)>
        {
            ("USNYC", "New York", "United States", "New York"),
            ("CNSHA", "Shanghai", "China", "Shanghai"),
            ("SGSIN", "Singapore", "Singapore", "Singapore"),
            ("NLRTM", "Rotterdam", "Netherlands", "Rotterdam"),
            ("DEHAM", "Hamburg", "Germany", "Hamburg"),
            ("AEJEA", "Jebel Ali", "United Arab Emirates", "Dubai"),
            ("GBFXT", "Felixstowe", "United Kingdom", "Felixstowe"),
            ("JPTYO", "Tokyo", "Japan", "Tokyo"),
            ("BRSSZ", "Santos", "Brazil", "Santos"),
            ("AUMEL", "Melbourne", "Australia", "Melbourne"),
            // Additional Middle Eastern ports
            ("JOAQJ", "Aqaba", "Jordan", "Aqaba"),
            ("SYLTK", "Latakia", "Syria", "Latakia"),
            ("LBBEY", "Beirut", "Lebanon", "Beirut"),
            ("EGALY", "Alexandria", "Egypt", "Alexandria"),
            ("SAJED", "Jeddah", "Saudi Arabia", "Jeddah"),
        };

        return ports.Select(p =>
        {
            var port = PortMaster.Create(p.Code, p.Name, p.Country, p.City, isActive: true);
            port.SetCreatedAudit(null, createdAt); // System-created
            port.SetUpdatedAudit(null, createdAt);
            return port;
        }).ToList();
    }
}
