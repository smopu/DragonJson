using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson.Collection.ArrayCollection.Generic
{
    //[CollectionWriteAttribute(typeof(RuntimeType))]
    [CollectionWriteStringAttribute(typeof(System.Reflection.TypeInfo), true)]
    [CollectionWriteStringAttribute(typeof(Type), true)] 
    public unsafe class WriterType : IWriterCollectionString
    {
        public string GetStringValue(object obj)
        {
            Type type = (Type)obj;
            return UnsafeOperation.TypeToString(type);
        }
    }
}
