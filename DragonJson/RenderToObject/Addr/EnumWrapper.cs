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
        public EnumWrapper(object inEnum)
        {
            this.inEnum = inEnum;
        }
        public object inEnum;
    }



}
