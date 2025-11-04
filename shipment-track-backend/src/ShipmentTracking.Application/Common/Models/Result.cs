using System.Collections.Generic;
using System.Linq;

namespace ShipmentTracking.Application.Common.Models;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public sealed class Result
{
    private Result(bool succeeded, IEnumerable<string>? errors = null)
    {
        Succeeded = succeeded;
        Errors = errors?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// Gets the error messages.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failure result with error messages.
    /// </summary>
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}
