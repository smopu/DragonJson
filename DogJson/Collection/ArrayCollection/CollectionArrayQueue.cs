using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    [ReadCollection(typeof(Queue<>), true)]
    public unsafe class CollectionArrayQueue<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public CollectionArrayQueue()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            if (typeof(T).IsValueType)
            {
                read.addObjectClassDelegate = (Action<Queue<T>, Box<T>, ReadCollectionLink.Add_Args>)AddStruct;
            }
            else
            {
                read.addObjectClassDelegate = (Action<Queue<T>, T, ReadCollectionLink.Add_Args>)Add;
            }

            read.addValueClassDelegate = (Action<Queue<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            return read;
        }


        void Add(Queue<T> array, T value, ReadCollectionLink.Add_Args arg)
        {
            array.Enqueue(value);
        }
        void AddStruct(Queue<T> array, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            array.Enqueue(value.value);
        }

        void AddValue(Queue<T> array, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value, typeof(T));
            array.Enqueue((T)set_value);
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new Queue<T>(arg.bridge->arrayCount);
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
