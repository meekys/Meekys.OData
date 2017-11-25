using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public static class ExpressionConstants
    {
        public const char Quote = '\'';
        public const string OpenParen = "(";
        public const string CloseParen = ")";
        public const string Comma = ",";
        public const string Null = "null";
        public const string DateTime = "datetime";
        public const string Binary = "binary";
        public const string BinaryX = "X";
        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1720:IdentifierContainsTypeName", Justification = "OData type")]
        public const string Guid = "guid";
        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1720:IdentifierContainsTypeName", Justification = "OData type")]
        public const string Long = "l";
        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1720:IdentifierContainsTypeName", Justification = "OData type")]
        public const string Decimal = "m";
        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1720:IdentifierContainsTypeName", Justification = "OData type")]
        public const string Double = "d";
        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1720:IdentifierContainsTypeName", Justification = "OData type")]
        public const string Float = "f";
    }
}