using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class DateTimeTokenParser : BaseTypedTokenParser
    {
        public DateTimeTokenParser()
            : base(ExpressionConstants.DateTime)
        {
        }

        protected override Expression ParseValue(string value)
        {
            DateTime val;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out val))
                return null;

            return Expression.Constant(val);
        }
    }
}