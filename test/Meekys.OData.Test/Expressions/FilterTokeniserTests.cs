using System;
using System.Collections.Generic;
using System.Linq;

using Meekys.OData.Expressions;

using Xunit;

namespace Meekys.OData.Tests.Expressions
{
    public class FilterTokeniserTests
    {
        [Fact]
        public void Test_Tokens_Multiple_Spaces_Removed()
        {
            // Arrange
            var tokens = new FilterTokeniser("a      b").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", "b" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 7 });
        }

        [Fact]
        public void Test_Tokens_Start_Bracket()
        {
            // Arrange
            var tokens = new FilterTokeniser("a(b").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", "(", "b" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 1, 2 });
        }

        [Fact]
        public void Test_Tokens_End_Bracket()
        {
            // Arrange
            var tokens = new FilterTokeniser("a)b").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", ")", "b" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 1, 2 });
        }

        [Fact]
        public void Test_Tokens_Comma()
        {
            // Arrange
            var tokens = new FilterTokeniser("a,b").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", ",", "b" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 1, 2 });
        }

        [Fact]
        public void Test_Tokens_Comma_With_Spaces()
        {
            // Arrange
            var tokens = new FilterTokeniser("a , b").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", ",", "b" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 2, 4 });
        }

        [Fact]
        public void Test_Tokens_Embedded_Brackets_Without_Spaces()
        {
            // Arrange
            var tokens = new FilterTokeniser("a(b)c").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", "(", "b", ")", "c" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 1, 2, 3, 4 });
        }

        [Fact]
        public void Test_Tokens_Embedded_Brackets_With_Sapces()
        {
            // Arrange
            var tokens = new FilterTokeniser(" a ( b ) c ").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "a", "(", "b", ")", "c" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 1, 3, 5, 7, 9 });
        }

        [Fact]
        public void Test_Tokens_Quoted_Start_And_End()
        {
            // Arrange
            var tokens = new FilterTokeniser("'Test'").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "'Test'" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0 });
        }

        [Fact]
        public void Test_Tokens_Quoted_Middle()
        {
            // Arrange
            var tokens = new FilterTokeniser("Test 'Test' Test").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "Test", "'Test'", "Test" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 5, 12 });
        }

        [Fact]
        public void Test_Tokens_Quoted_With_Escaping()
        {
            // Arrange
            var tokens = new FilterTokeniser("'Testing ''quotes'' and (brackets) in strings'").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "'Testing ''quotes'' and (brackets) in strings'" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0 });
        }

        [Fact]
        public void Test_Tokens_Quoted_Unclosed()
        {
            // Arrange
            var tokens = new FilterTokeniser("a 'b c").Tokens;

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => (object)tokens.ToList());
            Assert.Equal("Missing end of string starting at position 2", ex.Message);
        }

        [Fact]
        public void Test_Tokens_Quoted_Esacaped_And_Unclosed()
        {
            // Arrange
            var tokens = new FilterTokeniser("a 'b'' c").Tokens;

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => (object)tokens.ToList());
            Assert.Equal("Missing end of string starting at position 2", ex.Message);
        }

        [Fact]
        public void Test_Tokens_Quoted_With_Leading()
        {
            // Arrange
            var tokens = new FilterTokeniser("Test prefix'value' Test").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "Test", "prefix'value'", "Test" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 5, 19 });
        }

        [Fact]
        public void Test_Tokens_Quoted_Bad_Trailing()
        {
            // Arrange
            var tokens = new FilterTokeniser("Test 'value'trailing Test").Tokens;

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => (object)tokens.ToList());
            Assert.Equal("Unexpected end of string at position 11", ex.Message);
        }

        [Fact]
        public void Test_Tokens_Brackets_Embedded_Quotes()
        {
            // Arrange
            var tokens = new FilterTokeniser("('Test')").Tokens;

            // Act + Assert
            Assert.Equal(tokens.Select(t => t.Token), new[] { "(", "'Test'", ")" });
            Assert.Equal(tokens.Select(t => t.Position), new[] { 0, 1, 7 });
        }
    }
}