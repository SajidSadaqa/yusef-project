using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShipmentTracking.Infrastructure.Identity;
using ShipmentTracking.Infrastructure.Options;

namespace ShipmentTracking.Infrastructure.Seed;

/// <summary>
/// Seeds default roles and administrator account.
/// </summary>
public sealed class IdentitySeeder(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<AdminUserOptions> adminOptions,
    ILogger<IdentitySeeder> logger)
{
    private static readonly string[] DefaultRoles = { "Admin", "Staff", "Customer" };

    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AdminUserOptions _adminOptions = adminOptions.Value;
    private readonly ILogger<IdentitySeeder> _logger = logger;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureRolesAsync();
        await EnsureAdminUserAsync();
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var role in DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new ApplicationRole(role));
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join(",", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task EnsureAdminUserAsync()
    {
        var adminUser = await _userManager.FindByEmailAsync(_adminOptions.Email);
        if (adminUser is not null)
        {
            return;
        }

        adminUser = new ApplicationUser
        {
            UserName = _adminOptions.Email,
            Email = _adminOptions.Email,
            FirstName = _adminOptions.FirstName,
            LastName = _adminOptions.LastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var createResult = await _userManager.CreateAsync(adminUser, _adminOptions.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogError("Failed to create admin user: {Errors}", string.Join(",", createResult.Errors.Select(e => e.Description)));
            return;
        }

        await _userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
