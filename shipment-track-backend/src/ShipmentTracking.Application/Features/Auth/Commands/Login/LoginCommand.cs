using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppValidationException = ShipmentTracking.Application.Common.Exceptions.ValidationException;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Auth.Dto;

namespace ShipmentTracking.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command used to authenticate a user and issue tokens.
/// </summary>
public sealed record LoginCommand(string Email, string Password, string? IpAddress) : IRequest<TokenResponseDto>;

/// <summary>
/// Handles <see cref="LoginCommand"/>.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _identityService.ValidateUserCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!validationResult.Succeeded || validationResult.Value is null)
        {
            throw new AppValidationException(new[]
            {
                "Invalid email or password."
            });
        }

        var user = validationResult.Value;
        var roles = await _identityService.GetUserRolesAsync(user.Id, cancellationToken);

        var additionalClaims = new Dictionary<string, object>
        {
            ["firstName"] = user.FirstName ?? string.Empty,
            ["lastName"] = user.LastName ?? string.Empty,
            ["emailConfirmed"] = user.IsEmailConfirmed,
        };

        var tokenResult = await _tokenService.GenerateTokenPairAsync(
            user.Id,
            user.Email,
            roles,
            additionalClaims,
            cancellationToken);

        return new TokenResponseDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            ExpiresAtUtc = tokenResult.AccessTokenExpiresAtUtc,
            Claims = tokenResult.Claims
        };
    }
}
