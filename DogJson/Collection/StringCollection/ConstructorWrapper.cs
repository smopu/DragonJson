using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    public class ConstructorWrapper<T>
    {
        //public Type[] types;
        public object[] args;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    [ReadCollection(typeof(ConstructorWrapper<>), false)]
    public unsafe class ConstructorWrapperCollection<T> : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = true;

            if (typeof(T).IsValueType)
            {
                read.endDelegate = (End_)End;
                read.createValueDelegate = (CreateValue_)CreateValue;
            }
            else
            {
                
                read.endDelegate = (EndObject_)EndObject;
                read.createObjectDelegate = (CreateObject_)CreateObject;
            }

            read.addObjectStructDelegate = (AddObjectClass_)AddObjectClass;
            read.addObjectClassDelegate = (AddObjectClass_)AddObjectClass;


            read.getItemType = GetItemType;
            //read.endDelegate = (Action<Stack<T>, T[]>)EndObject;
            return read;
        }


        long typelong;
        long argslong;
        public ConstructorWrapperCollection()
        {
            fixed (char* vs = "type")
            {
                typelong = *(long*)vs;
            }
            fixed (char* vs = "args")
            {
                argslong = *(long*)vs;
            }
        }

        delegate void AddObjectClass_(object obj, object[] value, ReadCollectionLink.Add_Args arg);
        void AddObjectClass(object obj, object[] value, ReadCollectionLink.Add_Args arg)
        {
            //string keyName = new string(arg.bridge->keyStringStart, 0, arg.bridge->keyStringLength);//$create
            //if (keyName == "$create")
            //{
            //}
            ConstructorWrapper<T> wrapper = (ConstructorWrapper<T>)arg.temp;
            wrapper.args = (object[])value;
        }

        delegate void CreateValue_(ref T value, out object temp, ReadCollectionLink.Create_Args arg);
        void CreateValue(ref T value, out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = new ConstructorWrapper<T>();
            value = (T)FormatterServices.GetUninitializedObject(typeof(T));
        }

        delegate object CreateObject_(out object temp, ReadCollectionLink.Create_Args arg);
        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = new ConstructorWrapper<T>();
            return FormatterServices.GetUninitializedObject(typeof(T));
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            if (collection == null)
            {
                collection = CollectionManager.GetTypeCollection(typeof(object[]));
            }
            return collection;
        }

        delegate object EndObject_(object obj, ConstructorWrapper<T> temp);
        object EndObject(object obj, ConstructorWrapper<T> temp)
        {
            var types = new Type[temp.args.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = temp.args[i].GetType();
            }
            ConstructorInfo constructor = typeof(T).GetConstructor(types);
            constructor.Invoke(obj, temp.args);
            return obj;
        }

        delegate void End_(ref T value, ConstructorWrapper<T> temp);
        void End(ref T value, ConstructorWrapper<T> temp)
        {
            //var types = new Type[temp.args.Length];
            //for (int i = 0; i < types.Length; i++)
            //{
            //    types[i] = temp.args[i].GetType();
            //}
            value = (T)Activator.CreateInstance(typeof(T), temp.args);
        }



    }


}
