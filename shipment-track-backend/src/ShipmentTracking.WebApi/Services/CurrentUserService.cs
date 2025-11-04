using System.Linq;
using System.Security.Claims;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.WebApi.Services;

/// <summary>
/// Provides details about the authenticated user based on the current HTTP context.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var identifier = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("uid");
            return Guid.TryParse(identifier, out var guid) ? guid : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public IReadOnlyCollection<string> Roles => User?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray() ?? Array.Empty<string>();

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
