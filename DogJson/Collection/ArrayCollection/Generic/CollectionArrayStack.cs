using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [Collection(typeof(Stack<>), true)]
    public unsafe class CollectionArrayStack<T> : CollectionArrayBase<Stack<T>, T[]>
    {
        TypeCode typeCode;
        public CollectionArrayStack()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        protected override Stack<T> End(T[] array)
        {
            return new Stack<T>(array);
        }

        protected override void Add(T[] array, int index, object value)
        {
            array[index] = (T)value;
        }

        protected override void AddValue(T[] array, int index, char* str, JsonValue* value)
        {
            object set_value = GetValue(typeCode, str, value);
            array[index] = (T)set_value;
        }

        protected override T[] CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new T[arrayCount];
        }

        public override Type GetItemType(int index)
        {
            return typeof(T);
        }
    }


}
