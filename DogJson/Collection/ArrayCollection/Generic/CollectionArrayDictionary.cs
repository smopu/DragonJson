using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [Collection(typeof(DictionaryKV<,>), true)]
    public unsafe class CollectionArrayDictionaryKV<K, V> : CollectionArrayBase<DictionaryKV<K, V>, DictionaryKV<K, V>>
    {
        TypeCode typeCodeK;
        TypeCode typeCodeV;
        public CollectionArrayDictionaryKV()
        {
            typeCodeK = Type.GetTypeCode(typeof(K));
            typeCodeV = Type.GetTypeCode(typeof(V));
        }
        protected override void Add(DictionaryKV<K, V> kv, int index, object value)
        {
            if (index == 0)
            {
                kv.k = (K)value;
            }
            else
            {
                kv.v = (V)value;
            }
        }

        protected override void AddValue(DictionaryKV<K, V> kv, int index, char* str, JsonValue* value)
        {
            TypeCode typeCode;
            if (index == 0)
            {
                typeCode = typeCodeK;
            }
            else
            {
                typeCode = typeCodeV;
            }

            object set_value = GetValue(typeCode, str, value);

            if (index == 0)
            {
                kv.k = (K)set_value;
            }
            else
            {
                kv.v = (V)set_value;
            }
        }

        protected override DictionaryKV<K, V> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new DictionaryKV<K, V>();
        }
        public override Type GetItemType(int index)
        {
            if (index == 0)
            {
                return typeof(K);
            }
            return typeof(V);
        }

        protected override DictionaryKV<K, V> End(DictionaryKV<K, V> obj)
        {
            return obj;
        }
    }

    public class DictionaryKV<K, V>
    {
        public K k;
        public V v;
    }

    [Collection(typeof(Dictionary<,>), true)]
    public class CollectionArrayDictionary<K, V> : CollectionArrayBase<Dictionary<K, V>, Dictionary<K, V>>
    {
        protected override void Add(Dictionary<K, V> dict, int index, object value)
        {
            var kv = (DictionaryKV<K, V>)value;
            dict[kv.k] = kv.v;
        }
        protected override Dictionary<K, V> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new Dictionary<K, V>(arrayCount);
        }
        protected override Dictionary<K, V> End(Dictionary<K, V> obj)
        {
            return obj;
        }
        public override Type GetItemType(int index)
        {
            return typeof(DictionaryKV<K, V>);
        }

        protected override unsafe void AddValue(Dictionary<K, V> obj, int index, char* str, JsonValue* value)
        {
            throw new NotImplementedException();
        }
    }


}
