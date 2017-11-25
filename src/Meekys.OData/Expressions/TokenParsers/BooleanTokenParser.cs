using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class BooleanTokenParser : ITokenParser
    {
        public Expression Parse(string token)
        {
            if (string.Equals(token, bool.TrueString, StringComparison.OrdinalIgnoreCase))
                return Expression.Constant(true);

            if (string.Equals(token, bool.FalseString, StringComparison.OrdinalIgnoreCase))
                return Expression.Constant(false);

            return null;
        }
    }
}