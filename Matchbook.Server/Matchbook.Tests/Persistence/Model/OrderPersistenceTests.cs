using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class OrderPersistenceTests : IClassFixture<DbContextFixture>
    {
        private readonly DbContextFixture fixture;

        public OrderPersistenceTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCreateOrder()
        {
            Product product;
            SubAccount subAccount;
            SubAccount counterpartySubAccount;

            using (var dbContext = fixture.NewDbContext())
            {
                subAccount = DataGenerator.NewSavedSubAccount(dbContext);
                counterpartySubAccount = DataGenerator.NewSavedSubAccount(dbContext);

                product = DataGenerator.NewProduct();
                dbContext.Products.Add(product);
                dbContext.SaveChanges();
            }

            var order = new Order
            {
                ProductSymbol = product.Symbol,
                SubAccountId = subAccount.Id,
                CounterpartySubAccountId = counterpartySubAccount.Id,
            };

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Orders.Add(order);
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public void CanCreateWithAllDependencies()
        {
            var order = DataGenerator.NewOrder();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Orders.Add(order);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var loadedOrder = dbContext.Orders.Find(order.Id);

                Assert.NotNull(loadedOrder);
                Assert.False(loadedOrder.ProductSymbol.IsNullOrEmpty());
                Assert.True(loadedOrder.SubAccountId > 0);
                Assert.True(loadedOrder.CounterpartySubAccountId > 0);
            }
        }

        [Fact]
        public void CannotRemoveAProductWhenItIsReferencesByAnOrder()
        {
            var order = GivenASavedOrder();

            ThenRelatedEntityCannotBeDeleted<Product>(fixture, order.Product.Id);
        }

        [Fact]
        public void CannotRemoveASubAccountWhenItIsReferencesByAnOrder()
        {
            var order = GivenASavedOrder();

            ThenRelatedEntityCannotBeDeleted<SubAccount>(fixture, order.SubAccountId);
        }

        [Fact]
        public void CannotRemoveACounterpartySubAccountWhenItIsReferencesByAnOrder()
        {
            var order = GivenASavedOrder();

            ThenRelatedEntityCannotBeDeleted<SubAccount>(fixture, order.CounterpartySubAccountId);
        }

        [Fact]
        public void DeletingAnOrderWillNotDeleteRelatedEntities()
        {
            var order = GivenASavedOrder();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Orders.Remove(order);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                Assert.Null(dbContext.Orders.Find(order.Id));
                Assert.NotNull(dbContext.Products.First(p => p.Symbol == order.ProductSymbol));
                Assert.NotNull(dbContext.SubAccounts.Find(order.SubAccountId));
                Assert.NotNull(dbContext.SubAccounts.Find(order.CounterpartySubAccountId));
            }
        }

        [Fact]
        public void CannotCreateWithoutProduct()
        {
            var order = DataGenerator.NewOrder();
            order.ProductSymbol = null;
            order.Product = null;

            using var dbContext = fixture.NewDbContext();
            dbContext.Orders.Add(order);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        [Fact]
        public void EnumsAreSavedAsStrings()
        {
            var order = GivenASavedOrder();

            var enumProperties = new[]
                {
                    nameof(Order.Side),
                    nameof(Order.PriceInstruction),
                    nameof(Order.OrderStatus),
                    nameof(Order.OrderType),
                }
                .JoinString(",");

            using var dbContext = fixture.NewDbContext();
            using var reader = dbContext.Database.ExecuteSqlQuery($"select {enumProperties} from Orders as o where o.Id = ({order.Id})");

            Assert.True(reader.Read());     // We only expect 1 response
            Assert.Equal(order.Side.ToString(), reader.DbDataReader[nameof(Order.Side)]);
            Assert.Equal(order.PriceInstruction.ToString(), reader.DbDataReader[nameof(Order.PriceInstruction)]);
            Assert.Equal(order.OrderStatus.ToString(), reader.DbDataReader[nameof(Order.OrderStatus)]);
            Assert.Equal(order.OrderType.ToString(), reader.DbDataReader[nameof(Order.OrderType)]);
        }

        [Fact]
        public void UnitOfMeasureForPhysicalContractIsLinkedToReferenceTable()
        {
            var order = DataGenerator.NewOrder();
            order.PhysicalContract = new PhysicalContract
            {
                Quantity = 1,
                Number = "12345",
                UnitOfMeasure = "UKNOWN"
            };

            using var dbContext = fixture.NewDbContext();
            dbContext.Orders.Add(order);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        private Order GivenASavedOrder()
        {
            var order = DataGenerator.NewOrder();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Orders.Add(order);
                dbContext.SaveChanges();
            }
            return order;
        }
    }
}
