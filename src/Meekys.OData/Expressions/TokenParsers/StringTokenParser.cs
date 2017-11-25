using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public class StringTokenParser : ITokenParser
    {
        public Expression Parse(string token)
        {
            if (!(token.Length > 0 && token.First() == '\'')
                || !(token.Length > 0 && token.Last() == '\''))
                return null;

            var val = token.Substring(1, token.Length - 2).Replace("''", "'");

            return Expression.Constant(val);
        }
    }
}