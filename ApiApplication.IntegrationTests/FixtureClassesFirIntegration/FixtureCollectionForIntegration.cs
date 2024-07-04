using Xunit;

namespace ApiApplication.IntegrationTests.FixtureClassesFirIntegration
{
    [CollectionDefinition("DB collection")]
    public class FixtureCollectionForIntegration : ICollectionFixture<DbFixtureIntegration>
    {
    }
}
