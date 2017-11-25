using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Meekys.Common.Extensions;
using Meekys.OData.Expressions;

using NSubstitute;

using Xunit;

namespace Meekys.OData.Tests.Expressions
{
    public class OrderByApplicatorTests
    {
        private Expression<Func<TestType, int>> _intValueExpression = item => item.IntValue;
        private Expression<Func<TestType, decimal>> _decValueExpression = item => item.DecValue;
        private Expression<Func<TestType, string>> _stringValueExpression = item => item.StringValue;

        [Fact]
        public void Test_Apply_Single()
        {
            // Arrange
            var orderBy = new[]
            {
                new OrderByItem { Expression = _intValueExpression }
            };

            var query = FakeQuery<TestType>();

            // Act
            var result = OrderByApplicator.Apply(query, orderBy);

            // Assert
            AssertProviderCalls(query, "OrderBy");
        }

        [Fact]
        public void Test_Apply_Single_Descending()
        {
           // Arrange
            var orderBy = new[]
            {
                new OrderByItem { Expression = _intValueExpression, Descending = true }
            };

            var query = FakeQuery<TestType>();

            // Act
            var result = OrderByApplicator.Apply(query, orderBy);

            // Assert
            AssertProviderCalls(query, "OrderByDescending");
        }

        [Fact]
        public void Test_Apply_Multiple()
        {
            // Arrange
            var orderBy = new[]
            {
                new OrderByItem { Expression = _intValueExpression },
                new OrderByItem { Expression = _stringValueExpression },
                new OrderByItem { Expression = _decValueExpression }
            };

            var query = FakeQuery<TestType>();

            // Act
            var result = OrderByApplicator.Apply(query, orderBy);

            // Assert
            AssertProviderCalls(query, "OrderBy", "ThenBy", "ThenBy");
        }

        [Fact]
        public void Test_Apply_Multiple_Mixed()
        {
            // Arrange
            var orderBy = new[]
            {
                new OrderByItem { Expression = _intValueExpression },
                new OrderByItem { Expression = _stringValueExpression, Descending = true },
                new OrderByItem { Expression = _decValueExpression }
            };

            var query = FakeQuery<TestType>();

            // Act
            var result = OrderByApplicator.Apply(query, orderBy);

            // Assert
            AssertProviderCalls(query, "OrderBy", "ThenByDescending", "ThenBy");
        }

        private void AssertProviderCalls<T>(IQueryable<T> query, params string[] expectedCalls)
        {
            var receivedCalls = query.Provider.ReceivedCalls()
                .Select(c => c.GetArguments().First() as MethodCallExpression)
                .Select(m => m.Method.Name)
                .ToList();

            Assert.Equal(expectedCalls, receivedCalls);
        }

        private IQueryable<T> FakeQuery<T>()
        {
            var list = new List<T>();
            var enumerableQuery = new EnumerableQuery<T>(list);
            var expression = Expression.Constant(enumerableQuery);

            var query = Substitute.For<IQueryable<T>>();
            query.Expression.Returns(expression);

            var orderedQuery = Substitute.For<IOrderedQueryable<T>>();
            orderedQuery.Expression.Returns(expression);

            var provider = Substitute.For<IQueryProvider>();
            provider.CreateQuery(Arg.Any<Expression>()).Returns(orderedQuery);
            provider.CreateQuery<T>(Arg.Any<Expression>()).Returns(orderedQuery);

            query.Provider.Returns(provider);
            orderedQuery.Provider.Returns(provider);

            return query;
        }

        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1034:DoNotNestType", Justification = "Needs to be public for mocking purposes")]
        public class TestType
        {
            public string StringValue { get; set; }

            public int IntValue { get; set; }

            public decimal DecValue { get; set; }

            public double DblValue { get; set; }

            public bool BoolValue { get; set; }
        }
    }
}