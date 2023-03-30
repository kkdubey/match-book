using Bogus;
using Bogus.Extensions;
using Matchbook.Db;
using Matchbook.Model;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Matchbook.Tests.Persistence
{
    public static class DataGenerator
    {
        static DataGenerator()
        {
            Randomizer.Seed = new Random(DateTime.Now.Millisecond);
        }

        public static string[] KnownCurrencies = new[] { "USD", "EUR", "CHF" };
        public static string[] KnownUnitsOfMeasure = new[] { "BU", "MT", "ST" };

        public static SubAccount NewSubAccount()
        {
            return new Faker<SubAccount>()
                .RuleFor(x => x.Name, f => f.Finance.Account(10))
                .RuleFor(x => x.AccountOwner, f => f.Finance.Account(10))
                .Generate();
        }

        public static SubAccount NewUnsavedSubAccount(MatchbookDbContext dbContext)
        {
            var clearingAccount = NewClearingAccount();
            var internalClearingAccount = NewClearingAccount();
            dbContext.ClearingAccounts.AddRange(clearingAccount, internalClearingAccount);
            dbContext.SaveChanges();

            var account = NewSubAccount();
            account.ClearingAccountId = clearingAccount.Id;
            account.InternalClearingAccountId = internalClearingAccount.Id;
            return account;
        }
        
        public static SubAccount NewSavedSubAccount(MatchbookDbContext dbContext)
        {
            var account = NewUnsavedSubAccount(dbContext);
            dbContext.SubAccounts.Add(account);
            dbContext.SaveChanges();
            return account;
        }

        public static IEnumerable<SubAccount> NewSubAccounts()
        {
            return new Faker<SubAccount>()
                .RuleFor(x => x.Name, f => f.Finance.Account(10))
                .RuleFor(x => x.AccountOwner, f => f.Finance.Account(10))
                .RuleFor(x => x.ClearingAccount, _ => NewClearingAccounts().First())
                .RuleFor(x => x.InternalClearingAccount, NewClearingAccounts().First())
                .GenerateForever();
        }

        public static ProductSpecification NewProductSpec()
        {
            return new Faker<ProductSpecification>()
                .RuleFor(x => x.Name, f => f.Random.AlphaNumeric(20))
                .RuleFor(x => x.ExchangeName, f => f.Random.AlphaNumeric(4).ToUpper())
                .RuleFor(x => x.CommodityCode, f => f.Random.AlphaNumeric(2).ToUpper())
                .RuleFor(x => x.JPMCode, f => f.Random.AlphaNumeric(5).ToUpper())
                .RuleFor(x => x.ContractUoM, f => f.PickRandom(KnownUnitsOfMeasure))
                .RuleFor(x => x.PriceQuoteCurrency, f => f.PickRandom(KnownCurrencies))
                .Generate();
        }

        public static ProductSpecification NewSavedProductSpec(MatchbookDbContext dbContext)
        {
            var spec = NewProductSpec();
            dbContext.ProductSpecifications.Add(spec);
            dbContext.SaveChanges();
            return spec;
        }

        public static Product NewProduct()
        {
            return new Faker<Product>()
                .RuleFor(x => x.Month, f => f.Random.Number(1, 12))
                .RuleFor(x => x.Year, f => f.Random.Number(2019, 2028))
                .RuleFor(x => x.Specification, _ => NewProductSpec())
                .RuleFor(x => x.Symbol, (f, current) => $"{current.Specification.CommodityCode}{f.Random.String2(1)}{f.Random.Number(1, 12)}")
                .Generate();
        }

        public static IEnumerable<ClearingAccount> NewClearingAccounts()
        {
            return new Faker<ClearingAccount>()
                .RuleFor(x => x.Code, f => f.Finance.Account(5))
                .RuleFor(x => x.Description, f => f.Finance.AccountName())
                .RuleFor(x => x.Type, f => f.Random.Enum<ClearingAccountType>())
                .RuleFor(x => x.BrokerCode, f => f.Random.String(5))
                .RuleFor(x => x.BrokerTerm, f => f.Random.String(2).ToUpper())
                .GenerateForever();
        }

        public static ClearingAccount NewClearingAccount() => NewClearingAccounts().First();

        public static IEnumerable<Order> NewOrders()
        {
            return new Faker<Order>()
                .RuleFor(x => x.Side, f => f.PickRandom<Side>())
                .RuleFor(x => x.Product, _ => NewProduct())
                .RuleFor(x => x.ProductSymbol, (f, current) => current.Product.Symbol)
                .RuleFor(x => x.Quantity, f => f.Random.Int(100, 500))
                // Price will be null half of the time
                .RuleFor(x => x.Price, f => f.Random.Decimal(0, 50).OrNull(f, 0.5f))
                // If price is null, set the price instructions
                .RuleFor(x => x.PriceInstruction, (f, current) => current.Price == null ? f.PickRandom<PriceInstruction>() : default)
                .RuleFor(x => x.SubAccount, f => NewSubAccounts().First())
                .RuleFor(x => x.CounterpartySubAccount, f => NewSubAccounts().First())
                .RuleFor(x => x.OrderType, f => f.PickRandom<OrderType>())
                .RuleFor(x => x.OrderStatus, f => f.PickRandom<OrderStatus>())
                .GenerateForever();
        }

        public static Order NewOrder() => NewOrders().First();
    }
}
