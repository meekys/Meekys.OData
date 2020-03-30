using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Meekys.OData.Expressions;

using Xunit;

namespace Meekys.OData.Tests.Expressions
{
    public class FilterExpressionBuilderTests
    {
        [Theory]
        [InlineData("true", "True")]
        [InlineData("false", "False")]
        public void Test_Binary_Constant(string boolIn, string boolOut)
        {
            // Act
            var expression = FilterExpressionBuilder.Build<TestType>(boolIn);

            // Assert
            Assert.Equal($"item => {boolOut}", expression.ToString());
        }

        [Theory]
        [InlineData("eq", "==")]
        [InlineData("ne", "!=")]
        public void Test_Comparison_Operators_Bool(string operatorIn, string operatorOut)
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>($"BoolValue {operatorIn} false");

            // Assert
            Assert.Equal($"item => (item.BoolValue {operatorOut} False)", expression.ToString());
        }

        [Theory]
        [InlineData("gt", "GreaterThan")]
        [InlineData("ge", "GreaterThanOrEqual")]
        [InlineData("lt", "LessThan")]
        [InlineData("le", "LessThanOrEqual")]
        public void Test_Comparison_Operators_Bool_Invalid(string operatorIn, string operatorOut)
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(()
                => (object)FilterExpressionBuilder.Build<TestType>($"BoolValue {operatorIn} false"));

