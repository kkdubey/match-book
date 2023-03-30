using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Matchbook.Tests.Persistence.Model
{
    public partial class SubAccountTests
    {
        private (int id1, int id2) Given2ClearingAccounts()
        {
            var clearingAccount1 = DataGenerator.NewClearingAccount();
            var clearingAccount2 = DataGenerator.NewClearingAccount();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.ClearingAccounts.AddRange(clearingAccount1, clearingAccount2);
                dbContext.SaveChanges();
            }

            return (clearingAccount1.Id, clearingAccount2.Id);
        }

        private SubAccount GivenUnsavedSubAccount()
        {
            using (var dbContext = fixture.NewDbContext())
                return DataGenerator.NewUnsavedSubAccount(dbContext);
        }

        private int WhenSavingSubAccountWithClearingAccounts(int clearingAccount1, int clearingAccount2)
        {
            using (var dbContext = fixture.NewDbContext())
            {
                var subAccount = GivenUnsavedSubAccount();
                subAccount.ClearingAccountId = clearingAccount1;
                subAccount.InternalClearingAccountId = clearingAccount2;

                dbContext.SubAccounts.Add(subAccount);
                dbContext.SaveChanges();

                return subAccount.Id;
            }
        }

        private void ThenSubAccountHasCorrectClearingAccounts(int subAccountId, int id1, int id2)
        {
            using (var dbContext = fixture.NewDbContext())
            {
                var subAccount = dbContext.SubAccounts
                    .Include(x => x.ClearingAccount)
                    .Include(x => x.InternalClearingAccount)
                    .SingleOrDefault(sa => sa.Id == subAccountId);
                Assert.NotNull(subAccount);

                Assert.NotNull(subAccount.ClearingAccount);
                Assert.Equal(id1, subAccount.ClearingAccount.Id);
                Assert.NotNull(subAccount.InternalClearingAccount);
                Assert.Equal(id2, subAccount.InternalClearingAccount.Id);
            }
        }

        private void ThenSubAccountIsDeletedButClearingAccountsAreNot(int accId, int ca1, int ca2)
        {
            using (var dbContext = fixture.NewDbContext())
            {
                var subAccount = dbContext.SubAccounts.Find(accId);

                Assert.Null(subAccount);

                var clearingAccountIds = dbContext.ClearingAccounts.Select(ca => ca.Id).ToArray();
                Assert.Contains(ca1, clearingAccountIds);
                Assert.Contains(ca2, clearingAccountIds);
            }
        }

        private void WhenDeletingSubAccount(int accId)
        {
            using (var dbContext = fixture.NewDbContext())
            {
                var subAccount = dbContext.SubAccounts.Find(accId);

                dbContext.SubAccounts.Remove(subAccount);
                dbContext.SaveChanges();
            }
        }

        private (int subAccount, int clearingAccount1, int clearingAccount2) GivenSubAccountWithClearingAccounts()
        {
            var (ca1, ca2) = Given2ClearingAccounts();

            var subAccountId = WhenSavingSubAccountWithClearingAccounts(ca1, ca2);

            return (subAccountId, ca1, ca2);
        }

        private void ThenClearingAccountCannotBeDeleted(int clearingAccountId)
        {
            using var dbContext = fixture.NewDbContext();

            var clearingAccount = dbContext.ClearingAccounts.Find(clearingAccountId);
            dbContext.ClearingAccounts.Remove(clearingAccount);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }
    }
}
