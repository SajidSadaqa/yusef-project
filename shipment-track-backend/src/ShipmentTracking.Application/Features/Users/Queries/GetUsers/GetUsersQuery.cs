using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Users.Dto;

namespace ShipmentTracking.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Query to retrieve users with pagination.
/// </summary>
public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<UserDto>>;

/// <summary>
/// Handles <see cref="GetUsersQuery"/>.
/// </summary>
public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return _identityService.GetUsersAsync(request.PageNumber, request.PageSize, request.SearchTerm, cancellationToken);
    }
}
