using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class FloatTokenParser : BaseSuffixTokenParser<float>
    {
        public FloatTokenParser()
            : base(ExpressionConstants.Float)
        {
        }

        protected override Expression ParseValue(string value)
        {
            float val;
            if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return null;

            return Expression.Constant(val);
        }
    }
}