using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public class Box<T>
    {
        public T value;
        public Box(T value) {
            this.value = value;
        }
        public Box()
        {
        }
    }


    [Collection(typeof(Box<>), false)]
    public unsafe class BoxCollection<T> : CollectionObjectBase<T, Box<T>>
    {
        TypeCode typeCode;
        public BoxCollection()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }
        
        public override unsafe Type GetItemType(JsonObject* bridge)
        {
            return typeof(T);
        }

        protected override unsafe void Add(Box<T> obj, char* key, int keyLength, object value)
        {
            obj.value = (T)value;
        }

        protected override unsafe void AddValue(Box<T> obj, char* key, int keyLength, char* str, JsonValue* value)
        {
            obj.value = (T) GetValue(typeCode, str, value);
        }

        protected override unsafe Box<T> CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return new Box<T>();
        }

        protected override T End(Box<T> obj)
        {
            return obj.value;
        }
    }
}
