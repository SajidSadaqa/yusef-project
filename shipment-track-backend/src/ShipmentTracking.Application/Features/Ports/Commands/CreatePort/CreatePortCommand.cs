using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Ports.Dto;
using ShipmentTracking.Domain.Entities;

namespace ShipmentTracking.Application.Features.Ports.Commands.CreatePort;

/// <summary>
/// Command to create a new port.
/// </summary>
public sealed record CreatePortCommand(CreatePortDto Payload) : IRequest<PortDto>;

/// <summary>
/// Handles <see cref="CreatePortCommand"/>.
/// </summary>
public sealed class CreatePortCommandHandler : IRequestHandler<CreatePortCommand, PortDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPortCodeGenerator _codeGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreatePortCommandHandler(
        IApplicationDbContext dbContext,
        IPortCodeGenerator codeGenerator,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _codeGenerator = codeGenerator;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<PortDto> Handle(CreatePortCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        // Generate code if not provided
        var code = string.IsNullOrWhiteSpace(payload.Code)
            ? await _codeGenerator.GenerateAsync(payload.Name, payload.Country, cancellationToken)
            : payload.Code;

        var port = PortMaster.Create(code, payload.Name, payload.Country, payload.City, isActive: true);

        var now = _dateTimeProvider.UtcNow;
        port.SetCreatedAudit(_currentUserService.UserId, now);
        port.SetUpdatedAudit(_currentUserService.UserId, now);

        _dbContext.Ports.Add(port);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PortDto>(port);
    }
}
