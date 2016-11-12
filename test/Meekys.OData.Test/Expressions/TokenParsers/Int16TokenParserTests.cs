using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class Int16TokenParserTests
    {
        private Int16TokenParser _parser = new Int16TokenParser();
        
        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("32767", 32767)]
        [InlineData("-32768", -32768)]
        public void Test_Int16(string token, short expected)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }
        
        [Theory]
        [InlineData("Invalid")]
        [InlineData("32768")]
        [InlineData("-32769")]
        public void Test_Passthrough(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.Equal(null, result);
        }
    }
}