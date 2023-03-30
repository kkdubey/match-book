using Matchbook.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace Matchbook.Tests.Persistence
{
    public class DbContextFixture : IDisposable
    {
        private int isInitializedFlag = 0;

        private SqliteConnection dbConnection;
        private DbContextOptions<MatchbookDbContext> dbContextOptions;
        
        public MatchbookDbContext NewDbContext()
        {
            EnsureInitialized();
            return new MatchbookDbContext(dbContextOptions);
        }

        private void EnsureInitialized()
        {
            if (Interlocked.CompareExchange(ref isInitializedFlag, 1, 0) == 1)
                return;

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            dbConnection = new SqliteConnection("DataSource=:memory:");
            dbConnection.Open();

            dbContextOptions = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseSqlite(
                    dbConnection, 
                    actions => actions.MigrationsAssembly(typeof(MatchbookDbContext).Assembly.FullName))
                .Options;

            using (var dbContext = new MatchbookDbContextWithData(dbContextOptions))
                dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            dbConnection?.Close();
        }
    }
}
