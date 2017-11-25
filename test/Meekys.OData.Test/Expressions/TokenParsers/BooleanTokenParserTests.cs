using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class BooleanTokenParserTests
    {
        private BooleanTokenParser _parser = new BooleanTokenParser();

        [Theory]
        [InlineData("true")]
        [InlineData("TRUE")]
        [InlineData("True")]
        /*[InlineData("1")]*/
        public void Test_True(string token)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal((object)true, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("FALSE")]
        [InlineData("False")]
        /*[InlineData("0")]*/
        public void Test_False(string token)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal((object)false, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("Invalid")]
        public void Test_Passthrough(string token)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.Null(result);
        }
    }
}