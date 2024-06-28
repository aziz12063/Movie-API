using Xunit;

namespace ApiApplication.IntegrationTests.FixtureClassesFirIntegration
{
    [CollectionDefinition("DB collection integration")]
    public class FixtureCollectionForIntegration : ICollectionFixture<DbFixtureIntegration>
    {
    }
}
