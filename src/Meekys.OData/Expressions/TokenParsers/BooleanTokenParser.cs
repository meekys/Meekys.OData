using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class BooleanTokenParser : ITokenParser
    {
        private const string TrueNumber = "1";
        private const string FalseNumber = "0";
        
        public Expression Parse(string token)
        {
            if (string.Equals(token, System.Boolean.TrueString, StringComparison.OrdinalIgnoreCase)
                /*|| token == TrueNumber*/)
                return Expression.Constant(true);
                
            if (string.Equals(token, System.Boolean.FalseString, StringComparison.OrdinalIgnoreCase)
                /*|| token == FalseNumber*/)
                return Expression.Constant(false);
                
            return null;
        }
    }
}