using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentByTracking;

namespace ShipmentTracking.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting("public")]
[Route("api/public/track")]
public sealed class PublicTrackingController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicTrackingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{trackingNumber}")]
    [ProducesResponseType(typeof(PublicTrackingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> TrackShipment(string trackingNumber, CancellationToken cancellationToken)
    {
        var query = new GetShipmentByTrackingQuery(trackingNumber);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
