using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

using Meekys.Common;
using Meekys.Common.Extensions;

namespace Meekys.OData.Expressions
{
    public class FilterExpressionException : Exception
    {
        public FilterExpressionException(string message)
            : base(message)
        {
        }
    }
}