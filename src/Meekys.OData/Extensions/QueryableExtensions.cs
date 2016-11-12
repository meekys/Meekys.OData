using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Meekys.OData.Expressions;

namespace Meekys.OData.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string filter)
        {
            var expression = FilterExpressionBuilder<T>.Build(filter);
            
            return query.Where(expression);
        }
        
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderBy)
        {
            var expressions = OrderByExpressionBuilder<T>.Build(orderBy);
            
            return OrderByApplicator.Apply(query, expressions);
        }
 
         public static IQueryable<T> OrderByDefault<T>(this IQueryable<T> query)
        {
            var expressions = OrderByExpressionBuilder<T>.BuildDefault();
            
            return OrderByApplicator.Apply(query, expressions);
        }
    }
}