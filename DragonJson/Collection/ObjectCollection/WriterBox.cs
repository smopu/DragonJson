using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtrReflection;

namespace DragonJson.Collection.ArrayCollection.ObjectCollection
{
    [CollectionWriteAttribute(typeof(Box<>))]
    public unsafe class WriterBox<T> : IWriterCollectionObject, IWriterCollectionObjectIsCopy
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Object; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = ((Box<T>)obj);
            yield return new KeyValueStruct() { key = "#type", value = typeof(T), type = typeof(Type), isDontCopy = true };
            yield return new KeyValueStruct() { key = "#value", value = collection.value, type = typeof(T), isDontCopy = true };
        }

        public bool IsCopy(object obj)
        {
            return false;
        }
    }
}
