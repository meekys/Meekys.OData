using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class NullTokenParser : BaseTypedTokenParser
    {
        public NullTokenParser()
            : base(ExpressionConstants.Null)
        {
        }

        [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1062", Justification = "Asserted")]
        protected override Expression ParseValue(string value)
        {
            Debug.Assert(value != null, "value should not be null");

            if (value.Length > 0)
                throw new NotImplementedException("Explicitly typed null constants not supported");

            return Expression.Constant(null);
        }
    }
}