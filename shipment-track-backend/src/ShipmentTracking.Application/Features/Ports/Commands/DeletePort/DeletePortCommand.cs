using System;
using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;

namespace ShipmentTracking.Application.Features.Ports.Commands.DeletePort;

public sealed record DeletePortCommand(Guid Id) : IRequest<Result>;

internal sealed class DeletePortCommandHandler : IRequestHandler<DeletePortCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeletePortCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeletePortCommand request, CancellationToken cancellationToken)
    {
        var port = await _context.Ports
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (port is null)
        {
            throw new NotFoundException($"Port with ID {request.Id} not found.");
        }

        // Check if port code is being used in any active shipments
        // The Shipment.OriginPort and DestinationPort are value objects with a Code property
        var shipmentsWithPort = await _context.Shipments
            .Where(s => !s.IsDeleted)
            .ToListAsync(cancellationToken);

        var isPortInUse = shipmentsWithPort.Any(s =>
            s.OriginPort.Code.Equals(port.Code, StringComparison.OrdinalIgnoreCase) ||
            s.DestinationPort.Code.Equals(port.Code, StringComparison.OrdinalIgnoreCase));

        if (isPortInUse)
        {
            return Result.Failure(new[] { "Cannot delete port that is referenced by existing shipments. Consider deactivating it instead." });
        }

        // Deactivate the port (soft delete by marking inactive)
        port.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
