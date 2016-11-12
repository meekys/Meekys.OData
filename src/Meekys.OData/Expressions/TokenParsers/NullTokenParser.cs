using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class NullTokenParser : BaseTypedTokenParser
    {
        public NullTokenParser() : base(ExpressionConstants.Null)
        {
        }
        
        protected override Expression ParseValue(string value)
        {               
            if (value.Length > 0)
                throw new NotImplementedException("Explicitly typed null constants not supported");
                
            return Expression.Constant(null);
        }
    }
}