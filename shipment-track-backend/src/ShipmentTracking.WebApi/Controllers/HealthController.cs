using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipmentTracking.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("healthz")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy", timestamp = DateTimeOffset.UtcNow });
}
