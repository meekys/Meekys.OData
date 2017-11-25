using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class StringTokenParserTests
    {
        private StringTokenParser _parser = new StringTokenParser();

        [Theory]
        [InlineData("'string'", "string")]
        [InlineData("'string with ''escaped'' quotes'", "string with 'escaped' quotes")]
        public void Test_String(string token, string expected)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
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