using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtrReflection;

namespace DragonJson.Collection.ArrayCollection.Generic
{
    //[CollectionWriteAttribute(typeof(RuntimeType))]
    [CollectionWriteStringAttribute(typeof(System.Reflection.TypeInfo), true)]
    [CollectionWriteStringAttribute(typeof(Type), true)] 
    public unsafe class WriterType : ICollectionString
    {
        public string ToStringValue(object obj)
        {
            Type type = (Type)obj;
            return UnsafeOperation.TypeToString(type);
        }
        public object ToObject(string str)
        {
            Type type = UnsafeOperation.GetType(str);
            return type;
        }
    }
}
