using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions.TokenParsers
{
    public abstract class BaseSuffixTokenParser<T> : ITokenParser
    {
        private readonly string _suffix;

        protected BaseSuffixTokenParser(string suffix)
        {
            _suffix = suffix;
        }

        public Expression Parse(string token)
        {
            int number;
            if (!(token.Length > 0 && token[0] == '-')
                && !int.TryParse(token.Substring(0, 1), out number))
                return null;

            if (!token.EndsWith(_suffix, StringComparison.OrdinalIgnoreCase))
                return null;

            var value = token.Substring(0, token.Length - 1);

            return ParseValue(value)
                ?? ThrowBadFormatException(token);
        }

        protected abstract Expression ParseValue(string value);

        private Expression ThrowBadFormatException(string token)
        {
            throw new FormatException($"Unable to parse {typeof(T).Name} token: {token}");
        }
    }
}