using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Meekys.OData.Expressions
{
    public interface IFunctionSource
    {
        MemberInfo Find(string functionName, Type[] argumentTypes);
    }
}