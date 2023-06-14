using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
    public class EnumWrapper<T>
    {
        public EnumWrapper(T inEnum)
        {
            this.inEnum = inEnum;
        }
        public T inEnum;
    }
    public class EnumWrapper
    {
        public EnumWrapper(Type type, object inEnum)
        {
            this.type = type;
            this.inEnum = inEnum;
        }
        public Type type;
        public object inEnum;
    }



}
