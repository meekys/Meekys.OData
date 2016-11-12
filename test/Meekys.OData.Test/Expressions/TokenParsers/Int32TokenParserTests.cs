using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class Int32TokenParserTests
    {
        private Int32TokenParser _parser = new Int32TokenParser();
        
        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("2147483647", 2147483647)]
        [InlineData("-2147483648", -2147483648)]
        public void Test_Int32(string token, int expected)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }
        
        [Theory]
        [InlineData("Invalid")]
        [InlineData("2147483648")]
        [InlineData("-2147483649")]
        public void Test_Passthrough(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.Equal(null, result);
        }
    }
}