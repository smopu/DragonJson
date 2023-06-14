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
