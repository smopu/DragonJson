//#define NEW_Dictionary
using DragonJson.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
#if NEW_Dictionary
    public struct DictionaryKV<K, V>
    {
        public K k;
        public V v;
    }

    [Collection(typeof(Dictionary<,>), true)]
    public class CollectionArrayDictionary<K, V> : CollectionArrayBase<Dictionary<K, V>, DictionaryKV<K, V>[]>
    {
        TypeCode typeCodeK;
        TypeCode typeCodeV;
        public CollectionArrayDictionary()
        {
            typeCodeK = Type.GetTypeCode(typeof(K));
            typeCodeV = Type.GetTypeCode(typeof(V));
        }
        public override bool IsRef()
        {
            return false;
        }
        protected override void Add(DictionaryKV<K, V>[] dict, int index, object value)
        {
            if (index % 2 == 0)
            {
                dict[index / 2].k = (K)value;
            }
            else
            {
                dict[index / 2].v = (V)value;
            }
        }
        protected override DictionaryKV<K, V>[] CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new DictionaryKV<K, V>[arrayCount / 2];
        }
        protected override Dictionary<K, V> End(DictionaryKV<K, V>[] obj)
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            ; for (int i = 0; i < obj.Length; i++)
            {
                dict[obj[i].k] = obj[i].v;
            }
            return dict;
        }
        public override Type GetItemType(int index)
        {
            if (index % 2 == 0)
            {
                return typeof(K);
            }
            return typeof(V);
        }

        protected override unsafe void AddValue(DictionaryKV<K, V>[] obj, int index, char* str, JsonValue* value)
        {
            if (index % 2 == 0)
            {
                obj[index / 2].k = (K)GetValue(typeCodeK, str, value);
            }
            else
            {
                obj[index / 2].v = (V)GetValue(typeCodeK, str, value);
            }
        }
    }
#else


    public class DictionaryKV<K, V>
    {
        public K k;
        public V v;
    }

    [ReadCollection(typeof(DictionString<,>), false, true)]
    public unsafe class CollectionArrayDictionString<V> : CreateTaget<ReadCollectionLink>
    {
        TypeCode typeCodeV;
        public CollectionArrayDictionString()
        {
            typeCodeV = Type.GetTypeCode(typeof(V));
            if (typeCodeV == TypeCode.Object)
            {
                collection = CollectionManager.GetTypeCollection(typeof(V));
            }
        }

        CollectionManager.TypeAllCollection collection;
        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            read.createObject = CreateObject;

            Action<Dictionary<string, V>, object, ReadCollectionLink.Add_Args> ac = Add;
            read.addObjectClassDelegate = ac;

            Action<Dictionary<string, V>, ReadCollectionLink.AddValue_Args> ac2 = AddValue;
            read.addValueClassDelegate = ac2;

            read.getItemType = GetItemType;
            return read;
        }

        static void Add(Dictionary<string, V> obj, object value, ReadCollectionLink.Add_Args arg)
        {
            obj[new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength)] = (V)value;
        }

        void AddValue(Dictionary<string, V> obj, ReadCollectionLink.AddValue_Args arg)
        {
            obj[new string(arg.str, arg.value->keyStringStart, arg.value->keyStringLength)] = (V)arg.callGetValue(typeCodeV, arg.str, arg.value, typeof(V));
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new Dictionary<string, V>();
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            return collection;
        }

    }

    [ReadCollection(typeof(DictionaryKV<,>), true)]
    public unsafe class CollectionArrayDictionaryKV<K, V> : CreateTaget<ReadCollectionLink>
    {
        TypeCode typeCodeK;
        TypeCode typeCodeV;
        public CollectionArrayDictionaryKV()
        {
            typeCodeK = Type.GetTypeCode(typeof(K));
            typeCodeV = Type.GetTypeCode(typeof(V));
            //if (typeCodeK == TypeCode.Object && typeCodeK != TypeCode.String)
            //{
            //    collectionK = CollectionManager.GetTypeCollection(typeof(K));
            //}
            //if (typeCodeV == TypeCode.Object && typeCodeK != TypeCode.String)
            //{
            //    collectionV = CollectionManager.GetTypeCollection(typeof(V));
            //}
        }

        CollectionManager.TypeAllCollection collectionK;
        CollectionManager.TypeAllCollection collectionV;
        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            Action<DictionaryKV<K, V>, object, ReadCollectionLink.Add_Args> ac = Add;
            read.addObjectClassDelegate = ac;

            Action<DictionaryKV<K, V>, ReadCollectionLink.AddValue_Args> ac2 = AddValue;
            read.addValueClassDelegate = ac2;


            read.createObject = CreateObject;

            read.getItemType = GetItemType;
            return read;
        }


        static void Add(DictionaryKV<K, V> kv, object value, ReadCollectionLink.Add_Args arg)
        {
            if (arg.bridge->arrayIndex == 0)
            {
                kv.k = (K)value;
            }
            else
            {
                kv.v = (V)value;
            }
        }

        void AddValue(DictionaryKV<K, V> kv, ReadCollectionLink.AddValue_Args arg)
        {
            int index = arg.value->arrayIndex;
            TypeCode typeCode;
            if (index == 0)
            {
                typeCode = typeCodeK;
            }
            else
            {
                typeCode = typeCodeV;
            }

            if (index == 0)
            {
                kv.k = (K)arg.callGetValue(typeCode, arg.str, arg.value, typeof(K));
            }
            else
            {
                kv.v = (V)arg.callGetValue(typeCode, arg.str, arg.value, typeof(V));
            }
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new DictionaryKV<K, V>();
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            int index = arg.bridge->arrayIndex;
            if (index == 0)
            {
                if (collectionK == null)
                {
                    collectionK = CollectionManager.GetTypeCollection(typeof(K));
                }
                return collectionK;
            }
            if (collectionV == null)
            {
                collectionV = CollectionManager.GetTypeCollection(typeof(V));
            }
            return collectionV;
        }
    }


    [ReadCollection(typeof(Dictionary<,>), true)]
    public unsafe class DictionaryReader<K, V> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        public ReadCollectionLink Create()
        {
            collection =  CollectionManager.GetTypeCollection(typeof(DictionaryKV<K, V>));
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            Action<Dictionary<K, V>, DictionaryKV<K, V>, ReadCollectionLink.Add_Args> ac = Add;
            read.addObjectClassDelegate = ac;

            read.createObject = CreateObject;

            read.getItemType = GetItemType;


            return read;
        }



        void Add(Dictionary<K, V> dict, DictionaryKV<K, V> kv, ReadCollectionLink.Add_Args arg)
        {
            dict[kv.k] = kv.v;
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new Dictionary<K, V>(arg.bridge->arrayCount);
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            return collection;
        }

    }



#endif

}
