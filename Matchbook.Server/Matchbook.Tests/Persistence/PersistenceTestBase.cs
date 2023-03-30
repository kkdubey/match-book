using Xunit;

namespace Matchbook.Tests.Persistence
{
    [CollectionDefinition("Persistence Tests")]
    public class PersistenceFixtureCollection : ICollectionFixture<DbContextFixture> { }

    [Collection("Persistence Tests")]
    public abstract class PersistenceTestBase { }
}
