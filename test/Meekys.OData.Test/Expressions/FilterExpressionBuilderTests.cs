using System;
using System.Collections.Generic;
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
            var expression = FilterExpressionBuilder<TestType>.Build(string.Format("{0}", boolIn));
            
            // Assert
            Assert.Equal(string.Format("item => {0}", boolOut), expression.ToString());
        }

        [Theory]
        [InlineData("eq", "==")]
        [InlineData("ne", "!=")]
        public void Test_Comparison_Operators_Bool(string opIn, string opOut)
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build(string.Format("BoolValue {0} false", opIn));
            
            // Assert
            Assert.Equal(string.Format("item => (item.BoolValue {0} False)", opOut), expression.ToString());
        }

        [Theory]
        [InlineData("gt", "GreaterThan")]
        [InlineData("ge", "GreaterThanOrEqual")]
        [InlineData("lt", "LessThan")]
        [InlineData("le", "LessThanOrEqual")]
        public void Test_Comparison_Operators_Bool_Invalid(string opIn, string opOut)
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => (object)FilterExpressionBuilder<TestType>.Build(string.Format("BoolValue {0} false", opIn)));
            
            // Assert
            Assert.Equal(string.Format("The binary operator {0} is not defined for the types 'System.Boolean' and 'System.Boolean'.", opOut), exception.Message);
        }

        [Theory]
        [InlineData("eq", "==")]
        [InlineData("ne", "!=")]
        [InlineData("gt", ">")]
        [InlineData("ge", ">=")]
        [InlineData("lt", "<")]
        [InlineData("le", "<=")]
        public void Test_Comparison_Operators_Int(string opIn, string opOut)
        {
            // Act
            var expression = FilterExpressionBuilder<TestType>.Build(string.Format("IntegerValue {0} 5", opIn));
            
            // Assert
            Assert.Equal(string.Format("item => (item.IntegerValue {0} 5)", opOut), expression.ToString());
        }

        [Fact]
        public void Test_Negate_Constant()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("not true");
            
            // Assert
            Assert.Equal("item => Not(True)", expression.ToString());
        }
        
        [Fact]
        public void Test_Negate_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("not BoolValue");
            
            // Assert
            Assert.Equal("item => Not(item.BoolValue)", expression.ToString());
        }

        [Fact]
        public void Test_Negate_Expression_With_Parenthesis()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("not(IntegerValue eq 5)");
            
            // Assert
            Assert.Equal("item => Not((item.IntegerValue == 5))", expression.ToString());
        }

        [Fact]
        public void Test_Precidence_Sum_Product()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("1 add 2 sub 4 div 2 mul 3 sub 5 add 10 gt 5");
            
            // Assert
            Assert.Equal("item => (((((1 + 2) - ((4 / 2) * 3)) - 5) + 10) > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Precidence_Sum_Product_With_Parenthesis()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("1 add (2 sub 4) div 2 mul (3 sub 5) add 10 gt 5");
            
            // Assert
            Assert.Equal("item => (((1 + (((2 - 4) / 2) * (3 - 5))) + 10) > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Double()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("2147483647 eq DoubleValue");
            
            // Assert
            Assert.Equal("item => (2147483647 == item.DoubleValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Float()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("2147483647 eq FloatValue");
            
            // Assert
            Assert.Equal("item => (2.147484E+09 == item.FloatValue)", expression.ToString());
        }
        
        [Fact]
        public void Test_Converting_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("2147483647 eq DecimalValue");
            
            // Assert
            Assert.Equal("item => (2147483647 == item.DecimalValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Long()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("2147483647 eq LongValue");
            
            // Assert
            Assert.Equal("item => (2147483647 == item.LongValue)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Short()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("IntegerValue eq 32767");
            
            // Assert
            Assert.Equal("item => (item.IntegerValue == 32767)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("IntegerValue eq 255");
            
            // Assert
            Assert.Equal("item => (item.IntegerValue == 255)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte_To_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("DecimalValue gt 10");
            
            // Assert
            Assert.Equal("item => (item.DecimalValue > 10)", expression.ToString());
        }

        [Fact]
        public void Test_Converting_Byte_To_NullableDecimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("NullableDecimalValue gt 26");
            
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
            var expression = FilterExpressionBuilder<TestType>.Build($"{fieldName}Value eq {fieldName}Value");
            
            // Assert
            Assert.Equal($"item => (item.{fieldName}Value == item.{fieldName}Value)", expression.ToString());
        }

        [Theory]
        [InlineData("Integer", "Short")]
        [InlineData("Integer", "Byte")]
        [InlineData("Integer", "Short")]
        [InlineData("NullableInteger", "Short")]
        [InlineData("NullableInteger", "Byte")]
        [InlineData("NullableInteger", "Short")]
        [InlineData("NullableInteger", "Integer")]
        [InlineData("Long", "Byte")]
        [InlineData("Long", "Short")]
        [InlineData("Long", "Integer")]
        [InlineData("NullableLong", "Byte")]
        [InlineData("NullableLong", "Short")]
        [InlineData("NullableLong", "Integer")]
        [InlineData("NullableLong", "Long")]
        [InlineData("Decimal", "Byte")]
        [InlineData("Decimal", "Short")]
        [InlineData("Decimal", "Integer")]
        [InlineData("Decimal", "Long")]
        [InlineData("NullableDecimal", "Byte")]
        [InlineData("NullableDecimal", "Short")]
        [InlineData("NullableDecimal", "Integer")]
        [InlineData("NullableDecimal", "Long")]
        [InlineData("NullableDecimal", "Decimal")]
        [InlineData("Float", "Byte")]
        [InlineData("Float", "Short")]
        [InlineData("Float", "Integer")]
        [InlineData("Float", "Long")]
        [InlineData("Float", "Decimal")]
        [InlineData("NullableFloat", "Byte")]
        [InlineData("NullableFloat", "Short")]
        [InlineData("NullableFloat", "Integer")]
        [InlineData("NullableFloat", "Long")]
        [InlineData("NullableFloat", "Decimal")]
        [InlineData("NullableFloat", "Float")]
        [InlineData("Double", "Byte")]
        [InlineData("Double", "Short")]
        [InlineData("Double", "Integer")]
        [InlineData("Double", "Long")]
        [InlineData("Double", "Decimal")]
        [InlineData("Double", "Float")]
        [InlineData("NullableDouble", "Byte")]
        [InlineData("NullableDouble", "Short")]
        [InlineData("NullableDouble", "Integer")]
        [InlineData("NullableDouble", "Long")]
        [InlineData("NullableDouble", "Decimal")]
        [InlineData("NullableDouble", "Float")]
        [InlineData("NullableDouble", "Double")]
        [InlineData("NullableBool", "Bool")]
        public void Test_Casting_Operands(string fieldName, string castFieldName)
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build($"{fieldName}Value eq {castFieldName}Value");
            
            // Assert
            Assert.Equal($"item => (item.{fieldName}Value == Convert(item.{castFieldName}Value))", expression.ToString());
        }

        [Theory]
        [InlineData("Short", "Byte")]
        [InlineData("Short", "Short")]
        [InlineData("NullableShort", "NullableByte")]
        [InlineData("NullableShort", "NullableShort")]
        public void Test_Casting_Operands_CastToInt(string fieldName, string castFieldName)
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build($"{fieldName}Value eq {castFieldName}Value");
            
            // Assert
            Assert.Equal($"item => (Convert(item.{fieldName}Value) == Convert(item.{castFieldName}Value))", expression.ToString());
        }

        [Fact]
        public void Test_Unrecognised_Token()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("IntegerValue eq Unknown")));
            
            // Assert
            Assert.Equal("Unrecognised token: Unknown @ 16", exception.Message);
        }

        [Fact]
        public void Test_Unexpected_Token()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("IntegerValue eq 5 Unexpected")));
            
            // Assert
            Assert.Equal("Unexpected token: Unexpected @ 18", exception.Message);
        }
                
        [Fact]
        public void Test_Expected_Boolean()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("IntegerValue")));
            
            // Assert
            Assert.Equal("Expected boolean expression", exception.Message);
        }

        [Fact]
        public void Test_Expected_Close_Parenthesis_But_Different()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("(IntegerValue gt 5 Different")));
            
            // Assert
            Assert.Equal("Expected ) but got Different @ 19", exception.Message);
        }

        [Fact]
        public void Test_Expected_Close_Parenthesis()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("(IntegerValue gt 5")));
            
            // Assert
            Assert.Equal("Expected ) but got {No Token} after 5 @ 17", exception.Message);
        }

        [Fact]
        public void Test_Expected_Function_Close_Parenthesis_But_Different()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("Function(StringValue Different")));
            
            // Assert
            Assert.Equal("Expected ) but got Different @ 21", exception.Message);
        }

        [Fact]
        public void Test_Expected_Function_Close_Parenthesis()
        {
            // Arrange
            var exception = Assert.Throws<FilterExpressionException>(() => (object)(FilterExpressionBuilder<TestType>.Build("Function(StringValue")));

            // Assert
            Assert.Equal("Expected ) but got {No Token} after StringValue @ 9", exception.Message);
        }

        [Fact]
        public void Test_Unknown_Function()
        {
            // Arrange
            var exception = Assert.Throws<InvalidOperationException>(() => (object)(FilterExpressionBuilder<TestType>.Build("Unknown(StringValue)")));

            // Assert
            Assert.Equal("Unmatched function Unknown(String)", exception.Message);
        }

        [Fact]
        public void Test_Function_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("length(StringValue) gt 5");

            // Assert
            Assert.Equal("item => (item.StringValue.Length > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Single_Parameter()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("tolower(StringValue) eq 'test'");

            // Assert
            Assert.Equal("item => (item.StringValue.ToLower() == \"test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Multiple_Parameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("indexof(StringValue, 'Test') gt 5");

            // Assert
            Assert.Equal("item => (item.StringValue.IndexOf(\"Test\") > 5)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Overload_Single_Parameter()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("substring(StringValue, 5) eq 'Test'");

            // Assert
            Assert.Equal("item => (item.StringValue.Substring(5) == \"Test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Overload_Multiple_Parameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("substring(StringValue, 5, 4) eq 'Test'");

            // Assert
            Assert.Equal("item => (item.StringValue.Substring(5, 4) == \"Test\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_MultipleParameters()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("concat('Some', 'thing') eq 'Something'");

            // Assert
            Assert.Equal("item => (Concat(\"Some\", \"thing\") == \"Something\")", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Decimal()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("round(DecimalValue) eq 1m");

            // Assert
            Assert.Equal("item => (Round(item.DecimalValue) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Double()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("round(DoubleValue) eq 1d");

            // Assert
            Assert.Equal("item => (Round(item.DoubleValue) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Function_Static_Overload_Double_With_Float()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("round(FloatValue) eq 1d");

            // Assert
            Assert.Equal("item => (Round(Convert(item.FloatValue)) == 1)", expression.ToString());
        }

        [Fact]
        public void Test_Case_Insensitive_Property()
        {
            // Arrange
            var expression = FilterExpressionBuilder<TestType>.Build("decimalvalue gt 10");

            // Assert
            Assert.Equal("item => (item.DecimalValue > 10)", expression.ToString());
        }

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