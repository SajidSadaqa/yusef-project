using MediatR;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Ports.Dto;
using AutoMapper;

namespace ShipmentTracking.Application.Features.Ports.Commands.UpdatePort;

public sealed record UpdatePortCommand(Guid Id, UpdatePortDto Dto) : IRequest<PortDto>;

internal sealed class UpdatePortCommandHandler : IRequestHandler<UpdatePortCommand, PortDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public UpdatePortCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTime)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<PortDto> Handle(UpdatePortCommand request, CancellationToken cancellationToken)
    {
        var port = await _context.Ports
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (port is null)
        {
            throw new NotFoundException($"Port with ID {request.Id} not found.");
        }

        // Update port properties
        port.Update(
            request.Dto.Name,
            request.Dto.Country,
            request.Dto.City,
            request.Dto.IsActive
        );

        // Update audit fields
        var userId = _currentUser.UserId;
        port.SetUpdatedAudit(userId, _dateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PortDto>(port);
    }
}
