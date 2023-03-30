using Matchbook.Db;
using Matchbook.Model;
using Matchbook.WebHost.Controllers;
using Matchbook.WebHost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Matchbook.WebHost.Tests
{
    public class OrderLinkingControllerTests
    {
        private OrderLinkingController controller;
        private DbContextOptions<MatchbookDbContext> _options;

        [Fact]
        public void CreateOrderLink_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            _options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderLinkingTestInvalidRequestDatabase")
                .Options;
            using var dbContext = new MatchbookDbContext(_options);
            controller = new OrderLinkingController(dbContext);
            var request = new OrderLinkRequest
            {
                OrderIds = null
            };

            // Act
            var result = controller.CreateOrderLink(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task CreateOrderLink_InvalidOrderIds_ReturnsBadRequest()
        {
            // Arrange
            _options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderLinkingTestInvalidOrderIdsDatabase")
                .Options;
            using (var dbContext = new MatchbookDbContext(_options))
            {
                var request = new OrderLinkRequest
                {
                    OrderIds = new List<long> { 1, 2, 3 }
                };
                dbContext.Orders.AddRange(new List<Order>
            {
                new Order { Id = 1 },
                new Order { Id = 2 }
            });
                await dbContext.SaveChangesAsync();
                controller = new OrderLinkingController(dbContext);

                // Act
                var result = controller.CreateOrderLink(request);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task CreateOrderLink_OrdersWithDifferentSymbols_ReturnsBadRequest()
        {
            // Arrange
            _options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderLinkingTestOrdersWithDifferentSymbolsDatabase")
                .Options;
            using var dbContext = new MatchbookDbContext(_options);
            controller = new OrderLinkingController(dbContext);
            var request = new OrderLinkRequest
            {
                OrderIds = new List<long> { 1, 2 }
            };
            dbContext.Orders.AddRange(new List<Order>
            {
                new Order { Id = 1, ProductSymbol = "AAPL", SubAccountId = 1 },
                new Order { Id = 2, ProductSymbol = "GOOG", SubAccountId = 1 }
            });
            await dbContext.SaveChangesAsync();

            // Act
            var result = controller.CreateOrderLink(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrderLink_OrdersAlreadyLinked_ReturnsBadRequest()
        {
            // Arrange
            _options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderLinkingTestOrdersAlreadyLinkedDatabase")
                .Options;
            using var dbContext = new MatchbookDbContext(_options);
            var request = new OrderLinkRequest
            {
                OrderIds = new List<long> { 1, 2 }
            };
            dbContext.Orders.AddRange(new List<Order>
            {
                new Order { Id = 1, ProductSymbol = "AAPL", SubAccountId = 1, LinkId = 1 },
                new Order { Id = 2, ProductSymbol = "AAPL", SubAccountId = 1, LinkId = 1 }
            });
            await dbContext.SaveChangesAsync();
            dbContext.OrderLinks.Add(new OrderLink { Id = 1 });
            await dbContext.SaveChangesAsync();
            controller = new OrderLinkingController(dbContext);

            // Act
            var result = controller.CreateOrderLink(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrderLink_LinkNameAlreadyInUse_ReturnsBadRequest()
        {
            // Arrange
            _options = new DbContextOptionsBuilder<MatchbookDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderLinkingTestLinkNameAlreadyInUseDatabase")
                .Options;
            using var dbContext = new MatchbookDbContext(_options);

            // Seed the database with test data
            var orders = new[] {
                new Order { Id = 1, ProductSymbol = "AAPL", SubAccountId = 1 },
                new Order { Id = 2, ProductSymbol = "AAPL", SubAccountId = 2 },
                new Order { Id = 3, ProductSymbol = "AAPL", SubAccountId = 3 },
            };
            await dbContext.Orders.AddRangeAsync(orders);
            await dbContext.SaveChangesAsync();

            var linkName = "Test Link";
            var existingLink = new OrderLink { Id = 1, Name = linkName };
            await dbContext.OrderLinks.AddAsync(existingLink);
            await dbContext.SaveChangesAsync();

            var orderLinkRequest = new OrderLinkRequest
            {
                LinkName = linkName,
                OrderIds = new List<long> { 1, 2, 3 }
            };
            controller = new OrderLinkingController(dbContext);
            //var controller = new OrderLinkingController(dbContext);

            // Act
            var result = controller.CreateOrderLink(orderLinkRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("Link name is already in use", badRequestResult.Value);
        }
    }
}
