using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public abstract class BaseTypedTokenParser : ITokenParser
    {
        private readonly string _type;

        protected BaseTypedTokenParser(string type)
        {
           _type = type;
        }

        public Expression Parse(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            var parts = token.Split(ExpressionConstants.Quote);

            if (!CheckType(parts[0]))
                return null;

            var value = parts.Length > 1 ? parts[1] : string.Empty;

            return ParseValue(value)
                ?? ThrowBadFormatException(token);
        }

        protected virtual bool CheckType(string type)
        {
            return string.Equals(type, _type, StringComparison.OrdinalIgnoreCase);
        }

        protected abstract Expression ParseValue(string value);

        private Expression ThrowBadFormatException(string token)
        {
            throw new FormatException($"Unable to parse {_type} token: {token}");
        }
    }
}