using System;
using System.Collections.Generic;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.ValueObjects;

/// <summary>
/// Represents a shipment's volume in cubic meters.
/// </summary>
public sealed class Volume : ValueObject
{
    private Volume(decimal value)
    {
        Value = decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Gets the volume value in cubic meters.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Creates a volume value object after validation.
    /// </summary>
    /// <param name="value">The volume in cubic meters.</param>
    /// <returns>A validated <see cref="Volume"/> instance.</returns>
    /// <exception cref="DomainValidationException">Thrown when the volume is invalid.</exception>
    public static Volume FromDecimal(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainValidationException("Volume must be greater than zero cubic meters.");
        }

        if (value > 10_000m)
        {
            throw new DomainValidationException("Volume exceeds the maximum supported threshold (10,000 CBM).");
        }

        return new Volume(value);
    }

    /// <summary>
    /// Implicit conversion to <see cref="decimal"/>.
    /// </summary>
    /// <param name="volume">The volume value object.</param>
    public static implicit operator decimal(Volume volume) => volume.Value;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Value:0.####} cbm";
}
