using Xunit;

namespace ShipmentTracking.Integration.Tests.Infrastructure;

/// <summary>
/// Shared collection definition for integration tests that reuse the same <see cref="IntegrationTestFactory"/>.
/// </summary>
[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory>
{
    /// <summary>
    /// Collection name for use in test attributes.
    /// </summary>
    public const string Name = "Integration";
}
