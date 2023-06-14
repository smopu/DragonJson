using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson.Collection.ArrayCollection
{
    [CollectionWriteAttribute(typeof(List<>))]
    [CollectionWriteAttribute(typeof(Queue<>))]
    [CollectionWriteAttribute(typeof(HashSet<>))]
    [CollectionWriteAttribute(typeof(LinkedList<>))] 
    public unsafe class WriterEnumerable<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = (IEnumerable<T>)obj;
            foreach (var item in collection)
            {
                yield return new KeyValueStruct() { value = item, type = typeof(T) };
            }
        }
    }

    [CollectionWriteAttribute(typeof(Stack<>))]
    public unsafe class WriterStack<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = ((Stack<T>)obj).ToArray();
            for (int i = collection.Length - 1; i >= 0; i--)
            {
                yield return new KeyValueStruct() { value = collection[i], type = typeof(T) };
            }
        }
    }

    [CollectionWriteAttribute(typeof(EnumWrapper))]
    public unsafe class WriterEnum : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType(object obj) {

            EnumWrapper obj2 = (EnumWrapper)obj;
            EnumTypeWrap enumTypeWrap = CollectionManager.GetEnumTypeWrap(obj2.type);
            long source;
            List<string> vs = enumTypeWrap.GetValue(obj2.inEnum, out source);

            if (vs.Count == 0)
            {
                return JsonWriteType.Value;
            }
            else
            {
                return JsonWriteType.Array;
            }
        }

        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            EnumWrapper obj2 = (EnumWrapper)obj;
            EnumTypeWrap enumTypeWrap = CollectionManager.GetEnumTypeWrap(obj2.type);
            long source;
            List<string> vs = enumTypeWrap.GetValue(obj2.inEnum, out source);
            if (vs.Count == 0)
            {
                yield return new KeyValueStruct() { key = source.ToString(), type = typeof(string) };
            }
            foreach (var item in vs)
            {
                yield return new KeyValueStruct() { value = item, type = typeof(string) };
            }
        }


    }





}
