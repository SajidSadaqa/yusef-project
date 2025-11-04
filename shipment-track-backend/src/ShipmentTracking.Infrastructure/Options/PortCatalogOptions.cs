using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.Infrastructure.Options;

/// <summary>
/// Options representing the allowed port codes for validation.
/// </summary>
public sealed class PortCatalogOptions
{
    public const string SectionName = "PortCatalog";

    [MinLength(1)]
    public IList<string> Ports { get; set; } = new List<string>();
}
