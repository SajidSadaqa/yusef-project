namespace ShipmentTracking.Application.Common.Models;

/// <summary>
/// Represents parameters controlling pagination.
/// </summary>
public sealed class PaginationParameters
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private int _pageSize = DefaultPageSize;

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            <= 0 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value,
        };
    }
}
