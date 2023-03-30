using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public partial class SubAccountTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public SubAccountTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudSubAccounts()
        {
            VerifyCrudOperationsOn(
                fixture,
                GivenUnsavedSubAccount(),
                x => x.Id,
                x => x.AccountOwner = "new owner",
                x => x.AccountOwner == "new owner");
        }

        [Fact]
        public void SubAccountNameIsUnique()
        {
            var account = GivenUnsavedSubAccount();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.SubAccounts.Add(account);
                dbContext.SaveChanges();
            }
            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.SubAccounts.Add(new SubAccount { Name = account.Name, AccountOwner = "WTG" });
                Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
            }
        }

        [Fact]
        public void CreatingSubAccountWithClearingAccounts()
        {
            var (id1, id2) = Given2ClearingAccounts();
            
            var subAccountId = WhenSavingSubAccountWithClearingAccounts(id1, id2);

            ThenSubAccountHasCorrectClearingAccounts(subAccountId, id1, id2);
        }

        [Fact]
        public void DeletingSubAccountWillNotDeleteClearingAccounts()
        {
            var (accId, ca1, ca2) = GivenSubAccountWithClearingAccounts();

            WhenDeletingSubAccount(accId);

            ThenSubAccountIsDeletedButClearingAccountsAreNot(accId, ca1, ca2);
        }

        [Fact]
        public void DeletingOfClearingAccountIsNotPossibleIfAccountIsInUse()
        {
            var (_, ca1, ca2) = GivenSubAccountWithClearingAccounts();

            ThenClearingAccountCannotBeDeleted(ca1);
            ThenClearingAccountCannotBeDeleted(ca2);
        }
    }
}
