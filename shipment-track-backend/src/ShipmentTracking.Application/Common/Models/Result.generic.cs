using System.Collections.Generic;
using System.Linq;

namespace ShipmentTracking.Application.Common.Models;

/// <summary>
/// Represents the outcome of an operation that produces a value.
/// </summary>
/// <typeparam name="T">Type of value.</typeparam>
public sealed class Result<T>
{
    private Result(bool succeeded, T? value, IEnumerable<string>? errors = null)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// Gets the value returned by the operation.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error messages.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}
