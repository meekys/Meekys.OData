using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class DoubleTokenParser : BaseSuffixTokenParser<double>
    {
        public DoubleTokenParser() : base(ExpressionConstants.Double)
        {
        }
        
        protected override Expression ParseValue(string value)
        {
            double val;
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return null;
                
            return Expression.Constant(val);
        }
    }
}