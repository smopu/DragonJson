using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [ReadCollection(typeof(List<>), true)] 
    public unsafe class CollectionArrayList<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public CollectionArrayList()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            if (typeof(T).IsValueType)
            {
                read.addObjectClassDelegate = (Action<List<T>, Box<T>, ReadCollectionLink.Add_Args>)AddStruct;
            }
            else
            {
                read.addObjectClassDelegate = (Action<List<T>, T, ReadCollectionLink.Add_Args>)Add;
            }

            read.addValueClassDelegate = (Action<List<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            //read.addValueStructDelegate = (Action<List<T>, ReadCollectionLink.AddValue_Args>)AddValue;

            
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            return read;
        }


        void Add(List<T> array, T value, ReadCollectionLink.Add_Args arg)
        {
            array.Add(value);
        }
        void AddStruct(List<T> array, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            array.Add(value.value);
        }

        void AddValue(List<T> array, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value);
            array.Add((T)set_value);
        }
        void ValueStruct(List<T> array, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value);
            array.Add((T)set_value);
        }
        
        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            return new List<T>(arg.bridge->arrayCount);
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
