using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common;
using Meekys.Common.Extensions;

namespace Meekys.OData.Expressions
{
    [SuppressMessage("Microsoft.CodeAnalysis.Analyzers", "CA1032:ImplementStandardExceptionConstructors", Justification = "Internally raised exception")]
    public class FilterExpressionException : Exception
    {
        public FilterExpressionException(string message)
            : base(message)
        {
        }
    }
}