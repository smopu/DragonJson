using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    [CollectionRead(typeof(Queue<>), true)]
    public unsafe class CollectionArrayQueue<T> : CollectionArrayBase<Queue<T>, Queue<T>>
    {
        TypeCode typeCode;
        public CollectionArrayQueue()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        protected override void Add(Queue<T> array, int index, object value, ReadCollectionProxy proxy)
        {
            array.Enqueue((T)value);
        }

        protected override void AddValue(Queue<T> array, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            object set_value = proxy.callGetValue(typeCode, str, value);
            array.Enqueue((T)set_value);
        }

        protected override Queue<T> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new Queue<T>(arrayCount);
        }

        public override Type GetItemType(int index)
        {
            return typeof(T);
        }

        protected override Queue<T> End(Queue<T> obj)
        {
            return obj;
        }
    }


}
