using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    [ReadCollection(typeof(LinkedList<>), true)]
    public unsafe class CollectionArrayLinkedList<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public CollectionArrayLinkedList()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            if (typeof(T).IsValueType)
            {
                read.addObjectClassDelegate = (Action<LinkedList<T>, Box<T>, ReadCollectionLink.Add_Args>)AddStruct;
            }
            else
            {
                read.addObjectClassDelegate = (Action<LinkedList<T>, T, ReadCollectionLink.Add_Args>)Add;
            }

            read.addValueClassDelegate = (Action<LinkedList<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            return read;
        }


        void Add(LinkedList<T> array, T value, ReadCollectionLink.Add_Args arg)
        {
            array.AddLast(value);
        }
        void AddStruct(LinkedList<T> array, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            array.AddLast(value.value);
        }

        void AddValue(LinkedList<T> array, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value);
            array.AddLast((T)set_value);
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new LinkedList<T>();
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
