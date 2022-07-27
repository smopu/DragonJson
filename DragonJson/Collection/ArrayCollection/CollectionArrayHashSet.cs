using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJson
{
    [ReadCollection(typeof(HashSet<>), true)]
    public unsafe class CollectionArrayHashSet<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public CollectionArrayHashSet()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            if (typeof(T).IsValueType)
            {
                read.addObjectClassDelegate = (Action<HashSet<T>, Box<T>, ReadCollectionLink.Add_Args>)AddStruct;
            }
            else
            {
                read.addObjectClassDelegate = (Action<HashSet<T>, T, ReadCollectionLink.Add_Args>)Add;
            }

            read.addValueClassDelegate = (Action<HashSet<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            return read;
        }


        void Add(HashSet<T> array, T value, ReadCollectionLink.Add_Args arg)
        {
            array.Add(value);
        }
        void AddStruct(HashSet<T> array, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            array.Add(value.value);
        }

        void AddValue(HashSet<T> array, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value, typeof(T));
            array.Add((T)set_value);
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new HashSet<T>();
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(T));
            }
            return collection;
        }

    }


}
