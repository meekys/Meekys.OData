using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class BinaryTokenParserTests
    {
        private BinaryTokenParser _parser = new BinaryTokenParser();
        
        [Theory]
        [InlineData("binary'deadbeef'")]
        [InlineData("binary'DEADBEEF'")]
        [InlineData("binary'DeadbeeF'")]
        [InlineData("X'deadbeef'")]
        [InlineData("X'DEADBEEF'")]
        [InlineData("X'DeadbeeF'")]
        public void Test_Binary(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(new byte[] { 0xde, 0xad, 0xbe, 0xef }, (result as ConstantExpression).Value);
        }
        
        [Theory]
        [InlineData("binary'Invalid'")]
        [InlineData("X'Invalid'")]
        [InlineData("X'Deadbee'")]
        public void Test_Invalid_Binary(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));
            
            // Assert
            Assert.StartsWith("Unable to parse binary token: ", result.Message);
        }
        
        [Theory]
        [InlineData("Invalid")]
        [InlineData("BINARY'Invalid'")]
        [InlineData("x'Invalid'")]
        public void Test_Passthrough(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.Equal(null, result);
        }
    }
}