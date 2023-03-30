using Matchbook.Db;
using Matchbook.Model;
using Matchbook.WebHost.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Matchbook.WebHost.Tests
{
    public class OrdersControllerTests
    {
        private readonly MatchbookDbContext dbContext;

        public OrdersControllerTests()
        {
            // Setup DbContext for testing
            var options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrdersControllerTestDb")
                .Options;

            dbContext = new MatchbookDbContext(options);
        }

        [Fact]
        public async Task Get_ReturnsOrderSummaries()
        {
            // Arrange
            List<Order> orders = GetOrders();

            dbContext.Orders.AddRange(orders);
            await dbContext.SaveChangesAsync();

            var expectedOrderSummaries = orders.Select(o => new OrderSummary
            {
                Id = o.Id,
                ProductSymbol = o.ProductSymbol,
                Price = o.Price,
                Currency = o.Product.Specification.PriceQuoteCurrency,
                Quantity = o.Quantity,
                UnitOfMeasure = o.Product.Specification.ContractUoM,
                SubAccount = o.SubAccountId
            }).ToList();

            var ordersController = new OrdersController(dbContext);

            // Act
            var result = await ordersController.Get();

            // Assert
            var orderSummaries = Assert.IsAssignableFrom<List<OrderSummary>>(result.Value);
            Assert.Equal(expectedOrderSummaries.Count(), orderSummaries.Count());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static List<Order> GetOrders()
        {
            return new List<Order>() {
                new Order()
                {
                    Id = 1,
                    ProductSymbol = "ABC",
                    Price = 10.0m,
                    Quantity = 5,
                    SubAccountId = 1,
                    Product = new Product
                    {
                        Id = 1,
                        Symbol = "Test",
                        Specification = new ProductSpecification
                        {
                            PriceQuoteCurrency = "USD",
                            ContractUoM = "barrels"
                        }
                    }
                },
                new Order()
                {
                    Id = 2,
                    ProductSymbol = "DEF",
                    Price = 20.0m,
                    Quantity = 10,
                    SubAccountId = 2,
                    Product = new Product
                    {
                        Id = 2,
                        Symbol = "Test2",
                        Specification = new ProductSpecification
                        {
                            PriceQuoteCurrency = "GBP",
                            ContractUoM = "ounces"
                        }
                    }
                }
            };
        }
    }
}
