using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Meekys.OData.Expressions.TokenParsers;

using Xunit;

namespace Meekys.OData.Tests.Expressions.TokenParsers
{
    public class GuidTokenParserTests
    {
        private GuidTokenParser _parser = new GuidTokenParser();

        [Theory]
        [InlineData("guid'00000000-0000-0000-0000-000000000000")]
        [InlineData("GUID'00000000-0000-0000-0000-000000000000")]
        [InlineData("GuiD'00000000-0000-0000-0000-000000000000")]
        public void Test_Guid(string token)
        {
            // Act
            var result = _parser.Parse(token);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(Guid.Empty, (result as ConstantExpression).Value);
        }

        [Theory]
        [InlineData("guid'invalid'")]
        [InlineData("GUID'invalid'")]
        [InlineData("GuiD'invalid'")]
        public void Test_Guid_Invalid(string token)
        {
            // Act
            var result = Assert.Throws<FormatException>(() => (object)_parser.Parse(token));

            // Assert
            Assert.Equal($"Unable to parse guid token: {token}", result.Message);
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