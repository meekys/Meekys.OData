using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public interface IFunctionSource
    {
        MemberInfo Find(string functionName, Type[] argumentTypes);
    }
}