using System;
using System.Collections.Generic;
using System.Linq;

namespace Meekys.OData.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class KeyAttribute : Attribute
    {
    }
}