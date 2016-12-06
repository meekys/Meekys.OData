using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Meekys.Common.Helpers;
using Meekys.OData.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Meekys.OData.Mvc
{
    public class QueryableActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var objectResult = context.Result as ObjectResult;
            if (objectResult == null)
                return;
                
            var query = objectResult.Value as IQueryable;
            if (query == null)
                return;

            var queryContext = new QueryContext(context.HttpContext.Request);
            
            objectResult.Value = ApplyFilter(query, queryContext);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        
        private IQueryable ApplyFilter(IQueryable query, QueryContext context)
        {
            var method = MethodHelpers.GetMethodInfo(ApplyFilter<object>, default(IQueryable<object>), context)
                .GetGenericMethodDefinition()
                .MakeGenericMethod(query.ElementType);
                
            return (IQueryable)method.Invoke(this, new object[] { query, context });
        }

        private IQueryable<T> ApplyFilter<T>(IQueryable<T> query, QueryContext context)
        {
            if (!string.IsNullOrEmpty(context.Filter))
                query = query.Where(context.Filter);
                
            if (!string.IsNullOrEmpty(context.OrderBy))
                query = query.OrderBy(context.OrderBy);
            else if (context.Skip > 0 || context.Top > 0)
                query = query.OrderByDefault();

            if (context.Skip > 0)
                query = query.Skip(context.Skip);
                
            if (context.Top > 0)
                query = query.Take(context.Top);
                
            return query;
        }
        
        private class QueryContext
        {
            public string Filter;
            public string OrderBy;
            public int Skip;
            public int Top;
           
            public QueryContext(HttpRequest context)
            {
                Filter = context.HttpContext.Request.Query["$filter"];
                OrderBy = context.HttpContext.Request.Query["$orderby"];
                Int32.TryParse(context.HttpContext.Request.Query["$skip"], out Skip);
                Int32.TryParse(context.HttpContext.Request.Query["$top"], out Top);
            }
        }
    }
}