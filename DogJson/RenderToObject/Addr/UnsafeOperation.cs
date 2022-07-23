using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

namespace DogJson
{

    public unsafe class UnsafeOperation
    {
        public readonly static int PTR_COUNT = sizeof(IntPtr);

        static bool is64 = Environment.Is64BitProcess;

        /// <summary>
        /// 获取值类型的地址
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public unsafe static IntPtr GetValueAddr(TypedReference tf)
        {
            return *(IntPtr*)&tf;
        }
        
        /// <summary>
        /// 获取值类型的地址
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public unsafe static void* GetValueAddrVoidPtr(TypedReference tf)
        {
            return *(void**)&tf;
        }
        /// <summary>
        /// 获取引用类型的地址
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>

        public unsafe static IntPtr GetObjectAddr(TypedReference tf)
        {
            return *(IntPtr*)*(IntPtr*)&tf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int SizeOf(Type type)
        {
            return *((int*)type.TypeHandle.Value + 1);
        }



        static Dictionary<Type, int> allSize = new Dictionary<Type, int>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int SizeOfStack(Type type)
        {
            if (type.IsValueType)
            {
                int size;
                if (allSize.TryGetValue(type, out size))
                {
                    return size;
                }
                else
                {
                    int sizeOfGG = allSize[type] = (int)typeof(TypeTool<>).MakeGenericType(type).GetMethod("SizeOf", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
                    return sizeOfGG;
                }
            }
            else
            {
                return sizeof(IntPtr);
            }
        }


        static Dictionary<Type, IntPtr> allTypeHead = new Dictionary<Type, IntPtr>();
        public unsafe static IntPtr GetTypeHead(Type type)
        {
            if (type.IsArray)
            {
                IntPtr value;
                if (allTypeHead.TryGetValue(type, out value))
                {
                    return value;
                }
                else
                {
                    value = allTypeHead[type] = *(IntPtr*)GeneralTool.ObjectToVoid(
                    Array.CreateInstance(type.GetElementType(), new int[type.GetArrayRank()]));
                    return value;

                }
            }
            else
            {
                //object v = FormatterServices.GetUninitializedObject(type);
                //IntPtr value = allTypeHead[type] = *(IntPtr*)GeneralTool.ObjectToVoid(v);
                //return value;
                return type.TypeHandle.Value;
            }
            return type.TypeHandle.Value;


            //FormatterServices.GetUninitializedObject(myObject.type);
        }
  


        public unsafe static IntPtr* GetObjectAddr2(TypedReference tf)
        {
            return (IntPtr*)*(IntPtr*)&tf;
        }

        /// <summary>
        ///  获取引用类型的地址
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static IntPtr GetObjectAddr(object obj)
        {
            TypedReference tf = __makeref(obj);
            return *(IntPtr*)*(IntPtr*)&tf;
        }

        /// <summary>
        ///  程序的字段声明偏移位置
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public unsafe static int GetFeildOffset(IntPtr ptr)
        {
            ptr = ptr + 4 + sizeof(IntPtr);
            short length = *(short*)(ptr);
            byte chunkSize = *(byte*)(ptr + 2);
            return length + (chunkSize << 16);
        }
        public unsafe static int GetFeildOffset(FieldInfo fieldInfo)
        {
            var ptr = fieldInfo.FieldHandle.Value;
            ptr = ptr + 4 + sizeof(IntPtr);
            short length = *(short*)(ptr);
            byte chunkSize = *(byte*)(ptr + 2);
            return length + (chunkSize << 16);
        }

        public unsafe static IntPtr GetValueAddr(object obj)
        {
            TypedReference tf = __makeref(obj);
            return *(IntPtr*)&tf;
        }
        /// <summary>
        ///  将对象的地址设置到目标地址，不会有类型判定和引用计数，操作堆数据会造成GC判定错误，推荐在栈上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void UnsafeSetObject(void* tar, object obj)
        {
            TypedReference tf = __makeref(obj);
            *(IntPtr*)tar = *(IntPtr*)(*(IntPtr*)&tf);

        }

        public static Dictionary<string, Assembly> dictionaryAllAssembly = new Dictionary<string, Assembly>();

        //public static Dictionary<string, Type> dictionaryGetType = new Dictionary<string, Type>();
        //public static Dictionary<Type, ICollectionObjectBase> dictionaryGetBox = new Dictionary<Type, ICollectionObjectBase>();
        public static ConcurrentDictionary<string, Type> dictionaryGetType = new ConcurrentDictionary<string, Type>();

        public static unsafe Type GetType(string str)
        {
            Stack<Tuple<Type, int>> stack = new Stack<Tuple<Type, int>>();
            bool isRread = true;
            fixed (char* s = str)
            {
                int start = 0;
                int length = str.Length;
                for (int i = 0; i < length; i++)
                {
                    Loop:
                    char now = s[i];
                    switch (now)
                    {
                        case '`':
                            //泛型
                            int size = 0;
                            ++i;
                            for (; i < length; i++)
                            {
                                now = s[i];
                                if ('0' > now || now > '9')
                                {
                                    break;
                                }
                                else
                                {
                                    size *= 10;
                                    size += now - '0';
                                }
                            }
                            Type typeW = AppDomainGetType(new string(s, start, i - start));
                            stack.Push(Tuple.Create(typeW, size));
                            isRread = false;
                            break;
                        case '[':
                            if (isRread)
                            {
                                //数组
                                ++i;
                                for (; i < length; i++)
                                {
                                    now = s[i];
                                    if (now == ']')
                                    {
                                        ++i;
                                        now = s[i];
                                        if (now == '[')
                                        {
                                            //复合数组 继续读取
                                        }
                                        else
                                        {
                                            Type type1 = AppDomainGetType(new string(s, start, i - start));
                                            stack.Push(Tuple.Create(type1, 0));
                                            isRread = false;
                                            goto Loop;
                                        }
                                    }
                                    ++i;
                                    now = s[i];
                                }
                            }
                            break;
                        case ',':
                        case ']':
                            if (isRread)
                            {
                                Type type2 = AppDomainGetType(new string(s, start, i - start));
                                stack.Push(Tuple.Create(type2, 0));
                                isRread = false;
                            }
                            break;
                        default:
                            if (!isRread)
                            {
                                isRread = true;
                                start = i;
                            }
                            break;
                    }
                }
            }

            Stack<Tuple<Type, int>> stack2 = new Stack<Tuple<Type, int>>();
            while (true)
            {
                Tuple<Type, int> data = stack.Pop();
                //if (type.IsGenericType)
                //{
                //    int size = type.GetGenericArguments().Length;
                //    Type[] arg = new Type[size];
                //    for (int j = 0; j < size; j++)
                //    {
                //        arg[j] = stack2.Pop();
                //    }
                //    type = type.MakeGenericType(arg);
                //    if (stack.Count == 0)
                //    {
                //        return type;
                //    }
                //    stack2.Push(type);
                //}
                //else
                //{
                //    stack2.Push(type);
                //}
            }
        }

        static Type AppDomainGetType(string str)
        {
            Type type;
            if (dictionaryGetType.TryGetValue(str, out type))
            {
                return type;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                type = item.GetType(str);
                if (type != null)
                {
                    break;
                }
            }

            dictionaryGetType[str] = type;
            return type;
        }


        /// <summary>
        /// 将对象的地址设置到目标地址，有类型判定和引用计数，推荐在堆上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        public unsafe static void SetObject(IntPtr tar, object obj)
        {
            object tmp = "";
            TypedReference tr = __makeref(tmp);
            IntPtr* p = (IntPtr*)&tr;
            *p = tar;
            __refvalue(tr, object) = obj;
        }

        /// <summary>
        /// 获取目标地址的对象
        /// </summary>
        /// <param name="tar"></param>
        /// <returns></returns>
        public unsafe static object VoidToObject(void* tar)
        {
            object tmp = "";
            TypedReference tr = __makeref(tmp);//new TypedReference();
            IntPtr* p = (IntPtr*)&tr;
            *p = (IntPtr)tar;
            return __refvalue(tr, object);
        }
        public unsafe static void* ObjectToVoid(object obj)
        {
            TypedReference tf = __makeref(obj);
            return *(void**)*(IntPtr*)&tf;
        }
        public unsafe static void* ObjectToVoid2(object obj)
        {
            TypedReference tf = __makeref(obj);
            return *(void**)*(IntPtr*)&tf;
        }


        /// <summary>
        ///  获取32位程序的字段声明偏移位置
        /// <param name="ptr"></param>
        /// <returns></returns>
        public unsafe static int GetFeildOffset32(IntPtr ptr)
        {
            return *(Int16*)(ptr + 8) + 4;
        }

        /// <summary>
        /// 将对象的地址设置到目标地址，不会有类型判定和引用计数，操作堆数据会造成GC判定错误，推荐在栈上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        public unsafe static void UnsafeSetObject32(int* tar, object obj)
        {
            TypedReference tf = __makeref(obj);
            *tar = *(int*)(*(IntPtr*)&tf);
        }

        /// <summary>
        /// 将对象的地址设置到目标地址，有类型判定和引用计数，推荐在堆上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        public unsafe static void SetObject32(int* tar, object obj)
        {

            object tmp = "";
            TypedReference tr = __makeref(tmp);
            int* p = (int*)&tr;
            *p = (int)tar;
            __refvalue(tr, object) = obj;
        }

        /// <summary>
        /// 获取目标地址的对象
        /// </summary>
        /// <param name="tar"></param>
        /// <returns></returns>
        public unsafe static object GetObject32(int* tar)
        {

            object tmp = "";

            TypedReference tr = __makeref(tmp);//new TypedReference();

            int* p = (int*)&tr;

            *p = (int)tar;

            return __refvalue(tr, object);

        }

        /// <summary>
        /// 获取64位程序的字段声明偏移位置
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public unsafe static int GetFeildOffset64(IntPtr ptr)
        {
            return *(Int16*)(ptr + 12) + 8;
        }

        /// <summary>
        /// 将对象的地址设置到目标地址，不会有类型判定和引用计数，操作堆数据会造成GC判定错误，推荐在栈上操作
        /// </summary>

        /// <param name="tar"></param>

        /// <param name="obj"></param>

        public unsafe static void UnsafeSetObject64(long* tar, object obj)

        {

            TypedReference tf = __makeref(obj);

            *tar = *(long*)(*(IntPtr*)&tf);

        }

        /// <summary>
        /// 将对象的地址设置到目标地址，有类型判定和引用计数，推荐在堆上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        public unsafe static void SetObject64(long* tar, object obj)

        {

            object tmp = "";

            TypedReference tr = __makeref(tmp);

            long* p = (long*)&tr;

            *p = (long)tar;

            __refvalue(tr, object) = obj;

        }

        /// <summary>
        /// 获取目标地址的对象
        /// </summary>
        /// <param name="tar"></param>
        /// <returns></returns>
        public unsafe static object GetObject64(void* tar)

        {

            object tmp = "";

            TypedReference tr = __makeref(tmp);//new TypedReference();

            long* p = (long*)&tr;

            *p = (long)tar;

            return __refvalue(tr, object);

        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Type GetType(string typeName)
        //{
        //    Type type;
        //    if (dictionaryGetType.TryGetValue(typeName, out type))
        //    {
        //    }
        //    else
        //    {
        //        type = Type.GetType(typeName);
        //        if (type == null)
        //        {
        //            int index = typeName.IndexOf(',');

        //            if (index > 0)
        //            {
        //                string assemblyName = typeName.Substring(0, index);
        //                Assembly assembly;
        //                if (!dictionaryAllAssembly.TryGetValue(assemblyName, out assembly))
        //                {
        //                    assembly = Assembly.Load(assemblyName);//WithPartialName
        //                }

        //                string TypeName2 = typeName.Substring(index + 1, typeName.Length - index - 1);
        //                type = assembly.GetType(TypeName2);

        //            }
        //            //if (type == null)
        //            //{
        //            //    foreach (var item in dictionaryAllAssembly)
        //            //    {

        //            //    }
        //            //    //assembly = Assembly.GetEntryAssembly();
        //            //    //var sss = assembly.GetTypes();
        //            //    return type;
        //            //}
        //        }
        //        lock (dictionaryGetType)
        //        {
        //            dictionaryGetType[typeName] = type;
        //        }
        //    }
        //    return type;
        //}

    }

}


