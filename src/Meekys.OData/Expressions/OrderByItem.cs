using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public class OrderByItem
    {
        public LambdaExpression Expression { get; set; }

        public bool Descending { get; set; }
    }
}