using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public class FilterTokeniser
    {
        private const char SpaceChar = ' ';
        private const char OpenBracketChar = '(';
        private const char CloseBracketChar = ')';
        private const char QuoteChar = '\'';
        private const char CommaChar = ',';

        private static readonly char[] Separators =
        {
            SpaceChar,
            OpenBracketChar,
            CloseBracketChar,
            QuoteChar,
            CommaChar
        };

        private readonly string _input;

        public FilterTokeniser(string input)
        {
            _input = input;
        }

        public IEnumerable<FilterToken> Tokens
        {
            get
            {
                var start = 0;
                var end = 0;

                do
                {
                    end = _input.IndexOfAny(Separators, start);

                    var token = NextToken(start, ref end);

                    if (token != null && (token = token.Trim()).Length > 0)
                        yield return new FilterToken(token, start);

                    start = end + 1;
                }
                while (start > 0);
            }
        }

        private string NextToken(int start, ref int end)
        {
            if (end == -1)
                return _input.Substring(start);

            switch (_input[end])
            {
                case SpaceChar:
                    return _input.Substring(start, end - start);

                case QuoteChar:
                    do
                    {
                        end = _input.IndexOf(QuoteChar, end + 1);
                    }
                    while (PeekAndProgress(ref end, QuoteChar));

                    if (end == -1)
                        throw new ArgumentException($"Missing end of string starting at position {start}");

                    var nextChar = NextChar(end);
                    if (nextChar != default(char) && !Separators.Contains(nextChar))
                        throw new ArgumentException($"Unexpected end of string at position {end}");

                    break;

                default:
                // case OpenBracketChar:
                // case CloseBracketChar:
                // case CommaChar:
                    if (start != end)
                        end--; // Accept previous token (Handle this next time)

                    break;
            }

            return _input.Substring(start, end - start + 1);
        }

        private char PrevChar(int position)
        {
            if (position <= 0)
                return default(char);

            return _input[position - 1];
        }

        private char NextChar(int position)
        {
            if (position == -1
                || position + 1 >= _input.Length)
                return default(char);

            return _input[position + 1];
        }

        private bool PeekAndProgress(ref int position, char lookFor)
        {
            if (position + 1 >= _input.Length)
                return false;

            if (_input[position + 1] != lookFor)
                return false;

            position++;

            return true;
        }
    }
}