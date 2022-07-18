using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CollectionReadAttribute : Attribute
    {
        public Type type;
        public bool isArrays;
        public bool inherited;
        public CollectionReadAttribute(Type type, bool isArrays, bool inherited = false)
        {
            this.type = type;
            this.isArrays = isArrays;
            this.inherited = inherited;
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ReadCollectionAttribute : Attribute
    {
        public Type type;
        public bool isArrays;
        public bool inherited;
        public ReadCollectionAttribute(Type type, bool isArrays, bool inherited = false)
        {
            this.type = type;
            this.isArrays = isArrays;
            this.inherited = inherited;
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
     public class CollectionWriteAttribute : Attribute
     {
         public Type type;
         public bool inherited;
         public CollectionWriteAttribute(Type type, bool inherited = false)
         {
             this.type = type;
             this.inherited = inherited;
        }
    }




    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CollectionWriteStringAttribute : Attribute
    {
        public Type type;
        public bool inherited;
        public CollectionWriteStringAttribute(Type type, bool inherited = false)
        {
            this.type = type;
            this.inherited = inherited;
        }
    }




    /// <summary>
    /// 泛型特例
    /// </summary>
    public class SpecialCaseGeneric
     {
         public static SpecialCaseGeneric Create(Type type, Type tagte)
         {
             return new SpecialCaseGeneric(GetGenericArguments(type), tagte);
         }
         public static Type[] GetGenericArguments(Type type)
         {
             var constraints = type.GetGenericArguments()[0].GetGenericParameterConstraints();
             return constraints[0].GetGenericArguments();
         }
         public static Type GetGenericType(Type type)
         {
             var constraints = type.GetGenericArguments()[0].GetGenericParameterConstraints();
             return constraints[0];
         }

         /// <summary>
         /// 比较当前类型的特定泛型是否为当前指定特例的泛型
         /// </summary>
         /// <param name="type"></param>
         /// <returns></returns>
         public bool IsSpecialCaseGeneric(Type type, out Type tagteData)
         {
             var arg = type.GetGenericArguments();
             for (int i = 0; i < length; i++)
             {
                 //Dictionary<string, T> string == string
                 if (genericArg[i] != arg[argIndex[i]])
                 {
                     tagteData = null;
                     return false;
                 }
             }

             Type[] ts = new Type[arg.Length - length];
             for (int i = 0; i < arg.Length - length; i++)
             {
                 //T  = X
                 ts[i] = arg[argSpecialCaseIndex[i]];
             }

             tagteData = tagte.MakeGenericType(ts);

             return true;
         }


         /// <summary>
         /// DictionaryStringWriter<V>
         /// </summary>
         Type tagte;
         int length = 0;

         /// <summary>
         /// 确定的特例泛型  string
         /// </summary>
         Type[] genericArg;

         /// <summary>
         /// 确定的特例泛型   string
         /// </summary>
         int[] argIndex;

         /// <summary>
         /// 不确定的特例泛型  T
         /// </summary>
         int[] argSpecialCaseIndex;


         public SpecialCaseGeneric() { }

         public SpecialCaseGeneric(Type[] arg, Type tagte) 
         {
             this.tagte = tagte;
             for (int i = 0; i < arg.Length; ++i)
             {
                 if (!arg[i].IsGenericParameter)
                 {
                     ++length;
                 }
             }

             genericArg = new Type[length];
             argIndex = new int[length];
             argSpecialCaseIndex = new int[arg.Length - length];
             int k = 0;
             int j = 0;
             for (int i = 0; i < arg.Length; ++i)
             {
                 if (arg[i].IsGenericParameter)
                 {
                     argSpecialCaseIndex[j] = i;
                     ++j;
                 }
                 else
                 {
                     genericArg[k] = arg[i];
                     argIndex[k] = i;
                     ++k;
                 }
             }
         }





     }


     //class DictionString<T> : Dictionary<string, T> { }  sealed 类会导致无法继承，因此使用下面这种方式
     //public class DictionString<Zero, T> : SpecialCaseGeneric where Zero : Dictionary<string, T> { }




 }
