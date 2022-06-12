using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;

namespace DogJson
{
    public unsafe class MulticastDelegateWarp
    {
        public Delegate value;
        public Type type;
        public MethodInfo method;
        public string methodName;
        public int arrayCount;
        public Type delegateType;


        public Type[] args;
    }
    public unsafe class MulticastDelegateListWarp
    {
        public Delegate now;
        public Type type;
    }

    [Collection(typeof(MulticastDelegate), true)]
    public unsafe class CollectionArrayMulticastDelegate : CollectionArrayBase<Delegate, MulticastDelegateListWarp>
    {
        public CollectionArrayMulticastDelegate()
        {
        }
        protected override void Add(MulticastDelegateListWarp warp, int index, object value)
        {
            Delegate b = (Delegate)value;
            if (warp.now == null)
            {
                warp.now = b;
            }
            else
            {
                warp.now = MulticastDelegate.Combine(warp.now, b);
            }
        }

        protected override MulticastDelegateListWarp CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            MulticastDelegateListWarp warp = new MulticastDelegateListWarp();
            warp.type = arrayType;
            return warp;
        }

        public override Type GetItemType(int index)
        {
            return typeof(MulticastDelegateWarp);
        }
        protected override Delegate End(MulticastDelegateListWarp warp)
        {
            return warp.now;
        }

        protected override unsafe void AddValue(MulticastDelegateListWarp obj, int index, char* str, JsonValue* value)
        {
            throw new NotImplementedException();
        }
    }



    [Collection(typeof(MulticastDelegateWarp), true)]
    public unsafe class CollectionArrayDelegate : CollectionArrayBase<Delegate, MulticastDelegateWarp>
    {
        public CollectionArrayDelegate()
        {
        }

        protected override unsafe void AddValue(MulticastDelegateWarp warp, int index, char* str, JsonValue* value)
        {
            switch (index)
            {
                case 0:
                    warp.type = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
                    break;
                case 1:
                    if (warp.arrayCount > 2)
                    {
                        warp.methodName = new string(str, value->vStringStart, value->vStringLength);
                    }
                    else
                    {
                        warp.method = warp.type.GetMethod(
                            new string(str, value->vStringStart, value->vStringLength),
                            BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.Public | BindingFlags.Static);
                    }
                    break;
                default:
                    warp.args[index - 2] = UnsafeOperation.GetType(new string(str, value->vStringStart, value->vStringLength));
                    break;
            }
        }

        protected override MulticastDelegateWarp CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            MulticastDelegateWarp warp = new MulticastDelegateWarp();
            warp.arrayCount = arrayCount;
            warp.delegateType = parentType;
            if (arrayCount > 2)
            {
                warp.args = new Type[arrayCount - 2];
            }
            return warp;
        }

        public override Type GetItemType(int index)
        {
            return typeof(string);
        }

        protected override Delegate End(MulticastDelegateWarp warp)
        {
            if (warp.arrayCount > 2)
            {
                warp.method = warp.type.GetMethod(warp.methodName,
                    BindingFlags.NonPublic | BindingFlags.Instance
                    | BindingFlags.Public | BindingFlags.Static,
                    null,
                   warp.args,
                   null
                 );
            }
            return warp.method.CreateDelegate(warp.delegateType);
        }

        protected override void Add(MulticastDelegateWarp obj, int index, object value)
        {
            throw new NotImplementedException();
        }
    }


}
