using System;

namespace ShipmentTracking.Application.Common.Exceptions;

/// <summary>
/// Represents an error when a user attempts to access a resource they do not have permissions for.
/// </summary>
public sealed class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class.
    /// </summary>
    public ForbiddenAccessException()
        : base("You do not have permission to perform this action.")
    {
    }
}
