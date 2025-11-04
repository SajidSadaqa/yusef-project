using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipmentTracking.Application.Features.Shipments.Dto;
using ShipmentTracking.Integration.Tests.Infrastructure;
using ShipmentTracking.WebApi.Contracts.Shipments;
using Xunit;

namespace ShipmentTracking.Integration.Tests.Public;

/// <summary>
/// Exercises public tracking endpoints against a real PostgreSQL database.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PublicTrackingTests
{
    private readonly IntegrationTestFactory _factory;

    public PublicTrackingTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TrackShipment_ReturnsProjectedPublicDetails()
    {
        using var adminClient = await _factory.CreateAuthenticatedClientAsync();

        var createRequest = new CreateShipmentRequest(
            ReferenceNumber: $"REF-{Guid.NewGuid():N}".Substring(0, 12),
            CustomerId: Guid.NewGuid(),
            OriginPort: "USNYC",
            DestinationPort: "SGSIN",
            WeightKg: 120.5m,
            VolumeCbm: 2.75m,
            EstimatedDepartureUtc: DateTimeOffset.UtcNow.AddDays(1),
            EstimatedArrivalUtc: DateTimeOffset.UtcNow.AddDays(21),
            CurrentLocation: "New York Warehouse",
            Notes: "Handle with care.");

        var createResponse = await adminClient.PostAsJsonAsync("/api/shipments", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var shipment = await createResponse.Content.ReadFromJsonAsync<ShipmentDto>();
        shipment.Should().NotBeNull();

        var trackingClient = _factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });

        var trackResponse = await trackingClient.GetAsync($"/api/public/track/{shipment!.TrackingNumber}");

        trackResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var publicDto = await trackResponse.Content.ReadFromJsonAsync<PublicTrackingDto>();
        publicDto.Should().NotBeNull();
        publicDto!.TrackingNumber.Should().Be(shipment.TrackingNumber);
        publicDto.StatusHistory.Should().NotBeEmpty();
        publicDto.EstimatedArrivalUtc.Should().Be(shipment.EstimatedArrivalUtc);
    }
}
