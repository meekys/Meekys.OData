using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class DateTimeTokenParserTests
    {
        private DateTimeTokenParser _parser = new DateTimeTokenParser();

        [Theory]
        [InlineData("datetime'2016-09-23T23:59'")]
        [InlineData("DATETIME'2016-09-23T23:59'")]
        [InlineData("DatetimE'2016-09-23T23:59'")]
        [InlineData("datetime'2016-09-23T23:59:59'")]
        [InlineData("datetime'2016-09-23T23:59:59.1234567'")]
        [InlineData("datetime'2016-09-23T23:59:59+9:30'")]
        [InlineData("datetime'2016-09-23T23:59:59-7:30'")]
        /*[InlineData("datetime'2016-09-23T23:59:59Z'")]*/
        public void Test_DateTime(string token)
        {
            // Arrange
            var expected = DateTime.Parse(token.Split('\'')[1], CultureInfo.InvariantCulture);

            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("datetime'invalid'")]
        [InlineData("DATETIME'invalid'")]
        [InlineData("DatetimE'invalid'")]
        public void Test_DateTime_Invalid(string token)
        {
            // Act
            var ex = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));

            // Assert
            Assert.Equal($"Unable to parse datetime token: {token}", ex.Message);
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