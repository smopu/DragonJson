using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;

namespace DragonJson.ArrayCollection
{
    public unsafe class MulticastDelegateWrapper
    {

        /// <summary>
        /// 方法所属类
        /// </summary>
        public Type type;

        /// <summary>
        /// 方法名称
        /// </summary>
        public string name;

        /// <summary>
        /// 委托的输入参数，输入参数就能确认委托，所以不需要返回值
        /// </summary>
        public Type[] args;

        /// <summary>
        /// 委托的实例调用对象，静态为null
        /// </summary>
        public object target;

        ///// <summary>
        ///// 委托的具体类型
        ///// </summary>
        //public Type delegateType;
        //"testDelegate2": [
        //    {
        //        "type": "DogJsonTest,DogJsonTest.Read.JsonReadTestClassA+TClassA",
        //        "name": "Fool",
        //        "arg": [ ],
        //        "target": null
    }

    public unsafe class MulticastDelegateListWrapper
    {
        //public Delegate now;
        public Type delegateType;
        public MulticastDelegateWrapper[] allDelegate;
    }



    [ReadCollection(typeof(MulticastDelegate), true, true)]
    public unsafe class CollectionArrayMulticastDelegate : CreateTaget<ReadCollectionLink>
    {
        CollectionManager.TypeAllCollection collection;
        public ReadCollectionLink Create()
        {
            collection = CollectionManager.GetTypeCollection(typeof(MulticastDelegateWrapper));
            ReadCollectionLink read = new ReadCollectionLink();
            read.isLaze = true;

            read.addObjectClassDelegate = (Action<MulticastDelegateListWrapper, MulticastDelegateWrapper, ReadCollectionLink.Add_Args>)AddObjectClass;
            read.createObject = CreateObject;
            read.getItemType = GetItemType;
            read.endDelegate = (Func<MulticastDelegateListWrapper, object, Delegate>)End;
            return read;
        }

        void AddObjectClass(MulticastDelegateListWrapper wrapper, MulticastDelegateWrapper value, ReadCollectionLink.Add_Args arg)
        {
            wrapper.allDelegate[arg.bridge->arrayIndex] = value;
        }

        object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
        {
            temp = null;
            MulticastDelegateListWrapper wrapper = new MulticastDelegateListWrapper();
            wrapper.delegateType = arg.objectType;
            wrapper.allDelegate = new MulticastDelegateWrapper[arg.bridge->arrayCount];
            return wrapper;
        }

        CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            return collection;
        }

