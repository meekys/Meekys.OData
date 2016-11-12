using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class GuidTokenParser : BaseTypedTokenParser
    {
        public GuidTokenParser() : base(ExpressionConstants.Guid)
        {
        }
        
        protected override Expression ParseValue(string value)
        {
            Guid val;
            if (!Guid.TryParse(value, out val))
                return null;
            
            return Expression.Constant(val);
        }
    }
}