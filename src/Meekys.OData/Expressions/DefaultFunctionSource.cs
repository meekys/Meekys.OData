using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common.Helpers;

namespace Meekys.OData.Expressions
{
    public class DefaultFunctionSource : IFunctionSource
    {
        protected enum MatchResult
        {
            NoMatch,
            Exact,
            CanCast
        }

        private readonly Dictionary<Type, Type[]> _implicitCastMap = new Dictionary<Type, Type[]>
        {
            { typeof(double),  new[] { typeof(sbyte), typeof(byte),   typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(char), typeof(float), typeof(ulong) } },
            { typeof(float),   new[] { typeof(sbyte), typeof(byte),   typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(char), typeof(ulong) } },
            { typeof(decimal), new[] { typeof(sbyte), typeof(byte),   typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(char), typeof(ulong) } },
            { typeof(ulong),   new[] { typeof(byte),  typeof(ushort), typeof(uint),  typeof(char) } },
            { typeof(long),    new[] { typeof(sbyte), typeof(byte),   typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
            { typeof(uint),    new[] { typeof(byte),  typeof(ushort), typeof(char) } },
            { typeof(int),     new[] { typeof(sbyte), typeof(byte),   typeof(short), typeof(ushort), typeof(char) } },
            { typeof(ushort),  new[] { typeof(byte),  typeof(char) } },
            { typeof(short),   new[] { typeof(sbyte), typeof(byte) } }
        };

        private readonly Dictionary<string, MemberInfo[]> _functionMap;

        [SuppressMessage(
            "Microsoft.CodeAnalysis.Analyzers",
            "CA1304:BehaviorCouldVaryBasedOnCurrentUsersLocaleSettings",
            Justification = "Functions ToLower and ToUpper are interpreted by Query Provider")]
        [SuppressMessage(
            "Microsoft.CodeAnalysis.Analyzers",
            "CA1307:BehaviorCouldVaryBasedOnCurrentUsersLocaleSettings",
            Justification = "Functions EndsWith, StartsWith and IndexOf are interpreted by Query Provider")]
        public DefaultFunctionSource()
        {
            string refString = string.Empty;
            DateTime refDate = default(DateTime);
            decimal refDecimal = 0m;
            double refDouble = 0.0;

            _functionMap = new Dictionary<string, MemberInfo[]>(StringComparer.OrdinalIgnoreCase)
            {
                // String functions
                // { "substringof", },
                { "endswith",   new[] { MethodHelpers.GetMethodInfo(() => refString.EndsWith(refString)) } },
                { "startswith", new[] { MethodHelpers.GetMethodInfo(() => refString.StartsWith(refString)) } },
                { "length",     new[] { MethodHelpers.GetPropertyInfo(() => refString.Length) } },
                { "indexof",    new[] { MethodHelpers.GetMethodInfo(() => refString.IndexOf(refString)) } },
                { "replace",    new[] { MethodHelpers.GetMethodInfo(() => refString.Replace(refString, refString)) } },
                {
                    "substring",
                    new[]
                    {
                        MethodHelpers.GetMethodInfo(() => refString.Substring(0)),
                        MethodHelpers.GetMethodInfo(() => refString.Substring(0, 0))
                    }
                },
                { "tolower", new[] { MethodHelpers.GetMethodInfo(() => refString.ToLower()) } },
                { "toupper", new[] { MethodHelpers.GetMethodInfo(() => refString.ToUpper()) } },
                { "trim",    new[] { MethodHelpers.GetMethodInfo(() => refString.Trim()) } },
                { "concat",  new[] { MethodHelpers.GetMethodInfo(string.Concat, refString, refString) } },

                // Date functions
                { "day",    new[] { MethodHelpers.GetPropertyInfo(() => refDate.Day) } },
                { "hour",   new[] { MethodHelpers.GetPropertyInfo(() => refDate.Hour) } },
                { "minute", new[] { MethodHelpers.GetPropertyInfo(() => refDate.Minute) } },
                { "month",  new[] { MethodHelpers.GetPropertyInfo(() => refDate.Month) } },
                { "second", new[] { MethodHelpers.GetPropertyInfo(() => refDate.Second) } },
                { "year",   new[] { MethodHelpers.GetPropertyInfo(() => refDate.Year) } },

                // Math functions
                {
                    "round",
                    new[]
                    {
                        MethodHelpers.GetMethodInfo(() => Math.Round(refDecimal)),
                        MethodHelpers.GetMethodInfo(() => Math.Round(refDouble))
                    }
                },
                {
                    "floor",
                    new[]
                    {
                        MethodHelpers.GetMethodInfo(() => Math.Floor(refDecimal)),
                        MethodHelpers.GetMethodInfo(() => Math.Floor(refDouble))
                    }
                },
                {
                    "ceiling",
                    new[]
                    {
                        MethodHelpers.GetMethodInfo(() => Math.Ceiling(refDecimal)),
                        MethodHelpers.GetMethodInfo(() => Math.Ceiling(refDouble))
                    }
                }
            };
        }

        public virtual MemberInfo Find(string name, Type[] paramTypes)
        {
            MemberInfo[] methods;
            if (!_functionMap.TryGetValue(name, out methods))
                return null;

            var method = methods
                .Select(m => new { MemberInfo = m, Weight = GetMemberWeight(m, paramTypes) })
                .Where(x => x.Weight >= 0)
                .OrderBy(x => x.Weight)
                .Select(x => x.MemberInfo)
                .FirstOrDefault();

            return method;
        }

        protected virtual int GetMemberWeight(MemberInfo memberInfo, Type[] paramTypes)
        {
            if (memberInfo is MethodInfo)
                return GetMethodWeight(memberInfo as MethodInfo, paramTypes);

            if (memberInfo is PropertyInfo)
                return GetPropertyWeight(memberInfo as PropertyInfo, paramTypes);

            throw new NotSupportedException($"memberInfo of type {memberInfo.GetType()} is not supported");
        }

        protected int GetPropertyWeight(PropertyInfo propertyInfo, Type[] paramTypes)
        {
            if (paramTypes.Length != 1)
                return -1;

            var result = MatchParameter(propertyInfo.DeclaringType, paramTypes[0]);

            switch (result)
            {
                case MatchResult.NoMatch:
                    return -1;
                case MatchResult.Exact:
                    return 0;
                default:
                    return 1;
            }
        }

        protected int GetMethodWeight(MethodInfo methodInfo, Type[] paramTypes)
        {
            if (methodInfo.IsStatic)
                return GetStaticMethodWeight(methodInfo, paramTypes);

            return GetMemberMethodWeight(methodInfo, paramTypes);
        }

        protected int GetStaticMethodWeight(MethodInfo methodInfo, Type[] paramTypes)
        {
            var methodParams = methodInfo.GetParameters();

            if (methodParams.Length != paramTypes.Length)
                return -1;

            return MatchParameters(methodParams.Select(p => p.ParameterType).ToArray(), paramTypes);
        }

        protected int GetMemberMethodWeight(MethodInfo methodInfo, Type[] paramTypes)
        {
            var methodParams = methodInfo.GetParameters();

            if (paramTypes.Length == 0 || methodParams.Length != paramTypes.Length - 1)
                return -1;

            var methodTypes = new[] { methodInfo.DeclaringType }
                .Union(methodParams.Select(p => p.ParameterType))
                .ToArray();

            return MatchParameters(methodTypes, paramTypes);
        }

        protected int MatchParameters(Type[] methodTypes, Type[] paramTypes)
        {
            var matches = methodTypes.Zip(paramTypes, MatchParameter).ToList();

            if (matches.Any(m => m == MatchResult.NoMatch))
                return -1;

            return matches.Count(m => m == MatchResult.CanCast);
        }

        protected MatchResult MatchParameter(Type methodType, Type paramType)
        {
            if (methodType == paramType)
                return MatchResult.Exact;

            if (CanCastTo(paramType, methodType))
                return MatchResult.CanCast;

            return MatchResult.NoMatch;
        }

        protected bool CanCastTo(Type fromType, Type toType)
        {
            Type[] fromTypes;

            return _implicitCastMap.TryGetValue(toType, out fromTypes) && fromTypes.Contains(fromType);
        }
    }
}