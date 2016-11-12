using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Meekys.Common.Extensions;
using Meekys.OData.Expressions;

using NSubstitute;

using Xunit;

namespace Meekys.OData.Tests.Expressions
{
    public class OrderByApplicatorBuilderTests
    {
        private Expression<Func<TestType, int>> IntValueExpression = item => item.IntValue;
        private Expression<Func<TestType, decimal>> DecValueExpression = item => item.DecValue;
        private Expression<Func<TestType, string>> StringValueExpression = item => item.StringValue;

        [Fact]
        public void Test_Apply_Single()
        {
            // Arrange
            var orderBy = new[]
            {
                new OrderByItem { Expression = IntValueExpression }
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
                new OrderByItem { Expression = IntValueExpression, Descending = true }
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
                new OrderByItem { Expression = IntValueExpression },
                new OrderByItem { Expression = StringValueExpression },
                new OrderByItem { Expression = DecValueExpression }
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
                new OrderByItem { Expression = IntValueExpression },
                new OrderByItem { Expression = StringValueExpression, Descending = true },
                new OrderByItem { Expression = DecValueExpression }
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