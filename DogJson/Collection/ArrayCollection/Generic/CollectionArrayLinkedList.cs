using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [CollectionRead(typeof(LinkedList<>), true)]
    public unsafe class CollectionArrayLinkedList<T> : CollectionArrayBase<LinkedList<T>, LinkedList<T>>
    {
        TypeCode typeCode;
        public CollectionArrayLinkedList()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }
        protected override void Add(LinkedList<T> array, int index, object value, ReadCollectionProxy proxy)
        {
            array.AddLast((T)value);
        }

        protected override void AddValue(LinkedList<T> array, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            object set_value = proxy.callGetValue(typeCode, str, value);
            array.AddLast((T)set_value);
        }

        protected override LinkedList<T> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new LinkedList<T>();
        }

        public override Type GetItemType(int index)
        {
            return typeof(T);
        }

        protected override LinkedList<T> End(LinkedList<T> obj)
        {
            return obj;
        }
    }


}
