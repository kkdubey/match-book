using Matchbook.Model;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class UserTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public UserTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudUser()
        {
            VerifyCrudOperationsOn(
                fixture,
                new User { Name = "n", DsId = "ds" },
                x => x.Id,
                x => x.Name = "new name",
                x => x.Name == "new name");
        }
    }
}
