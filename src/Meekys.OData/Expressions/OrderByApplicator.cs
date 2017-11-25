using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common.Extensions;
using Meekys.Common.Helpers;

namespace Meekys.OData.Expressions
{
    public static class OrderByApplicator
    {
        public static IOrderedQueryable<T> Apply<T>(IQueryable<T> query, IEnumerable<OrderByItem> orderBy)
        {
            return (IOrderedQueryable<T>)orderBy
                .Aggregate(query, (q, x) => DynamicOrderBy(q, x));
        }

        private static IOrderedQueryable<T> DynamicOrderBy<T>(IQueryable<T> query, OrderByItem orderBy)
        {
            var func = MethodHelpers.GetMethodInfo(DynamicOrderBy<T, object>, query, orderBy)
                .GetGenericMethodDefinition()
                .MakeGenericMethod(typeof(T), orderBy.Expression.PropertyInfo().PropertyType);

            return (IOrderedQueryable<T>)func.Invoke(null, new object[] { query, orderBy });
        }

        private static IOrderedQueryable<T> DynamicOrderBy<T, TKey>(IQueryable<T> query, OrderByItem orderBy)
        {
            var keySelector = (Expression<Func<T, TKey>>)orderBy.Expression;

            var orderedQuery = query as IOrderedQueryable<T>;

            if (orderedQuery != null && query.Expression.Type != typeof(IQueryable<T>))
            {
                return orderBy.Descending
                    ? orderedQuery.ThenByDescending(keySelector)
                    : orderedQuery.ThenBy(keySelector);
            }

            return orderBy.Descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }
    }
}