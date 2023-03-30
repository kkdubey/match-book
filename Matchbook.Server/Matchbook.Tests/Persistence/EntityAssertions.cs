using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Matchbook.Tests.Persistence
{
    public static class EntityAssertions
    {
        public static void ThenRelatedEntityCannotBeDeleted<TEntity>(DbContextFixture fixture, object key) where TEntity : class
        {
            using var dbContext = fixture.NewDbContext();

            var entity = dbContext.Set<TEntity>().Find(key);
            Assert.NotNull(entity);

            dbContext.Set<TEntity>().Remove(entity);
            Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
        }

        public static void PropertyIsRequired<TEntity>(DbContextFixture fixture, TEntity entity, Expression<Func<TEntity, object>> propertySelector) where TEntity : class
        {
            LinqExpressionExtensions.AssignValue(entity, propertySelector, null);

            using (var dbContext = fixture.NewDbContext())
            {
                dbContext.Set<TEntity>().Add(entity);
                var exception = Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges());
                Assert.Contains("NOT NULL", exception.ToString());
            }
        }

        public static void VerifyCrudOperationsOn<T, TKey>(DbContextFixture fixture, T newValue, Func<T, TKey> getKey, Action<T> update, Func<T, bool> assertEqual) where T : class, new()
        {
            TKey key;

            using (var dbContext = fixture.NewDbContext())
            {
                var entityEntry = dbContext.Set<T>().Add(newValue);
                dbContext.SaveChanges();
                key = getKey(entityEntry.Entity);
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var entity = dbContext.Find<T>(key);
                Assert.NotNull(entity);

                update(entity);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var entity = dbContext.Find<T>(key);
                Assert.True(assertEqual(entity));
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var entity = dbContext.Find<T>(key);
                dbContext.Set<T>().Remove(entity);
                dbContext.SaveChanges();
            }

            using (var dbContext = fixture.NewDbContext())
            {
                var entity = dbContext.Find<T>(key);
                Assert.Null(entity);
            }
        }
    }
}
