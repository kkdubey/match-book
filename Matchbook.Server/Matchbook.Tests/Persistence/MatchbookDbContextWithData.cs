using Matchbook.Db;
using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Matchbook.Tests.Persistence
{
    public class MatchbookDbContextWithData : MatchbookDbContext
    {
        public MatchbookDbContextWithData(DbContextOptions<MatchbookDbContext> options) 
            : base(options)
        {
        }

        // Seeding data for test cases
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Currency>()
                .HasData(
                    from code in DataGenerator.KnownCurrencies
                    select new Currency { Code = code, Name = code} );
            modelBuilder.Entity<UnitOfMeasure>()
                .HasData(
                    from code in DataGenerator.KnownUnitsOfMeasure
                    select new UnitOfMeasure { Code = code, Name = code } );
        }
    }
}
