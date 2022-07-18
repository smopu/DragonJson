using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public class EnumWrap<T>
    {
        public EnumWrap(T inEnum)
        {
            this.inEnum = inEnum;
        }
        public T inEnum;
    }
    public class EnumWrap
    {
        public EnumWrap(object inEnum)
        {
            this.inEnum = inEnum;
        }
        public object inEnum;
    }



}
