using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public class FilterToken
    {
        public static implicit operator string(FilterToken token)
        {
            return token?.Token;
        }

        public string Token { get; }
        public int Position { get; }

        public FilterToken(string token, int position)
        {
            Token = token;
            Position = position;
        }

        public override string ToString() => $"{Token} @ {Position}";
    }
}