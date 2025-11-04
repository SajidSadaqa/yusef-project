using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipmentTracking.Application.Common.Exceptions;

/// <summary>
/// Represents application-level validation failures.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="failures">Collection of validation failures.</param>
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName, failure => failure.ErrorMessage)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">Collection of validation error messages.</param>
    public ValidationException(IEnumerable<string> errors)
        : this()
    {
        Errors = new Dictionary<string, string[]>
        {
            ["Error"] = errors.ToArray(),
        };
    }

    /// <summary>
    /// Gets the validation errors grouped by property name.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
}
