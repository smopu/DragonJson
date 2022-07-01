using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    public class ConstructorWrapper
    {
        public Type myType;
        public Type[] types;
        public object[] args;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    [Collection(typeof(ConstructorWrapper), false)]
    public unsafe class ConstructorWrapperCollection : CollectionObjectBase<object, ConstructorWrapper>
    {
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


        public override unsafe Type GetItemType(JsonObject* bridge)
        {
            return typeof(object[]);
            //if (keyLength == 4)
            //{
            //    long nameLong = *(long*)key;
            //    if (nameLong == typelong)
            //    {
            //        return typeof(Type[]);
            //    }
            //    else if (nameLong == argslong)
            //    {
            //        return typeof(object[]);
            //    }
            //}
            throw new Exception();
        }

        protected override unsafe void Add(ConstructorWrapper obj, char* key, int keyLength, object value)
        {
            string keyName = new string(key, 0, keyLength);//#create
            if (keyName == "#create")
            {
                obj.args = (object[])value;
            }
            //if (keyLength == 4)
            //{
            //    long nameLong = *(long*)key;
            //    if (nameLong == typelong)
            //    {
            //        obj.types = (Type[])value;
            //    }
            //    else if (nameLong == argslong)
            //    {
            //        obj.args = (object[])value;
            //    }
            //}
            //throw new Exception();
        }

        protected override unsafe void AddValue(ConstructorWrapper obj, char* key, int keyLength, char* str, JsonValue* value)
        {
            throw new NotImplementedException();
        }

        protected override ConstructorWrapper CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return new ConstructorWrapper()
            {
                myType = objectType
            };
        }

        protected override object End(ConstructorWrapper obj)
        {
            return Activator.CreateInstance(obj.myType, obj.args);
            //return obj.myType.GetConstructor(obj.types).Invoke(obj.args);   
        }
    }
}
