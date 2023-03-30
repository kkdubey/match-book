using Matchbook.Db;
using Matchbook.Model;
using Matchbook.Tests.Persistence;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matchbook.WebHost
{
    public static class DatabaseInitializer
    {
        public static void Initialize(MatchbookDbContext dbContext)
        {
            // Drop & re-create DB
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            // Data is already seeded?
            //if (dbContext.Orders.Any())
            //    return;

            // Do the initialization

            DataGenerator.KnownCurrencies
                .Select(code => new Currency { Code = code, Name = $"{code} currency" })
                .ForEach(currency => dbContext.Currencies.Add(currency));
            dbContext.SaveChanges();
            DataGenerator.KnownUnitsOfMeasure
                .Select(code => new UnitOfMeasure { Code = code, Name = $"{code} uom" })
                .ForEach(uom => dbContext.UnitsOfMeasure.Add(uom));
            dbContext.SaveChanges();

            DataGenerator.NewOrders()
                .Take(20)
                .ForEach(order => dbContext.Orders.Add(order));
            dbContext.SaveChanges();
        }
    }
}
