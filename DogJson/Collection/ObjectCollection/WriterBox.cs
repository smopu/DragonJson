using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection.ArrayCollection.ObjectCollection
{
    [CollectionWriteAttribute(typeof(Box<>))]
    public unsafe class WriterBox<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType() { return JsonWriteType.Object; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = ((Box<T>)obj);
            yield return new KeyValueStruct() { key = "$type", value = typeof(T), type = typeof(Type) };
            yield return new KeyValueStruct() { key = "$value", value = collection.value, type = typeof(T) };
        }
    }
}
