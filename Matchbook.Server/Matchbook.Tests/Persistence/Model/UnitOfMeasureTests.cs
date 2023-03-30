using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class UnitOfMeasureTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public UnitOfMeasureTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudUnitsOfMeasure()
        {
            VerifyCrudOperationsOn(
                fixture,
                new UnitOfMeasure { Code = "U", Name = "N" },
                x => x.Code,
                x => x.Name = "new name",
                x => x.Name == "new name");
        }

        [Fact]
        public void CannotCreateCurrencyWithExistingCode()
        {
            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.UnitsOfMeasure.Add(new UnitOfMeasure { Code = "XXX", Name = "Some" });
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.UnitsOfMeasure.Add(new UnitOfMeasure { Code = "XXX", Name = "Some other" });
                Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
            }
        }
    }
}
