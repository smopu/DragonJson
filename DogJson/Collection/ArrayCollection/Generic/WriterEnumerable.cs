using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection.Write.ArrayCollection
{

    [CollectionWriteAttribute(typeof(List<>))]
    [CollectionWriteAttribute(typeof(Queue<>))]
    [CollectionWriteAttribute(typeof(HashSet<>))]
    [CollectionWriteAttribute(typeof(LinkedList<>))]
    public unsafe class WriterEnumerable<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType() { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = (IEnumerable<T>)obj;
            foreach (var item in collection)
            {
                yield return new KeyValueStruct() { value = item, type = typeof(T) };
            }
        }
    }


}
