using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;

namespace DogJson.ArrayCollection
{
    public unsafe class MulticastDelegateWrapper
    {
        public Delegate value;
        public Type type;
        public MethodInfo method;
        public string methodName;
        public int arrayCount;
        public Type delegateType;
        public Type[] args;
    }

    public unsafe class MulticastDelegateListWrapper
    {
        public Delegate now;
        public Type type;
    }

    [Collection(typeof(MulticastDelegate), true)]
    public unsafe class CollectionArrayMulticastDelegate : CollectionArrayBase<Delegate, MulticastDelegateListWrapper>
    {
        public CollectionArrayMulticastDelegate()
        {
        }
        protected override void Add(MulticastDelegateListWrapper wrapper, int index, object value)
        {
            Delegate b = (Delegate)value;
            if (wrapper.now == null)
            {
                wrapper.now = b;
            }
            else
            {
                wrapper.now = MulticastDelegate.Combine(wrapper.now, b);
            }
        }

        protected override MulticastDelegateListWrapper CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            MulticastDelegateListWrapper wrapper = new MulticastDelegateListWrapper();
            wrapper.type = arrayType;
            return wrapper;
        }

        public override Type GetItemType(int index)
        {
            return typeof(MulticastDelegateWrapper);
        }
        protected override Delegate End(MulticastDelegateListWrapper wrapper)
        {
            return wrapper.now;
        }

        protected override unsafe void AddValue(MulticastDelegateListWrapper obj, int index, char* str, JsonValue* value)
        {
            throw new NotImplementedException();
        }
    }



    [Collection(typeof(MulticastDelegateWrapper), true)]
    public unsafe class CollectionArrayDelegate : CollectionArrayBase<Delegate, MulticastDelegateWrapper>
    {
        public CollectionArrayDelegate()
        {
        }

        protected override unsafe void AddValue(MulticastDelegateWrapper wrapper, int index, char* str, JsonValue* value)
        {
            switch (index)
            {
                case 0:
                    wrapper.type = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
                    break;
                case 1:
                    if (wrapper.arrayCount > 2)
                    {
                        wrapper.methodName = new string(str, value->vStringStart, value->vStringLength);
                    }
                    else
                    {
                        wrapper.method = wrapper.type.GetMethod(
                            new string(str, value->vStringStart, value->vStringLength),
                            BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.Public | BindingFlags.Static);
                    }
                    break;
                default:
                    wrapper.args[index - 2] = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
                    break;
            }
        }

        protected override MulticastDelegateWrapper CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            MulticastDelegateWrapper wrapper = new MulticastDelegateWrapper();
            wrapper.arrayCount = arrayCount;
            wrapper.delegateType = parentType;
            if (arrayCount > 2)
            {
                wrapper.args = new Type[arrayCount - 2];
            }
            return wrapper;
        }

        public override Type GetItemType(int index)
        {
            return typeof(string);
        }

        protected override Delegate End(MulticastDelegateWrapper wrapper)
        {
            if (wrapper.arrayCount > 2)
            {
                wrapper.method = wrapper.type.GetMethod(wrapper.methodName,
                    BindingFlags.NonPublic | BindingFlags.Instance
                    | BindingFlags.Public | BindingFlags.Static,
                    null,
                   wrapper.args,
                   null
                 );
            }
            return wrapper.method.CreateDelegate(wrapper.delegateType);
        }

        protected override void Add(MulticastDelegateWrapper obj, int index, object value)
        {
            throw new NotImplementedException();
        }
    }


}
