using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
    public unsafe abstract class StringCollectionBase
    {
        public abstract object GetObject(string obj, int index, object value);
        public abstract object GetObject(int index, char* str, JsonValue value);
        public abstract string ObjectToString(object obj);
    }

}
