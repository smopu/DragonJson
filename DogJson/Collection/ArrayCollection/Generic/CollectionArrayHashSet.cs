using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [CollectionRead(typeof(HashSet<>), true)]
    public unsafe class CollectionArrayHashSet<T> : CollectionArrayBase<HashSet<T>, HashSet<T>>
    {
        TypeCode typeCode;
        public CollectionArrayHashSet()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        protected override void Add(HashSet<T> array, int index, object value, ReadCollectionProxy proxy)
        {
            array.Add((T)value);
        }

        protected override void AddValue(HashSet<T> array, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            object set_value = proxy.callGetValue(typeCode, str, value);
            array.Add((T)set_value);
        }

        protected override HashSet<T> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new HashSet<T>();
        }

        public override Type GetItemType(int index)
        {
            return typeof(T);
        }

        protected override HashSet<T> End(HashSet<T> obj)
        {
            return obj;
        }
    }


}
