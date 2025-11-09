using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Provides port validation using the database with in-memory caching for performance.
/// </summary>
public sealed class PortCatalogService : IPortCatalogService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ActivePortCodes";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public PortCatalogService(IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    public async Task<bool> PortExistsAsync(string portCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(portCode))
        {
            return false;
        }

        var normalized = portCode.Trim().ToUpperInvariant();

        // Try to get from cache first
        if (_cache.TryGetValue<HashSet<string>>(CacheKey, out var cachedPorts) && cachedPorts != null)
        {
            return cachedPorts.Contains(normalized);
        }

        // If not in cache, create a scope to query database and cache the result
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var activePorts = await dbContext.Ports
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var portSet = new HashSet<string>(activePorts, StringComparer.OrdinalIgnoreCase);

        _cache.Set(CacheKey, portSet, CacheDuration);

        return portSet.Contains(normalized);
    }
}
