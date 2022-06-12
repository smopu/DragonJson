using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    public static class JsonManager
    {
        static bool isStart = false;
        static object lockObj = new object();
        public static bool IsStart{ get{ return isStart; } }
        public static IJsonRenderToObject jsonRenderToObject;
        public static void Start(IJsonRenderToObject defaultJsonRenderToObject)
        {
            lock (lockObj)
            {
                if (isStart)
                {
                    return;
                }

                jsonRenderToObject = defaultJsonRenderToObject;
                var assembly = Assembly.GetAssembly(typeof(ICollectionObjectBase));

                ChakeAssembly(assembly);
                assembly = Assembly.GetExecutingAssembly();
                ChakeAssembly(assembly);
                assembly = Assembly.GetCallingAssembly();
                ChakeAssembly(assembly);
                isStart = true;
                return;
            }
        }

        public static readonly Dictionary<Type, Type> formatterObjectTypeMap = new Dictionary<Type, Type>();
        public static readonly Dictionary<Type, ICollectionObjectBase> formatterAllObjectMap = new Dictionary<Type, ICollectionObjectBase>();
        private static void ChakeAssembly(Assembly assembly)
        {
            foreach (var t in assembly.GetTypes())
            {
                CollectionAttribute attribute = t.GetCustomAttribute<CollectionAttribute>();
                if (attribute != null)
                {
                    formatterObjectTypeMap[attribute.type] = t;
                    if (!attribute.type.IsGenericType)
                    {
                        formatterAllObjectMap[attribute.type] = (ICollectionObjectBase)System.Activator.CreateInstance(t);
                    }
                    //if (attribute.isArrays)
                    //{
                    //    formatterArrayTypeMap[attribute.type] = t;
                    //    if (!attribute.type.IsGenericType)
                    //    {
                    //        formatterAllArrayMap[attribute.type] = (ICollectionObjectBase)System.Activator.CreateInstance(t);
                    //    }
                    //}
                    //else
                    //{
                    //    formatterObjectTypeMap[attribute.type] = t;
                    //    if (!attribute.type.IsGenericType)
                    //    {
                    //        formatterAllObjectMap[attribute.type] = (ICollectionObjectBase)System.Activator.CreateInstance(t);
                    //    }
                    //}
                }
            }
        }



        //public static readonly Dictionary<Type, Type> formatterArrayTypeMap = new Dictionary<Type, Type>();


        //public static readonly Dictionary<Type, ICollectionObjectBase> formatterAllArrayMap = new Dictionary<Type, ICollectionObjectBase>();




    }

}
