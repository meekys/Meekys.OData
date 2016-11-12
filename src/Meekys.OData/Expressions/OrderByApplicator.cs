using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

using Meekys.Common.Extensions;
using Meekys.Common.Helpers;

namespace Meekys.OData.Expressions
{
    public class OrderByApplicator
    {
        public static IQueryable<T> Apply<T>(IQueryable<T> query, IEnumerable<OrderByItem> orderBy)
        {
            return OrderByApplicator<T>.Apply(query, orderBy);
        }
    }
    
    public class OrderByApplicator<T>
    {
        public static IOrderedQueryable<T> Apply(IQueryable<T> query, IEnumerable<OrderByItem> orderBy)
        {
            return (IOrderedQueryable<T>)orderBy
                .Aggregate(query, (q, x) => DynamicOrderBy(q, x));
        }
        
        private static IOrderedQueryable<T> DynamicOrderBy(IQueryable<T> query, OrderByItem orderBy)
        {
            var func = MethodHelpers.GetMethodInfo(DynamicOrderBy<object>, query, orderBy)
                .GetGenericMethodDefinition()
                .MakeGenericMethod(orderBy.Expression.PropertyInfo().PropertyType);
                
            return (IOrderedQueryable<T>)func.Invoke(null, new object[] { query, orderBy });
        }
        
        private static IOrderedQueryable<T> DynamicOrderBy<TKey>(IQueryable<T> query, OrderByItem orderBy)
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