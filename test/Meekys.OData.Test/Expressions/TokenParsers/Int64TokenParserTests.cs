using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class Int64TokenParserTests
    {
        private Int64TokenParser _parser = new Int64TokenParser();

        [Theory]
        [InlineData("0L", 0)]
        [InlineData("1L", 1)]
        [InlineData("1l", 1)]
        [InlineData("-1L", -1)]
        [InlineData("9223372036854775807L", 9223372036854775807L)]
        [InlineData("-9223372036854775808L", -9223372036854775808L)]
        public void Test_Int64(string token, long expected)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("9223372036854775808L")]
        [InlineData("-9223372036854775809L")]
        public void Test_Int64_Invalid(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));

            // Assert
            Assert.Equal($"Unable to parse Int64 token: {token}", result.Message);
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