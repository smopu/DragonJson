using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonJson;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using PtrReflection;
using System.Reflection;

//namespace DogJson
//{
//    public partial class 大家发财了哇
//    {
//        public int Test1;
//    }
//}

namespace DragonJsonTest
{
    public unsafe class Program
    {
        public class AAA<T, B>
        {
            public class BBB<C, D> { }
        }

        public class CCC
        {
            public class DDD<T, B> { }
        }


        static Type AppDomainGetType(string str)
        {
            Type type = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                type = item.GetType(str);
                if (type != null)
                {
                    break;
                }
            }
            return type;
        }

        public struct KKKK {
            public string str;
            public int num1;
            public double num;
            public char c;
            public float f;
            public KKKKClass kkkkc;
        }
        public class KKKKClass
        {
            public string str;
            public int num1;
            public double num;
            public char c;
            public float f;
        }



        public static unsafe void Main(string[] args)
        {
            UnsafeTool unsafeTool = new UnsafeTool();

            TestClassA testClassA = new TestClassA();
            testClassA.a = 2.718281828459;
            testClassA.d = "大家好";

            byte* objPtr = (byte*)unsafeTool.objectToVoidPtr(testClassA);
            objPtr += sizeof(IntPtr);

            byte* ptr = objPtr;
            //取得字段的偏移量
            int offset = GetFieldOffset(testClassA.GetType().GetField("a", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            ptr += offset;

            //取得字段的值
            double a = *(double*)ptr;
            Console.WriteLine(a);

            //给字段的赋值
            *(double*)ptr = 3.1415926535;
            Console.WriteLine(testClassA.a);

            ptr = objPtr;
            //取得字段的偏移量
            offset = GetFieldOffset(testClassA.GetType().GetField("d", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            ptr += offset;

            //取得字段的值
            string d = (string)unsafeTool.voidPtrToObject(*(void**)ptr);
            Console.WriteLine(d);

            //给字段的赋值
            *(IntPtr*)ptr = (IntPtr)unsafeTool.objectToVoidPtr("草泥马");
            Console.WriteLine(testClassA.d);

            Console.ReadKey();
        }


        public unsafe static int GetFieldOffset(FieldInfo fieldInfo)
        {
            var ptr = fieldInfo.FieldHandle.Value;
            ptr = ptr + 4 + sizeof(IntPtr);
            ushort length = *(ushort*)(ptr);
            byte chunkSize = *(byte*)(ptr + 2);
            return length + (chunkSize << 16);
        }


        [StructLayout(LayoutKind.Explicit)]
        public class TestClassA
        {
            [FieldOffset(0)]
            public double a;
            [FieldOffset(8)]
            public int b;
            [FieldOffset(16)]
            private long c;
            [FieldOffset(24)]
            public string d;
        }


        [StructLayout(LayoutKind.Explicit)]
        public unsafe class UnsafeTool
        {
            public delegate void* ObjectToVoidPtr(object obj);
            [FieldOffset(0)]
            public ObjectToVoidPtr objectToVoidPtr;
            [FieldOffset(0)]
            Func<object, object> func;

            public delegate object VoidPtrToObject(void* ptr);
            [FieldOffset(0)]
            public VoidPtrToObject voidPtrToObject;
            [FieldOffset(0)]
            Func<object, object> func2;

            public UnsafeTool() 
            {
                func = Out;
                func2 = Out;
            }
            object Out(object o) { return o; }
        }





        public class TestJsonClassD
        {
            public B[,,] arrayRekn;
            public long[,,] arrayArray1;
            public int[] arrayArray3;
            public E[] gDs;
        }

        enum DCC
        { 
            n1 = 0,
            n2 = 1,
            n3 = 2,
            n4 = 3,
        }

        enum DCC2
        {
            n0 = 0,
            n1 = 1,
            n2 = 2,
            n3 = 4,
            n4 = 8,
        }

        enum DCC3
        {
            n0 = 0,
            n1 = 1,
            n2 = 2,
            n3 = 3,
            n4 = 4,
            n8 = 8,
        }





        public static unsafe void Main2(string[] args)
        {
            CollectionManager.Start();//ReflectionToObject
            JsonReader jsonRender = new JsonReader();

            string testPath = Path.GetDirectoryName(typeof(JsonReader).Assembly.Location) + @"\JsonFile\" + nameof(JsonReadTestClassA.ReadClassTestJsonClassA) + ".json";
            string str = File.ReadAllText(testPath, Encoding.Unicode);

            TestJsonClassA o = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);

            //Assert.AreEqualObject(o, test1);
            Stopwatch oTime = new Stopwatch();
            int testConst = 10000;

            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < testConst; __1++)
            {
                jsonRender.ReadJsonText(str);
            }
            oTime.Stop();
            double time001 = oTime.Elapsed.TotalMilliseconds;
            Console.WriteLine("1：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
            o = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);

            //string strd = File.ReadAllText("TextFile1.json");

            Stopwatch oTime2 = new Stopwatch();
            oTime2.Reset(); oTime2.Start();
            for (int __1 = 0; __1 < testConst; __1++)
            {
                var ot = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);
                //AddrToObject2.indexDbug++;
            }


            //GC.Collect();
            oTime2.Stop();
            Console.WriteLine("C：{0} 毫秒", (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds);
            Console.WriteLine("2：{0} 毫秒", (oTime2.Elapsed).TotalMilliseconds);

            double time002 = (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds;
            Console.WriteLine(time002 / time001);
            Console.ReadKey();
        }
    }
}
