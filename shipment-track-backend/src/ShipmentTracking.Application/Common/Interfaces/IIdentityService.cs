using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Users.Dto;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Abstraction over user management functionality.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task<Result<string>> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Attempts to sign a user in by validating the supplied password.
    /// </summary>
    Task<Result<UserSummary>> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by email.
    /// </summary>
    Task<UserSummary?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves roles for the provided user.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Generates an email verification token.
    /// </summary>
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms a user email address.
    /// </summary>
    Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken);

    /// <summary>
    /// Generates a password reset token.
    /// </summary>
    Task<Result<string>> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Resets the user password.
    /// </summary>
    Task<Result> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken);

    /// <summary>
    /// Ensures that the specified role exists.
    /// </summary>
    Task<Result> EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken);

    /// <summary>
    /// Adds user to a role if not already assigned.
    /// </summary>
    Task<Result> AddUserToRoleAsync(string userId, string roleName, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of users.
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the roles assigned to a user.
    /// </summary>
    Task<Result> UpdateUserRolesAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken);

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    Task<Result> UpdateUserAsync(
        string userId,
        string? email,
        string? firstName,
        string? lastName,
        string? role,
        string? status,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken);
}
