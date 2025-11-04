using System.Threading;
using FluentAssertions;
using Moq;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Application.Features.Shipments.Commands.CreateShipment;
using ShipmentTracking.Application.Features.Shipments.Dto;

namespace ShipmentTracking.Application.Tests.Shipments;

public sealed class CreateShipmentCommandValidatorTests
{
    private readonly Mock<IPortCatalogService> _portCatalogMock = new();

    private CreateShipmentCommandValidator CreateValidator()
    {
        _portCatalogMock.Setup(service => service.PortExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return new CreateShipmentCommandValidator(_portCatalogMock.Object);
    }

    [Fact]
    public async Task Validate_WithValidPayload_ReturnsSuccess()
    {
        var validator = CreateValidator();
        var command = new CreateShipmentCommand(new CreateShipmentDto
        {
            CustomerReference = "ORD-123",
            OriginPort = "USNYC",
            DestinationPort = "CNSHA",
            WeightKg = 10.5m,
            VolumeCbm = 2.1m
        });

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithInvalidPorts_ReturnsErrors()
    {
        _portCatalogMock.Setup(service => service.PortExistsAsync("USNYC", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _portCatalogMock.Setup(service => service.PortExistsAsync("INVALID", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var validator = new CreateShipmentCommandValidator(_portCatalogMock.Object);
        var command = new CreateShipmentCommand(new CreateShipmentDto
        {
            CustomerReference = "ORD-123",
            OriginPort = "USNYC",
            DestinationPort = "INVALID",
            WeightKg = 10.5m,
            VolumeCbm = 2.1m
        });

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("DestinationPort"));
    }
}