        Delegate End(MulticastDelegateListWrapper wrapper, object temp)
        {
            Delegate[] delegates = new Delegate[wrapper.allDelegate.Length];
            for (int i = 0; i < delegates.Length; i++)
            {
                MulticastDelegateWrapper item = wrapper.allDelegate[i];

                MethodInfo method = item.type.GetMethod(
                    item.name,
                    BindingFlags.NonPublic | BindingFlags.Instance
                    | BindingFlags.Public | BindingFlags.Static,
                       null, item.args, null);

                if (item.target == null)
                {
                    delegates[i] = method.CreateDelegate(wrapper.delegateType);
                }
                else
                {
                    delegates[i] = method.CreateDelegate(wrapper.delegateType, item.target);
                }
            }
            Delegate nowDelegate = Delegate.Combine(delegates);
            return nowDelegate;
        }

    }



    //[CollectionRead(typeof(MulticastDelegate), true, true)]
    //public unsafe class CollectionArrayMulticastDelegate2 : CollectionArrayBase<Delegate, MulticastDelegateListWrapper>
    //{
    //    public CollectionArrayMulticastDelegate2()
    //    {
    //    }
    //    protected override void Add(MulticastDelegateListWrapper wrapper, int index, object value, ReadCollectionProxy proxy)
    //    {
    //        MulticastDelegateWrapper b = (MulticastDelegateWrapper)value;
    //        wrapper.allDelegate[index] = b;
    //        //if (wrapper.now == null)
    //        //{
    //        //    wrapper.now = b;
    //        //}
    //        //else
    //        //{
    //        //    wrapper.now = MulticastDelegate.Combine(wrapper.now, b);
    //        //}
    //    }

    //    protected override MulticastDelegateListWrapper CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
    //    {
    //        MulticastDelegateListWrapper wrapper = new MulticastDelegateListWrapper();
    //        wrapper.delegateType = arrayType; 
    //        wrapper.allDelegate = new MulticastDelegateWrapper[arrayCount]; 
    //        return wrapper;
    //    }

    //    public override Type GetItemType(int index)
    //    {
    //        return typeof(MulticastDelegateWrapper);
    //    }

    //    protected override Delegate End(MulticastDelegateListWrapper wrapper)
    //    {
    //        Delegate[] delegates = new Delegate[wrapper.allDelegate.Length];
    //        for (int i = 0; i < delegates.Length; i++)
    //        {
    //            MulticastDelegateWrapper item = wrapper.allDelegate[i];

    //            MethodInfo method = item.type.GetMethod(
    //                item.name,
    //                BindingFlags.NonPublic | BindingFlags.Instance
    //                | BindingFlags.Public | BindingFlags.Static,
    //                   null, item.args, null);

    //            if (item.target == null)
    //            {
    //                delegates[i] = method.CreateDelegate(wrapper.delegateType);
    //            }
    //            else
    //            {
    //                delegates[i] = method.CreateDelegate(wrapper.delegateType, item.target);
    //            }
    //        }
    //        Delegate nowDelegate = MulticastDelegate.Combine(delegates);
    //        return nowDelegate;
    //    }

    //    protected override unsafe void AddValue(MulticastDelegateListWrapper obj, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}



    //[CollectionRead(typeof(MulticastDelegateWrapper), true)]
    //public unsafe class CollectionArrayDelegate : CollectionArrayBase<Delegate, MulticastDelegateWrapper>
    //{
    //    public CollectionArrayDelegate()
    //    {
    //    }

    //    protected override unsafe void AddValue(MulticastDelegateWrapper wrapper, int index, char* str, JsonValue* value)
    //    {
    //        switch (index)
    //        {
    //            case 0:
    //                wrapper.type = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
    //                break;
    //            case 1:
    //                if (wrapper.arrayCount > 2)
    //                {
    //                    wrapper.name = new string(str, value->vStringStart, value->vStringLength);
    //                }
    //                else
    //                {
    //                    wrapper.method = wrapper.type.GetMethod(
    //                        new string(str, value->vStringStart, value->vStringLength),
    //                        BindingFlags.NonPublic | BindingFlags.Instance
    //                        | BindingFlags.Public | BindingFlags.Static);
    //                }
    //                break;
    //            default:
    //                wrapper.args[index - 2] = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
    //                break;
    //        }
    //    }

    //    protected override MulticastDelegateWrapper CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
    //    {
    //        MulticastDelegateWrapper wrapper = new MulticastDelegateWrapper();
    //        wrapper.arrayCount = arrayCount;
    //        wrapper.delegateType = parentType;
    //        if (arrayCount > 2)
    //        {
    //            wrapper.args = new Type[arrayCount - 2];
    //        }
    //        return wrapper;
    //    }

    //    public override Type GetItemType(int index)
    //    {
    //        return typeof(string);
    //    }

    //    protected override Delegate End(MulticastDelegateWrapper wrapper)
    //    {
    //        if (wrapper.arrayCount > 2)
    //        {
    //            wrapper.method = wrapper.type.GetMethod(wrapper.name,
    //                BindingFlags.NonPublic | BindingFlags.Instance
    //                | BindingFlags.Public | BindingFlags.Static,
    //                null,
    //               wrapper.args,
    //               null
    //             );
    //        }
    //        return wrapper.method.CreateDelegate(wrapper.delegateType);
    //    }

    //    protected override void Add(MulticastDelegateWrapper obj, int index, object value)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public unsafe class DelegateWrapper
    {
        public Delegate now;
    }

    [CollectionWrite(typeof(MulticastDelegate), true)]
    public unsafe class MulticastDelegateWriter : IWriterCollectionObject, IWriterCollectionObjectIsCopy
    {
        public bool IsCopy(object obj) 
        {
            return false;
        }
        public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            MulticastDelegate collection = (MulticastDelegate)obj;
            foreach (var item in collection.GetInvocationList())
            {
                var method = item.Method;
                ParameterInfo[] parameters = method.GetParameters();
                Type[] args = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].ParameterType;
                }

                yield return new KeyValueStruct()
                {
                    value = new MulticastDelegateWrapper() {
                        type = method.DeclaringType,
                        name = method.Name,
                        target = item.Target,
                        args = args
                    },
                    type = typeof(MulticastDelegateWrapper),
                };
            }
        }

    }




}
