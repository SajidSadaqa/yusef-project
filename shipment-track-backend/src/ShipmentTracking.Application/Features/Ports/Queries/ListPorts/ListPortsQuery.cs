using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Ports.Dto;

namespace ShipmentTracking.Application.Features.Ports.Queries.ListPorts;

/// <summary>
/// Query to list ports with optional filtering and search.
/// </summary>
public sealed record ListPortsQuery(
    bool ActiveOnly = true,
    string? SearchTerm = null,
    string? Country = null) : IRequest<List<PortDto>>;

/// <summary>
/// Handles <see cref="ListPortsQuery"/>.
/// </summary>
public sealed class ListPortsQueryHandler : IRequestHandler<ListPortsQuery, List<PortDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ListPortsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<PortDto>> Handle(ListPortsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Ports.AsNoTracking();

        if (request.ActiveOnly)
        {
            query = query.Where(p => p.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower().Trim();
            query = query.Where(p =>
                p.Code.ToLower().Contains(searchTerm) ||
                p.Name.ToLower().Contains(searchTerm) ||
                (p.City != null && p.City.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.ToLower().Trim();
            query = query.Where(p => p.Country.ToLower().Contains(country));
        }

        var ports = await query
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<PortDto>>(ports);
    }
}
