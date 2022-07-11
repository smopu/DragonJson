using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    [CollectionRead(typeof(List<>), true)]
    public unsafe class CollectionArrayList<T> : CollectionArrayBase<List<T>, List<T>>
    {
        TypeCode typeCode;
        public CollectionArrayList()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        protected override void Add(List<T> array, int index, object value, ReadCollectionProxy proxy)
        {

            array.Add((T)value);
        }

        protected override void AddValue(List<T> array, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            object set_value = proxy.callGetValue(typeCode, str, value);
            array.Add((T)set_value);
        }

        protected override List<T> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new List<T>(arrayCount);
        }

        public override Type GetItemType(int index)
        {
            return typeof(T);
        }

        protected override List<T> End(List<T> obj)
        {
            return obj;
        }
    }


}
