using System.Threading;
using System.Threading.Tasks;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Auth.Dto;

namespace ShipmentTracking.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token using a refresh token.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<TokenResponseDto>;

/// <summary>
/// Handles <see cref="RefreshTokenCommand"/>.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokenService.RefreshTokenAsync(request.RefreshToken, request.IpAddress, cancellationToken);

        return new TokenResponseDto
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            ExpiresAtUtc = result.AccessTokenExpiresAtUtc,
            Claims = result.Claims
        };
    }
}
