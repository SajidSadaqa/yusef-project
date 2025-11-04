using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.ValueObjects;

/// <summary>
/// Represents a shipment tracking number that follows the VTX-YYYYMM-#### pattern.
/// </summary>
public sealed partial class TrackingNumber : ValueObject
{
    private const string TrackingNumberPattern = @"^VTX-(?<year>\d{4})(?<month>0[1-9]|1[0-2])-(?<sequence>\d{4})$";

    private TrackingNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the string value of the tracking number.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a tracking number instance after validating the provided value.
    /// </summary>
    /// <param name="value">The raw tracking number.</param>
    /// <returns>A validated <see cref="TrackingNumber"/>.</returns>
    /// <exception cref="DomainValidationException">Thrown when the value does not match the expected pattern.</exception>
    public static TrackingNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Tracking number cannot be empty.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!TrackingNumberRegex().IsMatch(normalized))
        {
            throw new DomainValidationException(
                "Tracking number must follow the pattern VTX-YYYYMM-#### with a valid month.");
        }

        return new TrackingNumber(normalized);
    }

    /// <summary>
    /// Implicitly converts a tracking number to its string representation.
    /// </summary>
    /// <param name="trackingNumber">The tracking number.</param>
    public static implicit operator string(TrackingNumber trackingNumber) => trackingNumber.Value;

    /// <summary>
    /// Returns the string representation of the tracking number.
    /// </summary>
    /// <returns>The tracking number value.</returns>
    public override string ToString() => Value;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(TrackingNumberPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex TrackingNumberRegex();
}
