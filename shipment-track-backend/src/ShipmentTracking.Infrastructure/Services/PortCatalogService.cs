using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Options;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Provides port validation using a configurable in-memory catalog.
/// </summary>
public sealed class PortCatalogService : IPortCatalogService
{
    private readonly IOptionsMonitor<PortCatalogOptions> _optionsMonitor;
    private ConcurrentDictionary<string, byte> _ports;

    public PortCatalogService(IOptionsMonitor<PortCatalogOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _ports = BuildPortDictionary(optionsMonitor.CurrentValue);
        _optionsMonitor.OnChange(options => _ports = BuildPortDictionary(options));
    }

    public Task<bool> PortExistsAsync(string portCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(portCode))
        {
            return Task.FromResult(false);
        }

        var normalized = portCode.Trim().ToUpperInvariant();
        return Task.FromResult(_ports.ContainsKey(normalized));
    }

    private static ConcurrentDictionary<string, byte> BuildPortDictionary(PortCatalogOptions options)
    {
        var dictionary = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        var source = options.Ports.Count > 0
            ? options.Ports
            : DefaultPorts;

        foreach (var port in source)
        {
            if (!string.IsNullOrWhiteSpace(port))
            {
                dictionary.TryAdd(port.Trim().ToUpperInvariant(), 0);
            }
        }

        return dictionary;
    }

    private static readonly string[] DefaultPorts =
    {
        "USNYC", // New York
        "CNSHA", // Shanghai
        "SGSIN", // Singapore
        "NLRTM", // Rotterdam
        "DEHAM", // Hamburg
        "AEJEA", // Jebel Ali
        "GBFXT", // Felixstowe
        "JPTYO", // Tokyo
        "BRSSZ", // Santos
        "AUMEL", // Melbourne
    };
}
