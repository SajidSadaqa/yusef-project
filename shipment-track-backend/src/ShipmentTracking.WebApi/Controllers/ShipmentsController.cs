using ShipmentTracking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Shipments.Commands.AppendStatus;
using ShipmentTracking.Application.Features.Shipments.Commands.CreateShipment;
using ShipmentTracking.Application.Features.Shipments.Commands.DeleteShipment;
using ShipmentTracking.Application.Features.Shipments.Commands.UpdateShipment;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Application.Features.Shipments.Queries.GetShipmentHistory;
using ShipmentTracking.Application.Features.Shipments.Queries.ListShipments;
using ShipmentTracking.WebApi.Contracts.Shipments;

namespace ShipmentTracking.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public sealed class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateShipmentCommand(new CreateShipmentDto
        {
            CustomerReference = request.CustomerReference,
            CustomerId = request.CustomerId,
            OriginPort = request.OriginPort,
            DestinationPort = request.DestinationPort,
            WeightKg = request.WeightKg,
            VolumeCbm = request.VolumeCbm,
            EstimatedDepartureUtc = request.EstimatedDepartureUtc,
            EstimatedArrivalUtc = request.EstimatedArrivalUtc,
            CurrentLocation = request.CurrentLocation,
            Notes = request.Notes
        });

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetShipmentHistory), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShipment(Guid id, [FromBody] UpdateShipmentRequest request, CancellationToken cancellationToken)
    {
        if (id != request.ShipmentId)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var command = new UpdateShipmentCommand(new UpdateShipmentDto
        {
            ShipmentId = request.ShipmentId,
            CustomerReference = request.CustomerReference,
            CustomerId = request.CustomerId,
            OriginPort = request.OriginPort,
            DestinationPort = request.DestinationPort,
            WeightKg = request.WeightKg,
            VolumeCbm = request.VolumeCbm,
            EstimatedDepartureUtc = request.EstimatedDepartureUtc,
            EstimatedArrivalUtc = request.EstimatedArrivalUtc,
            CurrentLocation = request.CurrentLocation,
            Notes = request.Notes
        });

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AppendStatus(Guid id, [FromBody] AppendShipmentStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new AppendStatusCommand(new AppendShipmentStatusDto
        {
            ShipmentId = id,
            Status = request.Status,
            Description = request.Description,
            Location = request.Location,
            EventTimeUtc = request.EventTimeUtc
        });

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShipment(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteShipmentCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ShipmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListShipments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] DateTimeOffset? fromDateUtc = null,
        [FromQuery] DateTimeOffset? toDateUtc = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListShipmentsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = Enum.TryParse(status, true, out ShipmentStatus parsedStatus) ? parsedStatus : null,
            FromDateUtc = fromDateUtc,
            ToDateUtc = toDateUtc,
            SearchTerm = search
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ShipmentStatusHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShipmentHistory(Guid id, CancellationToken cancellationToken)
    {
        var history = await _mediator.Send(new GetShipmentHistoryQuery(id), cancellationToken);
        return Ok(history);
    }
}
