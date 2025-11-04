using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Common.Models;
using ShipmentTracking.Application.Features.Auth.Commands.Login;
using ShipmentTracking.Application.Features.Auth.Dto;
using ShipmentTracking.Application.Common.Exceptions;

namespace ShipmentTracking.Application.Tests.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ITokenService> _tokenService = new();

    [Fact]
    public async Task Handle_WithInvalidCredentials_ThrowsValidationException()
    {
        _identityService.Setup(service => service.ValidateUserCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserSummary>.Failure(new[] { "Invalid credentials" }));

        var handler = new LoginCommandHandler(_identityService.Object, _tokenService.Object);
        var command = new LoginCommand("user@test.com", "password", null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenResponse()
    {
        var user = new UserSummary
        {
            Id = Guid.NewGuid().ToString(),
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User",
            IsEmailConfirmed = true
        };

        _identityService.Setup(service => service.ValidateUserCredentialsAsync(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserSummary>.Success(user));

        _identityService.Setup(service => service.GetUserRolesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Staff" });

        var tokenResult = new TokenResult
        {
            AccessToken = "access",
            RefreshToken = "refresh",
            AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(15),
            Claims = new Dictionary<string, object>
            {
                ["uid"] = user.Id
            }
        };

        _tokenService.Setup(service => service.GenerateTokenPairAsync(user.Id, user.Email, It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResult);

        var handler = new LoginCommandHandler(_identityService.Object, _tokenService.Object);
        var command = new LoginCommand(user.Email, "password", "127.0.0.1");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(new TokenResponseDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            ExpiresAtUtc = tokenResult.AccessTokenExpiresAtUtc,
            Claims = tokenResult.Claims
        });
    }
}
