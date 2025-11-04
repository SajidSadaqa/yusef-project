using System.Collections.Generic;
using System.Linq;

namespace ShipmentTracking.Domain.ValueObjects;

/// <summary>
/// Provides a base type for value objects using structural equality semantics.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Retrieves the atomic values that participate in equality comparison.
    /// </summary>
    /// <returns>An ordered sequence of values used to compare objects.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other || obj.GetType() != GetType())
        {
            return false;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            return GetEqualityComponents()
                .Aggregate(0, (current, component) => (current * 397) ^ (component?.GetHashCode() ?? 0));
        }
    }
}
