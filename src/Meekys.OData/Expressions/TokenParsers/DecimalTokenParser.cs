using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class DecimalTokenParser : BaseSuffixTokenParser<decimal>
    {
        public DecimalTokenParser() : base(ExpressionConstants.Decimal)
        {
        }
        
        protected override Expression ParseValue(string value)
        {
            decimal val;
            if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return null;
                
            return Expression.Constant(val);
        }
    }
}