            // Assert
            Assert.Equal($"The binary operator {operatorOut} is not defined for the types 'System.Boolean' and 'System.Boolean'.", exception.Message);
        }

        [Theory]
        [InlineData("eq", "==")]
        [InlineData("ne", "!=")]
        [InlineData("gt", ">")]
        [InlineData("ge", ">=")]
        [InlineData("lt", "<")]
        [InlineData("le", "<=")]
        public void Test_Comparison_Operators_Int(string operatorIn, string operatorOut)
        {
            // Act
            var expression = FilterExpressionBuilder.Build<TestType>($"IntegerValue {operatorIn} 5");

            // Assert
            Assert.Equal($"item => (item.IntegerValue {operatorOut} 5)", expression.ToString());
        }

        [Fact]
        public void Test_Negate_Constant()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("not true");

            // Assert
            Assert.Equal("item => Not(True)", expression.ToString());
        }

        [Fact]
        public void Test_Negate_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("not BoolValue");

            // Assert
            Assert.Equal("item => Not(item.BoolValue)", expression.ToString());
        }

        [Fact]
        public void Test_Negate_Expression_With_Parenthesis()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("not(IntegerValue eq 5)");

            // Assert
            Assert.Equal("item => Not((item.IntegerValue == 5))", expression.ToString());
        }

        [Fact]
        public void Test_Precidence_Sum_Product()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("1 add 2 sub 4 div 2 mul 3 sub 5 add 10 gt 5");

            // Assert
            Assert.Equal("item => (((((1 + 2) - ((4 / 2) * 3)) - 5) + 10) > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Precidence_Sum_Product_With_Parenthesis()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("1 add (2 sub 4) div 2 mul (3 sub 5) add 10 gt 5");

            // Assert
            Assert.Equal("item => (((1 + (((2 - 4) / 2) * (3 - 5))) + 10) > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Double()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("2147483647 eq DoubleValue");

            // Assert
            Assert.Equal("item => (2147483647 == item.DoubleValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Float()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>($"2147483647 eq FloatValue");

            // Assert
            Assert.Equal("item => (2.1474836E+09 == item.FloatValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("2147483647 eq DecimalValue");

            // Assert
            Assert.Equal("item => (2147483647 == item.DecimalValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Long()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("2147483647 eq LongValue");

            // Assert
            Assert.Equal("item => (2147483647 == item.LongValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Short()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("IntegerValue eq 32767");

            // Assert
            Assert.Equal("item => (item.IntegerValue == 32767)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("IntegerValue eq 255");

            // Assert
            Assert.Equal("item => (item.IntegerValue == 255)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte_To_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("DecimalValue gt 10");

            // Assert
            Assert.Equal("item => (item.DecimalValue > 10)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte_To_NullableDecimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("NullableDecimalValue gt 26");

            // Assert
            Assert.Equal("item => (item.NullableDecimalValue > 26)", expression.ToString());
        }

        [Theory]
        [InlineData("Bool")]
        [InlineData("Integer")]
        [InlineData("Long")]
        [InlineData("Decimal")]
        [InlineData("Float")]
        [InlineData("Double")]
        [InlineData("NullableBool")]
        [InlineData("NullableInteger")]
        [InlineData("NullableLong")]
        [InlineData("NullableDecimal")]
        [InlineData("NullableFloat")]
        [InlineData("NullableDouble")]
        public void Test_Casting_Operands_Same_Type(string fieldName)
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>($"{fieldName}Value eq {fieldName}Value");

            // Assert
            Assert.Equal($"item => (item.{fieldName}Value == item.{fieldName}Value)", expression.ToString());
        }

        [Theory]
        [InlineData("Integer", "Byte", "Int32")]
        [InlineData("Integer", "Short", "Int32")]
        [InlineData("NullableInteger", "Byte", "Nullable`1")]
        [InlineData("NullableInteger", "Short", "Nullable`1")]
        [InlineData("NullableInteger", "Integer", "Nullable`1")]
        [InlineData("Long", "Byte", "Int64")]
        [InlineData("Long", "Short", "Int64")]
        [InlineData("Long", "Integer", "Int64")]
        [InlineData("NullableLong", "Byte", "Nullable`1")]
        [InlineData("NullableLong", "Short", "Nullable`1")]
        [InlineData("NullableLong", "Integer", "Nullable`1")]
        [InlineData("NullableLong", "Long", "Nullable`1")]
        [InlineData("Decimal", "Byte", "Decimal")]
        [InlineData("Decimal", "Short", "Decimal")]
        [InlineData("Decimal", "Integer", "Decimal")]
        [InlineData("Decimal", "Long", "Decimal")]
        [InlineData("NullableDecimal", "Byte", "Nullable`1")]
        [InlineData("NullableDecimal", "Short", "Nullable`1")]
        [InlineData("NullableDecimal", "Integer", "Nullable`1")]
        [InlineData("NullableDecimal", "Long", "Nullable`1")]
        [InlineData("NullableDecimal", "Decimal", "Nullable`1")]
        [InlineData("Float", "Byte", "Single")]
        [InlineData("Float", "Short", "Single")]
        [InlineData("Float", "Integer", "Single")]
        [InlineData("Float", "Long", "Single")]
        [InlineData("Float", "Decimal", "Single")]
        [InlineData("NullableFloat", "Byte", "Nullable`1")]
        [InlineData("NullableFloat", "Short", "Nullable`1")]
        [InlineData("NullableFloat", "Integer", "Nullable`1")]
        [InlineData("NullableFloat", "Long", "Nullable`1")]
        [InlineData("NullableFloat", "Decimal", "Nullable`1")]
        [InlineData("NullableFloat", "Float", "Nullable`1")]
        [InlineData("Double", "Byte", "Double")]
        [InlineData("Double", "Short", "Double")]
        [InlineData("Double", "Integer", "Double")]
        [InlineData("Double", "Long", "Double")]
        [InlineData("Double", "Decimal", "Double")]
        [InlineData("Double", "Float", "Double")]
        [InlineData("NullableDouble", "Byte", "Nullable`1")]
        [InlineData("NullableDouble", "Short", "Nullable`1")]
        [InlineData("NullableDouble", "Integer", "Nullable`1")]
        [InlineData("NullableDouble", "Long", "Nullable`1")]
        [InlineData("NullableDouble", "Decimal", "Nullable`1")]
        [InlineData("NullableDouble", "Float", "Nullable`1")]
        [InlineData("NullableDouble", "Double", "Nullable`1")]
        [InlineData("NullableBool", "Bool", "Nullable`1")]
        public void Test_Casting_Operands(string fieldName, string castFieldName, string castType)
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>($"{fieldName}Value eq {castFieldName}Value");

            // Assert
            Assert.Equal($"item => (item.{fieldName}Value == Convert(item.{castFieldName}Value, {castType}))", expression.ToString());
        }

        [Theory]
        [InlineData("Short", "Byte", "Int32")]
        [InlineData("Short", "Short", "Int32")]
        [InlineData("NullableShort", "NullableByte", "Nullable`1")]
        [InlineData("NullableShort", "NullableShort", "Nullable`1")]
        public void Test_Casting_Operands_CastToInt(string fieldName, string castFieldName, string castType)
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>($"{fieldName}Value eq {castFieldName}Value");

            // Assert
            Assert.Equal($"item => (Convert(item.{fieldName}Value, {castType}) == Convert(item.{castFieldName}Value, {castType}))", expression.ToString());
        }

        [Fact]
        public void Test_Unrecognised_Token()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("IntegerValue eq Unknown"));

            // Assert
            Assert.Equal("Unrecognised token: Unknown @ 16", exception.Message);
        }

        [Fact]
        public void Test_Unexpected_Token()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("IntegerValue eq 5 Unexpected"));

            // Assert
            Assert.Equal("Unexpected token: Unexpected @ 18", exception.Message);
        }

        [Fact]
        public void Test_Expected_Boolean()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("IntegerValue"));

            // Assert
            Assert.Equal("Expected boolean expression", exception.Message);
        }

        [Fact]
        public void Test_Expected_Close_Parenthesis_But_Different()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("(IntegerValue gt 5 Different"));

            // Assert
            Assert.Equal("Expected ) but got Different @ 19", exception.Message);
        }

        [Fact]
        public void Test_Expected_Close_Parenthesis()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("(IntegerValue gt 5"));

            // Assert
            Assert.Equal("Expected ) but got {No Token} after 5 @ 17", exception.Message);
        }

        [Fact]
        public void Test_Expected_Function_Close_Parenthesis_But_Different()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("Function(StringValue Different"));

            // Assert
            Assert.Equal("Expected ) but got Different @ 21", exception.Message);
        }

        [Fact]
        public void Test_Expected_Function_Close_Parenthesis()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)FilterExpressionBuilder.Build<TestType>("Function(StringValue"));

            // Assert
            Assert.Equal("Expected ) but got {No Token} after StringValue @ 9", exception.Message);
        }

        [Fact]
        public void Test_Unknown_Function()
        {
            // Arrange
            var exception = Assert.Throws<InvalidOperationException>(() => (object)FilterExpressionBuilder.Build<TestType>("Unknown(StringValue)"));

            // Assert
            Assert.Equal("Unmatched function Unknown(String)", exception.Message);
        }

        [Fact]
        public void Test_Function_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("length(StringValue) gt 5");

            // Assert
            Assert.Equal("item => (item.StringValue.Length > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Single_Parameter()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("tolower(StringValue) eq 'test'");

            // Assert
            Assert.Equal("item => (item.StringValue.ToLower() == \"test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Multiple_Parameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("indexof(StringValue, 'Test') gt 5");

            // Assert
            Assert.Equal("item => (item.StringValue.IndexOf(\"Test\") > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Overload_Single_Parameter()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("substring(StringValue, 5) eq 'Test'");

            // Assert
            Assert.Equal("item => (item.StringValue.Substring(5) == \"Test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Overload_Multiple_Parameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("substring(StringValue, 5, 4) eq 'Test'");

            // Assert
            Assert.Equal("item => (item.StringValue.Substring(5, 4) == \"Test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_MultipleParameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("concat('Some', 'thing') eq 'Something'");

            // Assert
            Assert.Equal("item => (Concat(\"Some\", \"thing\") == \"Something\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("round(DecimalValue) eq 1m");

            // Assert
            Assert.Equal("item => (Round(item.DecimalValue) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Double()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("round(DoubleValue) eq 1d");

            // Assert
            Assert.Equal("item => (Round(item.DoubleValue) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Double_With_Float()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("round(FloatValue) eq 1d");

            // Assert
            Assert.Equal("item => (Round(Convert(item.FloatValue, Double)) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Case_Insensitive_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder.Build<TestType>("decimalvalue gt 10");

            // Assert
            Assert.Equal("item => (item.DecimalValue > 10)", expression.ToString());
        }

        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1812:InternalClassNeverInstantiated", Justification = "Type referenced for testing")]
        private class TestType
        {
            public string StringValue { get; set; }

            public byte ByteValue { get; set; }

            public short ShortValue { get; set; }

            public int IntegerValue { get; set; }

            public long LongValue { get; set; }

            public decimal DecimalValue { get; set; }

            public double DoubleValue { get; set; }

            public float FloatValue { get; set; }

            public bool BoolValue { get; set; }

            public byte? NullableByteValue { get; set; }

            public short? NullableShortValue { get; set; }

            public int? NullableIntegerValue { get; set; }

            public long? NullableLongValue { get; set; }

            public decimal? NullableDecimalValue { get; set; }

            public double? NullableDoubleValue { get; set; }

            public float? NullableFloatValue { get; set; }

            public bool? NullableBoolValue { get; set; }
        }
    }
}