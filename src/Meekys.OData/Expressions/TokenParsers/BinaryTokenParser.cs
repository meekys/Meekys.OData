using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class BinaryTokenParser : BaseTypedTokenParser
    {
        public BinaryTokenParser()
            : base(ExpressionConstants.Binary)
        {
        }

        protected override bool CheckType(string type)
        {
            return type == ExpressionConstants.Binary
                || type == ExpressionConstants.BinaryX;
        }

        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1062", Justification = "Asserted")]
        protected override Expression ParseValue(string value)
        {
            Debug.Assert(value != null, "value should not be null");

            if (value.Length % 2 != 0)
                throw new FormatException($"Unable to parse binary token: {value}");

            try
            {
                var val = Enumerable.Range(0, value.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                    .ToArray();

                return Expression.Constant(val);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Unable to parse binary token: {value}", ex);
            }
        }
    }
}