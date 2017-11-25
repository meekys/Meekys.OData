using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common;
using Meekys.Common.Extensions;

namespace Meekys.OData.Expressions
{
    public static class FunctionExpressionBuilder
    {
        public static IFunctionSource FunctionBuilder { get; set; }

        public static Expression Build(string name, Expression[] arguments)
        {
            var method = GetFunctionMethod(name, arguments.Select(a => a.Type).ToArray());

            if (method == null)
                throw new InvalidOperationException($"Unmatched function {name}({arguments.Select(a => a.Type.Name).ToCsv()})");

            return BuildPropertyFunctionExpression(method, arguments)
                ?? BuildStaticMethodExpression(method, arguments)
                ?? BuildMemberMethodExpression(method, arguments)
                ?? BuildUnknownMemberExpression(method, arguments);
        }

        private static MemberInfo GetFunctionMethod(string functionName, Type[] argumentTypes)
        {
            var builder = FunctionBuilder ?? new DefaultFunctionSource();

            return builder.Find(functionName, argumentTypes);
        }

        private static Expression BuildPropertyFunctionExpression(MemberInfo member, Expression[] arguments)
        {
            var property = member as PropertyInfo;

            if (property == null)
                return null;

            return Expression.Property(arguments[0], property);
        }

        private static Expression BuildStaticMethodExpression(MemberInfo member, Expression[] arguments)
        {
            var method = member as MethodInfo;

            if (method == null || !method.IsStatic)
                return null;

            arguments = arguments.Zip(method.GetParameters(), (a, p) => CastExpressionBuilder.Build(p.ParameterType, a))
                .ToArray();

            return Expression.Call(null, method, arguments);
        }

        private static Expression BuildMemberMethodExpression(MemberInfo member, Expression[] arguments)
        {
            var method = member as MethodInfo;

            if (method == null || method.IsStatic)
                return null;

            var instance = arguments.First();

            arguments = arguments.Skip(1).Zip(method.GetParameters(), (a, p) => new { Argument = a, CastTo = p.ParameterType })
                .Select(x => CastExpressionBuilder.Build(x.CastTo, x.Argument))
                .ToArray();

            return Expression.Call(instance, method, arguments);
        }

        private static Expression BuildUnknownMemberExpression(MemberInfo member, Expression[] arguments)
        {
            var argumentTypes = arguments.Select(a => a.Type).ToList();

            throw new NotImplementedException($"Functions member of type {member.GetType()}({argumentTypes}) is not supported");
        }
    }
}