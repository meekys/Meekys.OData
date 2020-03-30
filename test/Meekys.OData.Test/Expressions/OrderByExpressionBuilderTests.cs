using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Meekys.OData.Expressions;

using Xunit;

namespace Bills.Tests.Expressions
{
    public class OrderByExpressionBuilderTests
    {
        [Fact]
        public void Test_SingleField()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue").ToList();

            // Assert
            Assert.Single(results);

            var result = results.First();
            Assert.False(result.Descending);
            Assert.Equal("item => item.IntValue", result.Expression.ToString());
        }

        [Fact]
        public void Test_SingleField_Descending()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue desc").ToList();

            // Assert
            Assert.Single(results);

            var result = results.First();
            Assert.True(result.Descending);
            Assert.Equal("item => item.IntValue", result.Expression.ToString());
        }

        [Fact]
        public void Test_MultipleFields()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue, StringValue, DecValue").ToList();

            // Assert
            Assert.Equal(3, results.Count);

            Assert.False(results[0].Descending);
            Assert.Equal("item => item.IntValue", results[0].Expression.ToString());

            Assert.False(results[1].Descending);
            Assert.Equal("item => item.StringValue", results[1].Expression.ToString());

            Assert.False(results[2].Descending);
            Assert.Equal("item => item.DecValue", results[2].Expression.ToString());
        }

        [Fact]
        public void Test_MultipleFields_Descending()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue desc, StringValue desc, DecValue desc").ToList();

            // Assert
            Assert.Equal(3, results.Count);

            Assert.True(results[0].Descending);
            Assert.Equal("item => item.IntValue", results[0].Expression.ToString());

            Assert.True(results[1].Descending);
            Assert.Equal("item => item.StringValue", results[1].Expression.ToString());

            Assert.True(results[2].Descending);
            Assert.Equal("item => item.DecValue", results[2].Expression.ToString());
        }

        [Fact]
        public void Test_MultipleFields_Mixed()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue, StringValue desc, DecValue").ToList();

            // Assert
            Assert.Equal(3, results.Count);

            Assert.False(results[0].Descending);
            Assert.Equal("item => item.IntValue", results[0].Expression.ToString());

            Assert.True(results[1].Descending);
            Assert.Equal("item => item.StringValue", results[1].Expression.ToString());

            Assert.False(results[2].Descending);
            Assert.Equal("item => item.DecValue", results[2].Expression.ToString());
        }

        [Fact]
        public void Test_Accepted_By_OrderByApplicator_Apply()
        {
            // Act
            var results = OrderByExpressionBuilder.Build<TestType>("IntValue, StringValue desc, DecValue").ToList();
            var query = new List<TestType>().AsQueryable();

            // Assert
            query = OrderByApplicator.Apply(query, results);
            Assert.True(true);
        }

        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1812:InternalClassNeverInstantiated", Justification = "Type referenced for testing")]
        private class TestType
        {
            public string StringValue { get; set; }

            public int IntValue { get; set; }

            public decimal DecValue { get; set; }

            public double DblValue { get; set; }

            public bool BoolValue { get; set; }
        }
    }
}