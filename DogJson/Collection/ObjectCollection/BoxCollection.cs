using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public class Box<T> : IBox
    {
        public T value;
        public Box(T value) {
            this.value = value;
        }
        public Box()
        {
        }
        public void SetObject(object obj)
        {
            this.value = (T)obj;
        }
    }

    public interface IBox
    {
        void SetObject(object ob);
    }

    [ReadCollection(typeof(Box<>), false)]
    public unsafe class BoxCollection<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        TypeCode typeCode;
        public BoxCollection()
        {
            typeCode = Type.GetTypeCode(typeof(T));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            //if (typeof(T).IsValueType)
            //{
            //    read.addObjectDelegate = (Action<Box<T>, Box<T>, ReadCollectionLink.Add_Args>)AddObjectStruct;
            //}
            //else
            //{
            //    read.addObjectDelegate = (Action<Box<T>, T, ReadCollectionLink.Add_Args>)AddObjectClass;
            //}
            if (typeof(T) == typeof(string))
            {
                read.isLaze = true;
            }
            else
            {
                read.isLaze = false;
            }
            read.addValueClassDelegate = (Action<Box<T>, ReadCollectionLink.AddValue_Args>)AddValue;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            read.endDelegate = (EndObject_)EndObject;
            return read;
        }

        void AddObjectClass(Box<T> obj, T value, ReadCollectionLink.Add_Args arg)
        {
            obj.value = (T)value;
        }

        void AddObjectStruct(Box<T> obj, Box<T> value, ReadCollectionLink.Add_Args arg)
        {
            obj.value = value.value;
        }

        void AddValue(Box<T> obj, ReadCollectionLink.AddValue_Args arg)
        {
            obj.value = (T)arg.callGetValue(typeCode, arg.str, arg.value);
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            object data = new Box<T>();
            if (typeof(T) == typeof(string))
            {
                //var typeHead = UnsafeOperation.GetTypeHead(typeof(T));
                //*(IntPtr*)GeneralTool.ObjectToVoid(data) = typeHead;
                temp = null;
                return data;
            }
            else
            {
                var typeHead = UnsafeOperation.GetTypeHead(typeof(T));
                *(IntPtr*)GeneralTool.ObjectToVoid(data) = typeHead;
                temp = null;
                return data;
            }
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(T));
            }
            return collection;
        }

        delegate object EndObject_(Box<T> obj, object temp);
        object EndObject(Box<T> obj, object temp)
        {
            return obj.value;
        }

    }


    //[CollectionRead(typeof(Box<>), false)]
    //public unsafe class BoxCollection<T> : CollectionObjectBase<T, Box<T>>
    //{
    //    TypeCode typeCode;
    //    public BoxCollection()
    //    {
    //        typeCode = Type.GetTypeCode(typeof(T));
    //    }
        
    //    public override unsafe Type GetItemType(JsonObject* bridge)
    //    {
    //        return typeof(T);
    //    }

    //    protected override unsafe void Add(Box<T> obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
    //    {
    //        obj.value = (T)value;
    //    }

    //    protected override unsafe void AddValue(Box<T> obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
    //    {
    //        obj.value = (T)proxy.callGetValue(typeCode, str, value);
    //    }

    //    protected override unsafe Box<T> CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
    //    {
    //        return new Box<T>();
    //    }

    //    protected override T End(Box<T> obj)
    //    {
    //        return obj.value;
    //    }
    //}
}
