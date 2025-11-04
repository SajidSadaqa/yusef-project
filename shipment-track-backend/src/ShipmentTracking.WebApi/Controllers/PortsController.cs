using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Application.Features.Ports.Commands.CreatePort;
using ShipmentTracking.Application.Features.Ports.Commands.UpdatePort;
using ShipmentTracking.Application.Features.Ports.Commands.DeletePort;
using ShipmentTracking.Application.Features.Ports.Dto;
using ShipmentTracking.Application.Features.Ports.Queries.ListPorts;
using ShipmentTracking.Application.Features.Ports.Queries.GetPortById;
using ShipmentTracking.WebApi.Contracts.Ports;

namespace ShipmentTracking.WebApi.Controllers;

/// <summary>
/// API endpoints for port management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PortsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all ports with optional filtering and search.
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Allow public access for shipment forms
    [ProducesResponseType(typeof(List<PortDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPorts(
        [FromQuery] bool activeOnly = true,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? country = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListPortsQuery(activeOnly, searchTerm, country);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a port by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PortDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPortById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPortByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new port.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PortDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePort([FromBody] CreatePortRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePortCommand(new CreatePortDto
        {
            Name = request.Name,
            Country = request.Country,
            City = request.City,
            Code = request.Code
        });

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPortById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing port.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PortDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePort(Guid id, [FromBody] UpdatePortRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePortCommand(id, new UpdatePortDto
        {
            Name = request.Name,
            Country = request.Country,
            City = request.City,
            IsActive = request.IsActive
        });

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a port (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePort(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePortCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return NoContent();
    }
}
