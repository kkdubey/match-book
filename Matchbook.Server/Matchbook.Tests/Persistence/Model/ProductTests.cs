using Matchbook.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static Matchbook.Tests.Persistence.EntityAssertions;

namespace Matchbook.Tests.Persistence.Model
{
    public class ProductTests : PersistenceTestBase
    {
        private readonly DbContextFixture fixture;

        public ProductTests(DbContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCrudProduct()
        {
            VerifyCrudOperationsOn(
                fixture,
                DataGenerator.NewProduct(),
                x => x.Id,
                p => p.Year = 2054,
                p => p.Year == 2054);
        }

        [Fact]
        public void SymbolShouldBeUnique()
        {
            var existingProduct = GivenASavedProduct();

            using var dbContext = fixture.NewDbContext();
            var newProduct = DataGenerator.NewProduct();
            newProduct.Symbol = existingProduct.Symbol;
            dbContext.Products.Add(newProduct);

            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        [Fact]
        public void SpecificationIsRequired()
        {
            var productWithoutSpec = DataGenerator.NewProduct();
            productWithoutSpec.Specification = null;

            using var dbContext = fixture.NewDbContext();
            dbContext.Products.Add(productWithoutSpec);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        [Fact]
        public void DeletingAProductShouldNotDeleteSpec()
        {
            var product = GivenASavedProduct();

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var spec = dbContext.ProductSpecifications.Find(product.SpecificationId);
                Assert.NotNull(spec);
            }
        }

        [Fact]
        public void CannotDeleteProductSpecIfReferencedByProduct()
        {
            var product = GivenASavedProduct();
            using var dbContext = fixture.NewDbContext();
            var specs = dbContext.ProductSpecifications.Find(product.SpecificationId);
            dbContext.ProductSpecifications.Remove(specs);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        private Product GivenASavedProduct()
        {
            using var dbContext = fixture.NewDbContext();
            var product = DataGenerator.NewProduct();
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
            return product;
        }
    }
}
