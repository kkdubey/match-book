using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class CurrencyTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public CurrencyTests(DbContextFixture fixture) 
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudCurrencies()
        {
            VerifyCrudOperationsOn(fixture,
                new Currency { Code = "C", Name = "N" },
                x => x.Code,
                x => x.Name = "new name",
                x => x.Name == "new name");
        }

        [Fact]
        public void CannotCreateCurrencyWithExistingCode()
        {
            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Currencies.Add(new Currency { Code = "XXX", Name = "Some" });
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Currencies.Add(new Currency { Code = "XXX", Name = "Some other" });
                Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
            }
        }
    }
}
