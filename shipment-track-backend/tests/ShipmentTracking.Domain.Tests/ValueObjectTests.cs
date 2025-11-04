using Xunit;
using FluentAssertions;
using ShipmentTracking.Domain.Exceptions;
using ShipmentTracking.Domain.ValueObjects;

namespace ShipmentTracking.Domain.Tests;

public sealed class ValueObjectTests
{
    [Theory]
    [InlineData("VTX-202501-0001")]
    [InlineData("vtx-202512-9999")]
    public void TrackingNumber_Create_WithValidValue_ReturnsValueObject(string input)
    {
        var trackingNumber = TrackingNumber.Create(input);

        trackingNumber.Value.Should().Be(input.ToUpperInvariant());
    }

    [Theory]
    [InlineData("ABC-123")]
    [InlineData("VTX-202513-0001")] // invalid month
    [InlineData("VTX-202501-ABCD")]
    [InlineData("")]
    public void TrackingNumber_Create_WithInvalidValue_Throws(string input)
    {
        var act = () => TrackingNumber.Create(input);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Weight_FromDecimal_WithNegativeValue_Throws()
    {
        var act = () => Weight.FromDecimal(-1);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Volume_FromDecimal_WithZero_Throws()
    {
        var act = () => Volume.FromDecimal(0);

        act.Should().Throw<DomainValidationException>();
    }
}
