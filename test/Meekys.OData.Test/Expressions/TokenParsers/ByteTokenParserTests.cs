using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class ByteTokenParserTests
    {
        private ByteTokenParser _parser = new ByteTokenParser();
        
        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("254", 254)]
        [InlineData("255", 255)]
        public void Test_Byte(string token, byte expected)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }
        
        [Theory]
        [InlineData("-1")]
        [InlineData("256")]
        [InlineData("Invalid")]
        public void Test_Passthrough(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.Equal(null, result);
        }
    }
}