using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class FloatTokenParserTests
    {
        private FloatTokenParser _parser = new FloatTokenParser();
        
        [Theory]
        [InlineData("0F", 0f)]
        [InlineData("1F", 1f)]
        [InlineData("1f", 1f)]
        [InlineData("1.1F", 1.1f)]
        [InlineData("-1.1F", -1.1f)]
        [InlineData("1.1e5F", 1.1e5f)]
        [InlineData("-1.1E-5F", -1.1e-5f)]
        public void Test_Float(string token, float expected)
        {   
            // Act
            var result = _parser.Parse(token);
            
            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("1.1x100F")]
        [InlineData("1.1x-100F")]
        public void Test_Float_Invalid(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));
            
            // Assert
            Assert.Equal(String.Format("Unable to parse Single token: {0}", token), result.Message);
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