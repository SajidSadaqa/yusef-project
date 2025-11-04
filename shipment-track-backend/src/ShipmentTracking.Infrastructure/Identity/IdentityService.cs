using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Users.Dto;

namespace ShipmentTracking.Infrastructure.Identity;

/// <summary>
/// Concrete implementation of <see cref="IIdentityService"/> backed by ASP.NET Identity.
/// </summary>
public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<string>> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            EmailConfirmed = false,
            IsActive = true
        };

        var identityResult = await _userManager.CreateAsync(user, password);

        if (!identityResult.Succeeded)
        {
            return Result<string>.Failure(identityResult.Errors.Select(error => error.Description));
        }

        return Result<string>.Success(user.Id.ToString());
    }

    public async Task<Result<UserSummary>> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return Result<UserSummary>.Failure(new[] { "Invalid email or password." });
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Result<UserSummary>.Failure(new[] { "Invalid email or password." });
        }

        user.LastLoginAtUtc = _dateTimeProvider.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<UserSummary>.Success(MapToSummary(user));
    }

    public async Task<UserSummary?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user is null ? null : MapToSummary(user);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var guid))
        {
            return Array.Empty<string>();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == guid, cancellationToken);
        if (user is null)
        {
            return Array.Empty<string>();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<string>.Failure(new[] { "User not found." });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Result<string>.Success(token);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<string>.Failure(new[] { "User not found." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return Result<string>.Success(token);
    }

    public async Task<Result> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return Result.Success();
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole(roleName));
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> AddUserToRoleAsync(string userId, string roleName, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        await EnsureRoleExistsAsync(roleName, cancellationToken);

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return Result.Success();
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToUpperInvariant();
            query = query.Where(user => user.Email!.ToUpper().Contains(normalized)
                                        || (user.FirstName != null && user.FirstName.ToUpper().Contains(normalized))
                                        || (user.LastName != null && user.LastName.ToUpper().Contains(normalized)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(user => user.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToArray()
            });
        }

        return new PagedResult<UserDto>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<Result> UpdateUserRolesAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToRemove = currentRoles.Except(roles, StringComparer.OrdinalIgnoreCase).ToArray();
        var rolesToAdd = roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return Result.Failure(removeResult.Errors.Select(e => e.Description));
            }
        }

        foreach (var role in rolesToAdd)
        {
            await EnsureRoleExistsAsync(role, cancellationToken);
            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                return Result.Failure(addResult.Errors.Select(e => e.Description));
            }
        }

        return Result.Success();
    }

    private async Task<ApplicationUser?> FindUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var guid))
        {
            return null;
        }

        return await _userManager.Users.FirstOrDefaultAsync(user => user.Id == guid, cancellationToken);
    }

    private static UserSummary MapToSummary(ApplicationUser user) => new()
    {
        Id = user.Id.ToString(),
        Email = user.Email!,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PhoneNumber = user.PhoneNumber,
        IsEmailConfirmed = user.EmailConfirmed
    };
}
