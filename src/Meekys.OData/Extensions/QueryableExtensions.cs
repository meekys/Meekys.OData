using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions;

namespace Meekys.OData.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string filter)
        {
            var expression = FilterExpressionBuilder.Build<T>(filter);

            return query.Where(expression);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderBy)
        {
            var expressions = OrderByExpressionBuilder.Build<T>(orderBy);

            return OrderByApplicator.Apply(query, expressions);
        }

        public static IQueryable<T> OrderByDefault<T>(this IQueryable<T> query)
        {
            var expressions = OrderByExpressionBuilder.BuildDefault<T>();

            return OrderByApplicator.Apply(query, expressions);
        }
    }
}