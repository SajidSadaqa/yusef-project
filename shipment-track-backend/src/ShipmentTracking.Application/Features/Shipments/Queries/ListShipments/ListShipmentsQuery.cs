using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Domain.Enums;

namespace ShipmentTracking.Application.Features.Shipments.Queries.ListShipments;

/// <summary>
/// Query for retrieving shipments with pagination and filtering.
/// </summary>
public sealed record ListShipmentsQuery : IRequest<PagedResult<ShipmentDto>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public ShipmentStatus? Status { get; init; }

    public DateTimeOffset? FromDateUtc { get; init; }

    public DateTimeOffset? ToDateUtc { get; init; }

    public string? SearchTerm { get; init; }
}

/// <summary>
/// Handles <see cref="ListShipmentsQuery"/>.
/// </summary>
public sealed class ListShipmentsQueryHandler : IRequestHandler<ListShipmentsQuery, PagedResult<ShipmentDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ListShipmentsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PagedResult<ShipmentDto>> Handle(ListShipmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Shipments
            .AsNoTracking()
            .Where(s => !s.IsDeleted);

        if (request.Status.HasValue)
        {
            query = query.Where(s => s.CurrentStatus == request.Status);
        }

        if (request.FromDateUtc.HasValue)
        {
            query = query.Where(s => s.CreatedAtUtc >= request.FromDateUtc.Value);
        }

        if (request.ToDateUtc.HasValue)
        {
            query = query.Where(s => s.CreatedAtUtc <= request.ToDateUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = $"%{request.SearchTerm.Trim().ToUpperInvariant()}%";
            query = query.Where(s =>
                EF.Functions.Like(s.TrackingNumber.Value.ToUpper(), term) ||
                EF.Functions.Like(s.ReferenceNumber.ToUpper(), term));
        }

        query = query.OrderByDescending(s => s.CreatedAtUtc);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var pagedQuery = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var items = await pagedQuery
            .ProjectTo<ShipmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<ShipmentDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
