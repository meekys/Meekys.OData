using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class DoubleTokenParserTests
    {
        private DoubleTokenParser _parser = new DoubleTokenParser();

        [Theory]
        [InlineData("0D", 0d)]
        [InlineData("1D", 1d)]
        [InlineData("1d", 1d)]
        [InlineData("1.1D", 1.1d)]
        [InlineData("-1.1D", -1.1d)]
        [InlineData("1.1e5D", 1.1e5d)]
        [InlineData("-1.1E-5D", -1.1e-5d)]
        public void Test_Double(string token, double expected)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("1.1x100D")]
        [InlineData("1.1x-100D")]
        public void Test_Double_Invalid(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));

            // Assert
            Assert.Equal($"Unable to parse Double token: {token}", result.Message);
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