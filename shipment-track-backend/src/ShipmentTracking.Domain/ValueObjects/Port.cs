using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.ValueObjects;

/// <summary>
/// Represents a UN/LOCODE port code (e.g. "SINVAL", "USNYC").
/// </summary>
public sealed partial class Port : ValueObject
{
    private const string PortPattern = @"^[A-Z]{5}$";

    private Port(string code)
    {
        Code = code;
    }

    /// <summary>
    /// Gets the normalized port code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Creates a port code value object.
    /// </summary>
    /// <param name="value">The raw port code.</param>
    /// <returns>A validated <see cref="Port"/> instance.</returns>
    /// <exception cref="DomainValidationException">Thrown when the port code is invalid.</exception>
    public static Port Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Port code cannot be empty.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!PortRegex().IsMatch(normalized))
        {
            throw new DomainValidationException(
                "Port code must be a valid UN/LOCODE (5 uppercase letters).");
        }

        return new Port(normalized);
    }

    /// <summary>
    /// Implicitly converts a port to its string representation.
    /// </summary>
    /// <param name="port">The port value object.</param>
    public static implicit operator string(Port port) => port.Code;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    /// <inheritdoc/>
    public override string ToString() => Code;

    [GeneratedRegex(PortPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PortRegex();
}
