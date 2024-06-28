using Xunit;

namespace SharedFixtureTest
{
    [CollectionDefinition("SharedDB")]
    public class FixtureCollectionShared : ICollectionFixture<DbFixtureShared>
    {
    }
}
