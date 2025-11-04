using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipmentTracking.Integration.Tests.Infrastructure;
using ShipmentTracking.WebApi.Contracts.Auth;
using Xunit;

namespace ShipmentTracking.Integration.Tests.Auth;

/// <summary>
/// Covers authentication endpoints end-to-end.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class AuthFlowTests
{
    private readonly IntegrationTestFactory _factory;

    public AuthFlowTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithSeededAdmin_ReturnsAccessAndRefreshTokens()
    {
        using var client = _factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            _factory.AdminEmail,
            _factory.AdminPassword));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        tokenResponse.Should().NotBeNull();
        tokenResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokenResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        tokenResponse.ExpiresAtUtc.Should().BeAfter(DateTimeOffset.UtcNow);
        tokenResponse.Claims.Should().ContainKey("sub");

        var fakeEmailService = _factory.GetFakeEmailService();
        fakeEmailService.Emails.Should().BeEmpty("login should not trigger outbound emails");
    }

    [Fact]
    public async Task RefreshToken_RotatesTokensSuccessfully()
    {
        using var client = _factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            _factory.AdminEmail,
            _factory.AdminPassword));

        loginResponse.EnsureSuccessStatusCode();

        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new InvalidOperationException("Cannot read login token response");

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest(
            loginTokens.RefreshToken,
            IpAddress: "127.0.0.1"));

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshedTokens = await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>();
        refreshedTokens.Should().NotBeNull();
        refreshedTokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        refreshedTokens.AccessToken.Should().NotBe(loginTokens.AccessToken);
        refreshedTokens.RefreshToken.Should().NotBe(loginTokens.RefreshToken);
    }
}
