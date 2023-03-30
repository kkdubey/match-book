using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Matchbook.Tests
{
    public static class LinqExpressionExtensions
    {
        public static void AssignValue<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, TProperty newValue)
        {
            var memberExpression = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpression.Member;

            property.SetValue(entity, newValue);
        }

        public static TValue GetValue<TEntity, TValue>(TEntity entity, Expression<Func<TEntity, TValue>> propertySelector)
        {
            var memberExpression = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpression.Member;

            return (TValue)property.GetValue(entity);
        }
    }
}
