using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common.Extensions;
using Meekys.OData.Attributes;

namespace Meekys.OData.Expressions
{
    public static class OrderByExpressionBuilder
    {
        public static List<OrderByItem> Build<T>(string orderBy)
        {
            var builder = new OrderByExpressionBuilder<T>();

            return builder.BuildExpressions(orderBy);
        }

        public static List<OrderByItem> BuildDefault<T>()
        {
            var builder = new OrderByExpressionBuilder<T>();

            return builder.BuildDefaultExpressions();
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic implementation of same class")]
    public class OrderByExpressionBuilder<T>
    {
        private const string KeyAttributeImplementationMessage = "You must explicity implement a key with System.ComponentModel.DataAnnotations.KeyAttribute on one or more properties.";

        public List<OrderByItem> BuildExpressions(string orderBy)
        {
            if (orderBy == null)
                throw new ArgumentNullException(nameof(orderBy));

            var expressions = orderBy.Split(',')
                .Select(f => f.Trim())
                .Where(f => f.Length > 0)
                .Select(BuildExpression)
                .ToList();

            return expressions;
        }

        private OrderByItem BuildExpression(string fieldExpression)
        {
            var parts = fieldExpression.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var field = parts.First();

            var param = Expression.Parameter(typeof(T), "item");
            var property = Expression.PropertyOrField(param, field);
            var func = Expression.Lambda(property, param);

            var desc = parts.Length > 1 && parts[1].StartsWith("desc", StringComparison.OrdinalIgnoreCase);

            return new OrderByItem
            {
                Expression = func,
                Descending = desc
            };
        }

        public List<OrderByItem> BuildDefaultExpressions()
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
            return typeof(T).GetTypeInfo().GetProperties()
                .Where(p => p.HasCustomAttribute<KeyAttribute>())
                .ToList();
        }

        private List<PropertyInfo> GetDefaultPrimaryKeyProperty()
        {
            var keys = typeof(T).GetTypeInfo().GetProperties()
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