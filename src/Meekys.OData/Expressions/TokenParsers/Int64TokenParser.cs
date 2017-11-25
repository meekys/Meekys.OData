using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class Int64TokenParser : BaseSuffixTokenParser<long>
    {
        public Int64TokenParser()
            : base(ExpressionConstants.Long)
        {
        }

        protected override Expression ParseValue(string value)
        {
            long val;
            if (!long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return null;

            return Expression.Constant(val);
        }
    }
}