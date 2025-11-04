using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Ports.Dto;

namespace ShipmentTracking.Application.Features.Ports.Queries.GetPortById;

public sealed record GetPortByIdQuery(Guid Id) : IRequest<PortDto>;

internal sealed class GetPortByIdQueryHandler : IRequestHandler<GetPortByIdQuery, PortDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPortByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PortDto> Handle(GetPortByIdQuery request, CancellationToken cancellationToken)
    {
        var port = await _context.Ports
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (port is null)
        {
            throw new NotFoundException($"Port with ID {request.Id} not found.");
        }

        return _mapper.Map<PortDto>(port);
    }
}
