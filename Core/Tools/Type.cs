using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    public static class Type
    {
        public static bool TypeIsChildOf<TParent>(System.Type childType) where TParent : class
        {
            System.Type parent = childType.GetTypeInfo().BaseType;
            if (parent == null) return false;
            if (parent == typeof(TParent)) return true;
            return TypeIsChildOf<TParent>(parent);
        }


    }
}
