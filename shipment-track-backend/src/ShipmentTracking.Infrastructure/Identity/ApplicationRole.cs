using Microsoft.AspNetCore.Identity;

namespace ShipmentTracking.Infrastructure.Identity;

/// <summary>
/// Custom identity role.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }
}
