using System;
using System.Collections.Generic;
using ShipmentTracking.Domain.Exceptions;

namespace ShipmentTracking.Domain.ValueObjects;

/// <summary>
/// Represents a shipment's weight in kilograms.
/// </summary>
public sealed class Weight : ValueObject
{
    private Weight(decimal value)
    {
        Value = decimal.Round(value, 3, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Gets the weight value in kilograms.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Creates a weight value object.
    /// </summary>
    /// <param name="value">The weight in kilograms.</param>
    /// <returns>A validated <see cref="Weight"/> instance.</returns>
    /// <exception cref="DomainValidationException">Thrown when the weight is invalid.</exception>
    public static Weight FromDecimal(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainValidationException("Weight must be greater than zero kilograms.");
        }

        if (value > 100_000m)
        {
            throw new DomainValidationException("Weight exceeds the maximum supported threshold (100,000 kg).");
        }

        return new Weight(value);
    }

    /// <summary>
    /// Implicit conversion to <see cref="decimal"/>.
    /// </summary>
    /// <param name="weight">The weight value object.</param>
    public static implicit operator decimal(Weight weight) => weight.Value;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Value:0.###} kg";
}
