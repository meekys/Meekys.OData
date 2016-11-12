using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class NullTokenParserTests
    {
        private NullTokenParser _parser = new NullTokenParser();
        
        [Theory]
        [InlineData("null")]
        [InlineData("NULL")]
        [InlineData("Null")]
        public void Test_Null(string token)
        {
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(null, (result as ConstantExpression).Value);
        }
        
        [Theory]
        [InlineData("null'object'")]
        [InlineData("NULL'object'")]
        [InlineData("NulL'object'")]
        public void Test_Typed_Null(string token)
        {
            // Act
            var result = Assert.Throws<NotImplementedException>(() => (object)_parser.Parse(token));
            
            // Assert
            Assert.Equal("Explicitly typed null constants not supported", result.Message);
        }
        
        [Theory]
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