using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class DecimalTokenParserTests
    {
        private DecimalTokenParser _parser = new DecimalTokenParser();

        [Theory]
        [InlineData("0M")]
        [InlineData("1M")]
        [InlineData("1m")]
        [InlineData("1.1234567890123456789012345678M")]
        [InlineData("-1.1234567890123456789012345678M")]
        [InlineData("1.12345678901234567890123456789M")]
        [InlineData("-1.12345678901234567890123456789M")]
        [InlineData("79228162514264337593543950335M")]
        [InlineData("-79228162514264337593543950335M")]
        public void Test_Int64(string token)
        {
            // Arrange
            var expected = decimal.Parse(token.Substring(0, token.Length - 1), CultureInfo.InvariantCulture);

            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("79228162514264337593543950336M")]
        [InlineData("-79228162514264337593543950336M")]
        public void Test_Int64_Invalid(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));

            // Assert
            Assert.Equal($"Unable to parse Decimal token: {token}", result.Message);
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