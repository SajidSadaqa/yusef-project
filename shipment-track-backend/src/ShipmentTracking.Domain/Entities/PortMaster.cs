using ShipmentTracking.Domain.Common;

namespace ShipmentTracking.Domain.Entities;

/// <summary>
/// Represents a port/terminal master record where shipments originate from or are destined to.
/// </summary>
public sealed class PortMaster : BaseEntity, IAuditableEntity
{
    private PortMaster(string code, string name, string country, string? city, bool isActive)
    {
        Code = code;
        Name = name;
        Country = country;
        City = city;
        IsActive = isActive;
    }

    /// <summary>
    /// Gets the unique port code (e.g., USNYC, CNSHA).
    /// Maximum 5 characters, following UN/LOCODE standard.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the full name of the port.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the country where the port is located.
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Gets the city where the port is located.
    /// </summary>
    public string? City { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the port is active and available for selection.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <inheritdoc/>
    public Guid? CreatedByUserId { get; private set; }

    /// <inheritdoc/>
    public Guid? UpdatedByUserId { get; private set; }

    /// <summary>
    /// Creates a new port instance.
    /// </summary>
    /// <param name="code">The unique port code (max 5 characters).</param>
    /// <param name="name">The full name of the port.</param>
    /// <param name="country">The country where the port is located.</param>
    /// <param name="city">The city where the port is located (optional).</param>
    /// <param name="isActive">Whether the port is active.</param>
    /// <returns>A new port instance.</returns>
    public static PortMaster Create(string code, string name, string country, string? city = null, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Port code cannot be empty.", nameof(code));
        }

        if (code.Length > 5)
        {
            throw new ArgumentException("Port code cannot exceed 5 characters.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Port name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Port country cannot be empty.", nameof(country));
        }

        return new PortMaster(code.ToUpperInvariant().Trim(), name.Trim(), country.Trim(), city?.Trim(), isActive);
    }

    /// <summary>
    /// Updates the port details.
    /// </summary>
    public void UpdateDetails(string name, string country, string? city)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Port name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Port country cannot be empty.", nameof(country));
        }

        Name = name.Trim();
        Country = country.Trim();
        City = city?.Trim();
    }

    /// <summary>
    /// Activates the port.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the port.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates the port with all details including active status.
    /// </summary>
    public void Update(string name, string country, string? city, bool isActive)
    {
        UpdateDetails(name, country, city);
        IsActive = isActive;
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
