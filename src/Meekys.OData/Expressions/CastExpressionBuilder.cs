using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Meekys.Common.Extensions;

namespace Meekys.OData.Expressions
{
    public class CastExpressionBuilder
    {
        public static Expression Build<TCastTo>(Expression input)
        {
            return Build(typeof(TCastTo), input);
        }

        public static Expression Build(Type castTo, Expression input)
        {
            // If it's the same type, nothing needs to be done
            if (input.Type == castTo)
                return input;

            var constant = input as ConstantExpression;

            if (constant == null)
                return Expression.Convert(input, castTo); // If it's a property/method/etc, convert it

            // If it's a constant, convert it inline
            return Expression.Constant(Convert.ChangeType(constant.Value, Nullable.GetUnderlyingType(castTo) ?? castTo), castTo);
        }
    }
}