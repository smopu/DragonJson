using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection
{
     
    [CollectionWrite(typeof(KeyValuePair<,>))]
    public unsafe class WriterKeyValuePair<K, V> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            KeyValuePair<K, V> collection = (KeyValuePair<K, V>)obj;

            yield return new KeyValueStruct()
            {
                value = collection.Key,
                type = typeof(K),
            };
            yield return new KeyValueStruct()
            {
                value = collection.Value,
                type = typeof(V),
            };
            //yield return collection.Key;
            //yield return collection.Value;
        }
    }


    [CollectionWrite(typeof(Dictionary<,>))]
    public unsafe class WriterDictionary<K, V> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            Dictionary<K, V> collection = (Dictionary<K, V>)obj;
            foreach (var item in collection)
            {
                yield return new KeyValueStruct() {
                    value = new KeyValuePair<K, V>(item.Key, item.Value),
                    type = typeof(System.Collections.Generic.KeyValuePair<K, V>),
                };
            }
        }
        //public Type GetItemType(int index)
        //{
        //    return typeof(System.Collections.Generic.KeyValuePair<K, V>);
        //}
    }


    [CollectionWrite(typeof(DictionString<,>))]
    public unsafe class DictionaryStringWriter<V> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Object; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            Dictionary<string, V> collection = (Dictionary<string, V>)obj;
            foreach (var item in collection)
            {
                yield return new KeyValueStruct()
                {
                    value = item.Value,
                    type = item.Value.GetType(),
                    key = item.Key,
                };
            }
        }
    }

    public class DictionString<Zero, T> : SpecialCaseGeneric where Zero : Dictionary<string, T> { }
}

