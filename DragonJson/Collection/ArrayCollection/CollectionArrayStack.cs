using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtrReflection;

namespace DragonJson
{
    [ReadCollection(typeof(Stack<>), true)]
    public unsafe class CollectionArrayStack<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public CollectionArrayStack()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = false;

            if (typeof(T).IsValueType)
            {
                read.addObjectClassDelegate = (Action<Stack<T>, Box<T>, ReadCollectionLink.Add_Args>)AddStruct;
            }
            else
            {
                read.addObjectClassDelegate = (Action<Stack<T>, T, ReadCollectionLink.Add_Args>)Add;
            }

            read.addValueClassDelegate = (Action<Stack<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            //read.endDelegate = (Action<Stack<T>, T[]>)EndObject;
            return read;
        }


        void Add(Stack<T> obj, T value, ReadCollectionLink.Add_Args arg)
        {
            obj.Push(value);
            //T[] array = (T[])arg.temp;
            //array[arg.bridge->arrayIndex] = value;
        }
        void AddStruct(Stack<T> obj, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            obj.Push(value.value);
            //T[] array = (T[])arg.temp;
            //array[arg.bridge->arrayIndex] = value.value;
        }

        void AddValue(Stack<T> obj, ReadCollectionLink.AddValue_Args arg)
        {
            object set_value = arg.callGetValue(typeCode, arg.str, arg.value, typeof(T));
            obj.Push((T)set_value);
            //T[] array = (T[])arg.temp;
            //array[arg.value->arrayIndex] = (T)set_value;
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            //temp = new T[arg.bridge->arrayCount];
            return new Stack<T>(arg.bridge->arrayCount);
        }

        //void EndObject(Stack<T> obj, T[] temp)
        //{
        //    for (int i = 0; i < temp.Length; i++)
        //    {
        //        obj.Push(temp[i]);
        //    }
        //}


        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(T));
            }
            return collection;
        }

    }



    //[ReadCollection(typeof(Stack<>), true)]
    //public unsafe class CollectionArrayStack2<T> : CollectionArrayBase<Stack<T>, T[]>
    //{
    //    TypeCode typeCode;
    //    public CollectionArrayStack2()
    //    {
    //        typeCode = Type.GetTypeCode(typeof(T));
    //    }

    //    protected override Stack<T> End(T[] array)
    //    {
    //        return new Stack<T>(array);
    //    }

    //    protected override void Add(T[] array, int index, object value, ReadCollectionProxy proxy)
    //    {
    //        array[index] = (T)value;
    //    }

    //    protected override void AddValue(T[] array, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
    //    {
    //        object set_value = proxy.callGetValue(typeCode, str, value);
    //        array[index] = (T)set_value;
    //    }

    //    protected override T[] CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
    //    {
    //        return new T[arrayCount];
    //    }

    //    public override Type GetItemType(int index)
    //    {
    //        return typeof(T);
    //    }
    //}


}
