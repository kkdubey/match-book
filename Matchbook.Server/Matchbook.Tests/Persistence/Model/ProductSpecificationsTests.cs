using System;
using System.Linq.Expressions;
using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class ProductSpecificationsTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public ProductSpecificationsTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudProductSpecifications()
        {
            var newProductSpec = new ProductSpecification
            {
                Name = "Soft Red Winter Wheat",
                ExchangeName = "XCBT",
                CommodityCode = "C",
                JPMCode = "C SRW",
                ContractSize = 5000,
                ContractUoM = "BU",
                PriceQuoteCurrency = "USD",
                MaturityMonths = new[] { 1, 3, 6, 9 }
            };

            VerifyCrudOperationsOn(
                fixture,
                newProductSpec,
                s => s.Id,
                spec => spec.JPMCode = "new jpm code",
                spec => spec.JPMCode == "new jpm code");
        }

        [Fact]
        public void CannotCreateWithUnknownCurrency()
        {
            var productSpec = DataGenerator.NewProductSpec();
            productSpec.PriceQuoteCurrency = "XYZ";

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.ProductSpecifications.Add(productSpec);
                var exception = Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
                Assert.Contains("FOREIGN KEY", exception.ToString());
            }
        }

        [Fact]
        public void CannotCreateWithUnknownUnitOfMeasure()
        {
            var productSpec = DataGenerator.NewProductSpec();
            productSpec.ContractUoM = "XYZ";

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.ProductSpecifications.Add(productSpec);
                var exception = Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
                Assert.Contains("FOREIGN KEY", exception.ToString());
            }
        }

        [Fact]
        public void CommodityCodeShouldBeUnique()
        {
            var productSpec1 = DataGenerator.NewProductSpec();
            var productSpec2 = DataGenerator.NewProductSpec();
            productSpec2.CommodityCode = productSpec1.CommodityCode;

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.ProductSpecifications.Add(productSpec1);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.ProductSpecifications.Add(productSpec2);
                var exception = Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
                Assert.Contains("UNIQUE", exception.ToString());
            }
        }

        public static TheoryData<Expression<Func<ProductSpecification, object>>> requiredPropertySelectors =
            new TheoryData<Expression<Func<ProductSpecification, object>>>
        {
            { x => x.CommodityCode },
            { x => x.JPMCode },
            { x => x.Name },
            { x => x.ExchangeName },
            { x => x.ContractUoM },
            { x => x.PriceQuoteCurrency },
        };

        [Theory]
        [MemberData(nameof(requiredPropertySelectors))]
        public void SavingWithoutRequiredPropertiesFails(Expression<Func<ProductSpecification, object>> propertySelector)
        {
            var productSpec = DataGenerator.NewProductSpec();

            PropertyIsRequired(fixture, productSpec, propertySelector);
        }
    }
}
