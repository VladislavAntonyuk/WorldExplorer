namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using Xunit;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;