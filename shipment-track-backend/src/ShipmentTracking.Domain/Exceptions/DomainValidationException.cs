using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShipmentTracking.Domain.Exceptions;

/// <summary>
/// Represents an error that occurs when a domain invariant is violated.
/// </summary>
[Serializable]
public sealed class DomainValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
    /// </summary>
    public DomainValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
    /// </summary>
    /// <param name="message">The validation message that describes the error.</param>
    public DomainValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
    /// </summary>
    /// <param name="message">The validation message that describes the error.</param>
    /// <param name="errors">The collection of detailed validation errors.</param>
    public DomainValidationException(string message, IReadOnlyCollection<string> errors)
        : base(message)
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
    /// </summary>
    /// <param name="message">The validation message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public DomainValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private DomainValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Errors = info.GetValue(nameof(Errors), typeof(IReadOnlyCollection<string>)) as IReadOnlyCollection<string>;
    }

    /// <summary>
    /// Gets the collection of validation errors associated with the exception.
    /// </summary>
    public IReadOnlyCollection<string>? Errors { get; }

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Errors), Errors, typeof(IReadOnlyCollection<string>));
    }
}
