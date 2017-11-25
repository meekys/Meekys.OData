using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class ByteTokenParser : ITokenParser
    {
        public Expression Parse(string token)
        {
            byte val;

            if (!byte.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return null;

            return Expression.Constant(val);
        }
    }
}