using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

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


        public unsafe static IntPtr GetTypeHead(Type type)
        {
            return type.TypeHandle.Value;
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

        public static Dictionary<string, Type> dictionaryGetType = new Dictionary<string, Type>();
        //public static Dictionary<Type, ICollectionObjectBase> dictionaryGetBox = new Dictionary<Type, ICollectionObjectBase>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetType(string typeName)
        {
            Type type;
            if (dictionaryGetType.TryGetValue(typeName, out type))
            {
            }
            else
            {
                lock (dictionaryGetType)
                {
                    dictionaryGetType[typeName] = type = Type.GetType(typeName);
                }
            }
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

    }

}


