using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common.Extensions;
using Meekys.OData.Expressions.TokenParsers;

namespace Meekys.OData.Expressions
{
    public static class FilterExpressionBuilder
    {
        public static Expression<Func<T, bool>> Build<T>(string filter)
        {
            var builder = new FilterExpressionBuilder<T>(filter);

            return builder.BuildBoolExpression();
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic implementation of same class")]
    public class FilterExpressionBuilder<T> : IDisposable
    {
        private const string NoToken = "{No Token}";

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> LogicalOrMap =
            new Dictionary<string, Func<Expression, Expression, BinaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "or", Expression.OrElse }
        };

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> LogicalAndMap =
            new Dictionary<string, Func<Expression, Expression, BinaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "and", Expression.AndAlso }
        };

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> LogicalComparisonMap =
            new Dictionary<string, Func<Expression, Expression, BinaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "eq", Expression.Equal },
            { "ne", Expression.NotEqual },
            { "gt", Expression.GreaterThan },
            { "ge", Expression.GreaterThanOrEqual },
            { "lt", Expression.LessThan },
            { "le", Expression.LessThanOrEqual }
        };

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> AdditiveMap =
            new Dictionary<string, Func<Expression, Expression, BinaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "add", Expression.Add },
            { "sub", Expression.Subtract }
        };

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> MultiplicativeMap =
            new Dictionary<string, Func<Expression, Expression, BinaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "div", Expression.Divide },
            { "mul", Expression.Multiply }
        };

        private static readonly Dictionary<string, Func<Expression, UnaryExpression>> UnaryMap =
            new Dictionary<string, Func<Expression, UnaryExpression>>(StringComparer.OrdinalIgnoreCase)
        {
            { "not", Expression.Not }
        };

        private static readonly Type[] CastTypes = new[]
        {
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(long),
            typeof(int),
            typeof(short),
            typeof(byte),
            typeof(bool)
        };

        private static readonly Type[] ConstantParsers = new[]
        {
            typeof(NullTokenParser),
            typeof(StringTokenParser),
            typeof(DateTimeTokenParser),
            //// typeof(TimeTokenParser),
            //// typeof(DateTimeOffsetTokenParser),
            typeof(BooleanTokenParser),
            typeof(DoubleTokenParser),
            typeof(FloatTokenParser),
            typeof(DecimalTokenParser),
            typeof(ByteTokenParser),
            //// typeof(SByteTokenParser),
            typeof(Int16TokenParser),
            typeof(Int32TokenParser),
            typeof(Int64TokenParser),
            typeof(GuidTokenParser),
            typeof(BinaryTokenParser)
        };

        private readonly int _maxDepth;

        private BufferedEnumerator<FilterToken> _tokens;

        private ParameterExpression _parameter;
        private List<ITokenParser> _constantParsers;
        private bool _disposed;

        public int MaxDepth
        {
            get { return _maxDepth; }
        }

        public int RecursionDepth { get; set; }

        public FilterExpressionBuilder(string filter)
        {
            _maxDepth = 100;
            InitConstantParsers();

            var tokenizer = new FilterTokeniser(filter);

            _tokens = new BufferedEnumerator<FilterToken>(tokenizer.Tokens.GetEnumerator(), 1);
            _parameter = Expression.Parameter(typeof(T), "item");

            if (!_tokens.MoveNext())
                throw new ArgumentException("Expected expression", nameof(filter));
        }

        public Expression<Func<T, bool>> BuildBoolExpression()
        {
            var expression = BuildExpression();

            if (_tokens.Current != null)
                throw new FilterExpressionException($"Unexpected token: {_tokens.Current}");

            if (expression.Type != typeof(bool))
                throw new FilterExpressionException("Expected boolean expression");

            return Expression.Lambda<Func<T, bool>>(expression, _parameter);
        }

        private Expression BuildExpression()
        {
            using (RecurseIn())
            {
                return BuildLogicalOr();
            }
        }

        private bool CurrentOperator(Dictionary<string, Func<Expression, Expression, BinaryExpression>> map, out Func<Expression, Expression, BinaryExpression> op)
        {
            op = null;

            if (_tokens.Current == null)
                return false;

            return map.TryGetValue(_tokens.Current.Token, out op);
        }

        private bool CurrentOperator(Dictionary<string, Func<Expression, UnaryExpression>> map, out Func<Expression, UnaryExpression> op)
        {
            op = null;

            if (_tokens.Current == null)
                return false;

            return map.TryGetValue(_tokens.Current.Token, out op);
        }

        private bool CheckToken(string expected)
        {
            var found = _tokens.Current == expected;

            if (found)
                _tokens.MoveNext();

            return found;
        }

        private void ExpectToken()
        {
            if (!_tokens.MoveNext())
                throw new FilterExpressionException($"Expected token after {(_tokens.HasPrevious() ? _tokens.Previous().ToString() : NoToken)}");
        }

        private void ExpectToken(string expected)
        {
            if (_tokens.Current != expected)
            {
                if (_tokens.Current != null)
                    throw new FilterExpressionException($"Expected {expected} but got {_tokens.Current}");
                else if (_tokens.HasPrevious())
                    throw new FilterExpressionException($"Expected {expected} but got {NoToken} after {_tokens.Previous()}");
                else
                    throw new FilterExpressionException($"Expected {expected} but got {NoToken}");
            }

            _tokens.MoveNext();
        }

        private void CastParameters(ref Expression left, ref Expression right)
        {
            var leftType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
            var rightType = Nullable.GetUnderlyingType(right.Type) ?? right.Type;

            if (!leftType.In(CastTypes) || !rightType.In(CastTypes))
                return;

            if (TryCastParameters<double?>(ref left, ref right))
                return;
            if (TryCastParameters<double>(ref left, ref right))
                return;
            if (TryCastParameters<float?>(ref left, ref right))
                return;
            if (TryCastParameters<float>(ref left, ref right))
                return;
            if (TryCastParameters<decimal?>(ref left, ref right))
                return;
            if (TryCastParameters<decimal>(ref left, ref right))
                return;
            if (TryCastParameters<long?>(ref left, ref right))
                return;
            if (TryCastParameters<long>(ref left, ref right))
                return;
            if (TryCastParameters<int?>(ref left, ref right))
                return;
            if (TryCastParameters<int>(ref left, ref right))
                return;
            if (TryCastParameters<short?, int?>(ref left, ref right))
                return;
            if (TryCastParameters<short, int>(ref left, ref right))
                return;
            if (TryCastParameters<byte?, int?>(ref left, ref right))
                return;
            if (TryCastParameters<byte, int>(ref left, ref right))
                return;
            if (TryCastParameters<bool?>(ref left, ref right))
                return;
            if (TryCastParameters<bool>(ref left, ref right))
                return;
        }

        private bool TryCastParameters<TParam>(ref Expression left, ref Expression right)
        {
            return TryCastParameters<TParam, TParam>(ref left, ref right);
        }

        private bool TryCastParameters<TCastFrom, TCastTo>(ref Expression left, ref Expression right)
        {
            if (left.Type != typeof(TCastFrom) && right.Type != typeof(TCastFrom))
                return false;

            if (right.Type != typeof(TCastTo))
                right = CastExpressionBuilder.Build<TCastTo>(right);

            if (left.Type != typeof(TCastTo))
                left = CastExpressionBuilder.Build<TCastTo>(left);

            return true;
        }

        private Expression BuildBinaryExpression(Func<Expression> leftOrRightFunc, Dictionary<string, Func<Expression, Expression, BinaryExpression>> operatorMap)
        {
            using (RecurseIn())
            {
                var left = leftOrRightFunc();

                Func<Expression, Expression, BinaryExpression> op;

                while (CurrentOperator(operatorMap, out op))
                {
                    ExpectToken();

                    var right = leftOrRightFunc();

                    CastParameters(ref left, ref right);

                    left = op(left, right);
                }

                return left;
            }
        }

        private Expression BuildUnaryExpression(Func<Expression> rightFunc, Dictionary<string, Func<Expression, UnaryExpression>> operatorMap)
        {
            using (RecurseIn())
            {
                Func<Expression, UnaryExpression> op;

                if (CurrentOperator(operatorMap, out op))
                {
                    ExpectToken();

                    var right = BuildUnaryExpression(rightFunc, operatorMap);

                    return op(right);
                }

                return rightFunc();
            }
        }

        private Expression BuildLogicalOr()
        {
            return BuildBinaryExpression(BuildLogicalAnd, LogicalOrMap);
        }

        private Expression BuildLogicalAnd()
        {
            return BuildBinaryExpression(BuildComparison, LogicalAndMap);
        }

        private Expression BuildComparison()
        {
            return BuildBinaryExpression(BuildAdditive, LogicalComparisonMap);
        }

        private Expression BuildAdditive()
        {
            return BuildBinaryExpression(BuildMultiplicative, AdditiveMap);
        }

        private Expression BuildMultiplicative()
        {
            return BuildBinaryExpression(BuildUnary, MultiplicativeMap);
        }

        private Expression BuildUnary()
        {
            return BuildUnaryExpression(BuildPrimaryExpression, UnaryMap);
        }

        private Expression BuildPrimaryExpression()
        {
            using (RecurseIn())
            {
                return BuildParenExpression()
                    ?? BuildFunctionExpression()
                    ?? BuildMemberExpression()
                    ?? BuildConstantExpression()
                    ?? BuildUnknownToken();
            }
        }

        private Expression BuildParenExpression()
        {
            if (!CheckToken(ExpressionConstants.OpenParen))
                return null;

            var expression = BuildExpression();

            ExpectToken(ExpressionConstants.CloseParen);

            return expression;
        }

        private Expression BuildFunctionExpression()
        {
            FilterToken peekNext;
            if (!_tokens.TryPeek(out peekNext) || peekNext != ExpressionConstants.OpenParen)
                return null;

            var functionName = _tokens.Current;

            _tokens.MoveNext();

            var arguments = BuildFunctionArguments().ToArray();

            ExpectToken(ExpressionConstants.CloseParen);

            return FunctionExpressionBuilder.Build(functionName, arguments);
        }

        private IEnumerable<Expression> BuildFunctionArguments()
        {
            ExpectToken(ExpressionConstants.OpenParen);

            if (_tokens.Current == ExpressionConstants.CloseParen)
                yield break;

            do
            {
                yield return BuildExpression();
            }
            while (CheckToken(ExpressionConstants.Comma));
        }

        private Expression BuildMemberExpression()
        {
            if (_tokens.Current == null)
                return null;

            string propertyName = _tokens.Current;
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null)
                return null;

            _tokens.MoveNext();

            return Expression.Property(_parameter, property);
        }

        private Expression BuildConstantExpression()
        {
            var expression = _constantParsers
                .Select(x => x.Parse(_tokens.Current))
                .FirstOrDefault(x => x != null);

            if (expression != null)
                _tokens.MoveNext();

            return expression;
        }

        private void InitConstantParsers()
        {
            if (_constantParsers != null)
                return;

            _constantParsers = ConstantParsers
                .Select(x => (ITokenParser)Activator.CreateInstance(x))
                .ToList();
        }

        private Expression BuildUnknownToken()
        {
            throw new FilterExpressionException($"Unrecognised token: {_tokens.Current}");
        }

        private Recurse RecurseIn()
        {
            return new Recurse(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _tokens.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private class Recurse : IDisposable
        {
            private FilterExpressionBuilder<T> _owner;

            public Recurse(FilterExpressionBuilder<T> owner)
            {
                _owner = owner;

                _owner.RecursionDepth++;
                if (_owner.RecursionDepth > _owner.MaxDepth)
                    throw new InvalidOperationException("Expression depth is too deep");
            }

            public void Dispose()
            {
                _owner.RecursionDepth--;
            }
        }
    }
}