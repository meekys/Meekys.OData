using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

using Meekys.Common.Extensions;

namespace Meekys.OData.Expressions
{
    public class OrderByExpressionBuilder<T>
    {
        private const string KeyAttributeImplementationMessage = "You must explicity implement a key with System.ComponentModel.DataAnnotations.KeyAttribute on one or more properties.";

        public static List<OrderByItem> Build(string orderBy)
        {
            var builder = new OrderByExpressionBuilder<T>();
            
            return builder.BuildExpressions(orderBy);
        }
        
        public static List<OrderByItem> BuildDefault()
        {
            var builder = new OrderByExpressionBuilder<T>();
            
            return builder.BuildDefaultExpressions();
        }
        
        private List<OrderByItem> BuildExpressions(string orderBy)
        {
            var expressions = orderBy.Split(',')
                .Select(f => f.Trim())
                .Where(f => f.Length > 0)
                .Select(BuildExpression)
                .ToList();
                
            return expressions;
        }
        
        private OrderByItem BuildExpression(string fieldExpression)
        {
            var parts = fieldExpression.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
            var field = parts.First();
            
            var param = Expression.Parameter(typeof(T), "item");
            var property = Expression.PropertyOrField(param, field);
            var func = Expression.Lambda(property, param);
            
            var desc = parts.Length > 1 && parts[1].ToLowerInvariant().StartsWith("desc");
            
            return new OrderByItem
            {
                Expression = func,
                Descending = desc
            };
        }
                
        private List<OrderByItem> BuildDefaultExpressions()
        {
            var keyProperties = GetDefinedKeyProperties()
                ?? GetDefaultPrimaryKeyProperty();
                
            var expressions = keyProperties
                .Select(p => BuildExpression(p.Name))
                .ToList();
                
            return expressions;
        }
        
        private List<PropertyInfo> GetDefinedKeyProperties()
        {
            return typeof(T).GetProperties()
                .Where(p => p.HasCustomAttribute<KeyAttribute>())
                .ToList();
        }
        
        private List<PropertyInfo> GetDefaultPrimaryKeyProperty()
        {
            var keys = typeof(T).GetProperties()
                .Where(p => p.Name.In("Id", "Key"))
                .ToList();
            
            if (keys.Count == 1)
                return keys;
                
            if (keys.Count > 1)
                throw new InvalidOperationException($"Ambiguous key detected. Both Key and Id exist.\n{KeyAttributeImplementationMessage}");
                
            throw new InvalidOperationException($"No automatic key can be determined.\n{KeyAttributeImplementationMessage}");
        }
    }
}