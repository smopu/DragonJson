﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PtrReflection;

namespace DragonJson
{
    public partial class 大家发财了哇
    {
        public int test3;
    }
    public class TClassA
    {
        public static void Fool(int a, string b) 
        {

        }
        public static void Foo2(int a, string b)
        {

        }

    }
    public class Program
    {

        private unsafe JsonValue GCC(void* a)
        {
            return *(JsonValue*)a;
        }


        /// <summary>
        /// 对多维数组的每个元素赋相同的值
        /// </summary>
        /// <param name="arrayObject"></param>
        /// <param name="value"></param>
        static void SetArrayValue(object arrayObject, object value)
        {
            Array array = arrayObject as Array;

            int rank = array.Rank;
            int[] lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = array.GetLength(i);
            }
            int nowRank = 0;
            int[] indices = new int[rank];

            do
            {
                for (int i = 0; i < lengths[0]; i++)
                {
                    indices[0] = i;
                    array.SetValue(13123, indices);
                }
                nowRank = 1;
            Chake:
                ++indices[nowRank];
                if (indices[nowRank] == lengths[nowRank])
                {
                    ++nowRank;
                    if (nowRank < rank)
                    {
                        goto Chake;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 0; i < nowRank; i++)
                {
                    indices[i] = 0;
                }
            } while (true);
        }

        /// <summary>
        /// 多维数组按照 [index,index,,,] : Value输出
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static string ArrayToString(Array array)
        {
            StringBuilder sb = new StringBuilder();
            int rank = array.Rank;
            int[] lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = array.GetLength(i);
            }
            int nowRank = 0;
            int[] indices = new int[rank];

            do
            {
                for (int i = 0; i < lengths[0]; i++)
                {
                    indices[0] = i;
                    sb.Append("[");
                    for (int j = 0; j < rank - 1; j++)
                    {
                        sb.Append(indices[j] + ",");
                    }
                    sb.Append(indices[rank - 1] + "] : ");
                    sb.AppendLine(array.GetValue(indices).ToString());
                }
                nowRank = 1;
            Chake:
                ++indices[nowRank];
                if (indices[nowRank] == lengths[nowRank])
                {
                    ++nowRank;
                    if (nowRank < rank)
                    {
                        goto Chake;
                    }
                    else
                    {
                        return sb.ToString();
                    }
                }
                for (int i = 0; i < nowRank; i++)
                {
                    indices[i] = 0;
                }
            } while (true);
        }

        /// <summary>
        /// 多维数组按照 Json 格式输出
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static string ArrayToJsonString(Array array)
        {
            StringBuilder sb = new StringBuilder();
            int rank = array.Rank;
            int[] lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = array.GetLength(i);
            }
            for (int i = 0; i < rank - 1; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    sb.Append(" ");//\t
                }
                sb.AppendLine("[");
            }
            int nowRank = 0;
            int[] indices = new int[rank];

            do
            {
                for (int j = 0; j < rank; j++)
                {
                    sb.Append(" ");//\t
                }
                sb.Append("[");
                for (int i = 0; i < lengths[0]; i++)
                {
                    indices[0] = i;
                    if (i < lengths[0] - 1)
                    {
                        sb.Append(array.GetValue(indices).ToString() + ",");
                    }
                    else
                    {
                        sb.Append(array.GetValue(indices).ToString());
                    }
                }
                sb.Append("]");
                nowRank = 1;
            Chake:
                ++indices[nowRank];
                if (indices[nowRank] == lengths[nowRank])
                {
                    sb.AppendLine();
                    ++nowRank;
                    if (nowRank < rank)
                    {
                        for (int j = 0; j < rank - nowRank + 1; j++)
                        {
                            sb.Append(" ");//\t
                        }
                        sb.Append("]");
                        goto Chake;
                    }
                    else
                    {
                        sb.AppendLine("]");
                        return sb.ToString();
                    }
                }
                else
                {
                    sb.AppendLine(",");
                }
                for (int i = 0; i < nowRank; i++)
                {
                    indices[i] = 0;
                }
                for (int i = 0; i < nowRank - 1; i++)
                {
                    for (int j = 0; j < rank - i; j++)
                    {
                        sb.Append(" ");//\t
                    }
                    sb.AppendLine("[");
                }
            } while (true);
        }


        static int testCount = 1000;

        const int testCount2 = 97;
        const int testCount3 = 8;
        static int overBit = -0;

        static int[] bsdad = new int[testCount2 * testCount3];

        static bool[] Run = new bool[testCount3];
        static int[] RunIndex = new int[testCount3];

        static void TestCall(int j, int value)
        {
            for (int i = 0; i < 300; i++)
            {
                bsdad[j] = value;
            }
        }

        static int over = 0;
        static object lockall = new object();

        /// <summary>
        /// 创建有参的方法
        /// 注意：方法里面的参数类型必须是Object类型
        /// </summary>
        /// <param name="obj"></param>
        static void Thread1(object obj)
        {
            //Tuple<Thread, int> t = (System.Tuple<Thread, int>)obj;
            
            //int d = t.Item2;
            //Thread thread = t.Item1;

            int d = (int)obj;


            int max = d * testCount2 + testCount2;
            while (true)
            {
                while ((over & (1 << d)) == 0)
                {

                }
                for (int i = d * testCount2; i < max; i++)
                {
                    TestCall(i, 9999);
                }
                //Console.WriteLine("XX " + d);
                over &= (~(1 << d));
                //Run[d] = false;
                //--over;
            }
            //while (true)
            //{
            //    for (int i = d, max = d + testCount2; i < max; i++)
            //    {
            //        bsdad[i] = 999;
            //    }

            //    over--;
            //    return;
            //    thread.Suspend();
            //}
        }

        static void Threadd0(object obj)
        {
            Tuple<Thread, int> t = (System.Tuple<Thread, int>)obj;

            int d = t.Item2;
            Thread thread = t.Item1;
            int max = bsdad.Length;// d * testCount2 + testCount2;
            while (true)
            {
                while ((over & (1 << d)) == 0)
                {

                }
                for (int i = 0; i < max; i++)
                {
                    RunIndex[1] = 0;
                    if (RunIndex[1] == i)
                    {
                        break;
                    }
                    bsdad[i] = 9999;
                }
                //Console.WriteLine("XX " + d);
                over &= (~(1 << d));
                //Run[d] = false;
                //--over;
            }
        }


        static T Read<T>(IntPtr address)
        {
            var obj = default(T);
            var tr = __makeref(obj);

            //This is equivalent to shooting yourself in the foot
            //but it's the only high-perf solution in some cases
            //it sets the first field of the TypedReference (which is a pointer)
            //to the address you give it, then it dereferences the value.
            //Better be 10000% sure that your type T is unmanaged/blittable...
            unsafe { *(IntPtr*)(&tr) = address; }

            return __refvalue(tr, T);
        }

        unsafe struct SpanString 
        {
            public char* txt;
            public int startIndex;
            public int length;
            public SpanString(char* txt, int startIndex, int length)
            {
                this.txt = txt;
                this.startIndex = startIndex;
                this.length = length;


                var getValue = Delegate.CreateDelegate(typeof(Func<TestJsonClassA, double>),
                    typeof(TestJsonClassA).GetProperty("Num").GetGetMethod()) as Func<TestJsonClassA, double>;

                TestJsonClassA a1 = new TestJsonClassA();

                Action<double> action = typeof(TestJsonClassA).GetProperty("Num").GetSetMethod().CreateDelegate
                    (typeof(Action<double>), a1) as Action<double>;



            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class Pinnable<T>
        //GetType().GetMethod("").CreateDelegate;
        {
            public T Data;
        }
        public class ReflectionManage
        {
            class A
            {
                private int k1;
                protected int b4;
            }
            class B : A
            {
                private int k2;
                protected new int b3;
            }

            class C : B
            {
                private int Is64BitProcess;
                private int Is64BitProcess_2;
                private int Is64BitProcess_3;
                protected new int b;
            }


        }

        //static unsafe void Foo1(string[] args)
        //{

        //}

        //static unsafe void Foo1(ref string args)
        //{

        //}

        public void Foo1(string args)
        {
            args = null;
        }
        public void Foo1(int args)
        {

        }
        public static void Foo1(object args)
        {

        }

        public static void Foo2(object args)
        {

        }
        public static void Foo3(object args)
        {

        }

        public unsafe struct Chake
        {
            public int chakeIndex;
            public char chakeValue;
            public Chake* next;
            public Chake* no;
            public char* data;
            public char* noData;
            public int dataIndex;
            public int noDataIndex;
        }

        public unsafe class ChakeTree
        {
            public List<ChakeTree> chakeTrees = new List<ChakeTree>();
            public char chake;
            public int index = -1;
            public string[] strs;
            public void Fl() {

                Dictionary<char, int> dict = new Dictionary<char, int>();
                Dictionary<char, List<string>> dictList = new Dictionary<char, List<string>>();
                int nowIndex = index;
                int max = 0;
                char maxchar = ' ';
                do
                {
                    ++nowIndex;
                    dict = new Dictionary<char, int>();
                    dictList = new Dictionary<char, List<string>>();
                    for (int i = 0; i < strs.Length; i++)
                    {
                        char now = strs[i][nowIndex];
                        if (dict.ContainsKey(now))
                        {
                            ++dict[now];
                            dictList[now].Add(strs[i]);
                        }
                        else
                        {
                            dict[now] = 1;
                            dictList[now] = new List<string>();
                            dictList[now].Add(strs[i]);
                        }
                        if (dict[now] > max)
                        {
                            max = dict[now];
                            maxchar = now;
                        }
                    }
                    dict = dict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                } while (dict.Count == 1);
                //chake = dict.First().Key;
                foreach (var item in dict)
                {
                    ChakeTree chakeTree = new ChakeTree();
                    chakeTree.chake = item.Key;
                    chakeTree.index = nowIndex;
                    chakeTree.strs = dictList[chakeTree.chake].ToArray();
                    chakeTrees.Add(chakeTree);
                    if (chakeTree.strs.Length > 1)
                    {
                        chakeTree.Fl();
                    }
                }
            }

            public void Over(char* allChar,int size, Chake* chakes, int id) 
            {
                for (int i = 0; i < chakeTrees.Count - 1; i++)
                {
                    chakes[id].chakeIndex = chakeTrees[i].index;
                    chakes[id].chakeValue = chakeTrees[i].chake;
                    chakes[id].data = allChar + size * id;
                    chakes[id].dataIndex = id;
                    chakes[id].no = chakes + (chakeTrees[i].strs.Length);
                    if (chakeTrees[i].chakeTrees.Count > 0)
                    {
                        chakes[id].next = chakes + (id + i);
                        chakeTrees[i].Over(allChar, size, chakes, id + i);
                        //for (int j = 0; j < chakeTrees[i].chakeTrees.Count; j++)
                        //{
                        //    chakes[id].next = chakes + (id + j);
                        //    chakeTrees[i].chakeTrees[j].Over(allChar, size, chakes, id + j);
                        //}
                    }
                }
                int overI = chakeTrees.Count - 2;
                chakes[id].noData = allChar + size * 2;
                chakes[id].noDataIndex = 2;


            }



        }
        public enum TestEnum
        {
          Test001,
          Test002,
          Test003,
          Test004,
        }


        static unsafe bool Find(char* path, int pathLength, JsonReader jsonRender, ref JsonObject* obj)
        {
            JsonObject* parent = obj;
            //JsonObject* now = jsonRender.objectQueue + obj->objectQueueIndex + 1;
            ++obj;
            Start:
            if (parent->isObject)
            {
                int keySize = pathLength;
                for (int i = 0; i < pathLength; i++)
                {
                    if (*(path + i) == '/')
                    {
                        keySize = i;
                        break;
                    }
                }

            ObjectStart:
                if (keySize != obj->keyStringLength)
                {
                    goto False;
                }

                char* now = obj->keyStringStart;

                for (int i = 0; i < keySize; i++)
                {
                    if (*(path + i) == *(now + i))
                    {
                    }
                    else
                    {
                        goto False;
                    }
                }

                //true
                pathLength -= keySize + 1;
                if (pathLength <= 0)
                {
                    return true;
                }
                path += keySize;
                if (*path == '/')
                {
                    if (obj->objectQueueIndex + 1 >= jsonRender.objectQueueIndex)
                    {
                        return false;
                    }
                    parent = obj;
                    ++obj;
                    ++path;
                    goto Start;
                }
            False:
                //寻找下一个
                if (obj->objectNext >= jsonRender.objectQueueIndex)
                {
                    return false;
                }
                var next = jsonRender.objectQueue + obj->objectNext;
                //如果下一个的父对象不是和之前的一样
                //next->keyStringStart == null || 
                if (next->parentObjectIndex != obj->parentObjectIndex)
                {
                    return false;
                }
                obj = next;
                goto ObjectStart;
            }
            else
            {
                int pathIndex = 0;
                int pathSize = 0;
                for (int i = 0; i < pathLength; i++)
                {
                    if (*(path + i) == '/')
                    {
                        break;
                    }
                    if (*path < '0' || *path > '9')
                    {
                        return false;
                    }
                    else
                    {
                        ++pathSize;
                        pathIndex *= 10;
                        pathIndex += (*path - '0');
                    }
                }

                if (pathIndex >= parent->arrayCount)
                {
                    return false;
                }
                for (int i = 0; i < pathIndex; i++)
                {
                    obj = jsonRender.objectQueue + obj->objectNext;
                }
                path += pathSize + 1;
                pathLength -= pathSize + 1;
                if (pathLength <= 0)
                {
                    return true;
                }
                parent = obj;
                ++obj;
                goto Start;
            }

        }


        static unsafe void Main7(string[] args)
        {
            JsonReader jsonRender = new JsonReader();
            StreamReader streamReader = new StreamReader(@"TextFile1.json", Encoding.UTF32);
            string str = streamReader.ReadToEnd();
            jsonRender.ReadJsonText(str);

            //string data = "tClass001/tClass002/tClass003";
            //string data = "tClass001/tClass002s/1/tClass003";
            string data = "tClass001/tClass002/tClass003s/2";
            //string data = "tClass001/tClass002s/1/tClass003s/2";

            //"testClassDD": "tClass001/tClass002/tClass003",
            //"testClassDD4": "tClass001/tClass002s/1/tClass003",
            //"testClassDD2": "tClass001/tClass002/tClass003s/2",
            //"testClassDD3": "tClass001/tClass002s/1/tClass003s/2",


            JsonObject* obj = jsonRender.objectQueue;
            fixed (char* now = data)
            { 
               
                if (Find(now, data.Length, jsonRender, ref obj))
                {
                    Console.WriteLine(new string(obj->keyStringStart, 0, obj->keyStringLength));
                    Console.WriteLine(obj->arrayIndex);
                    obj = jsonRender.objectQueue + obj->parentObjectIndex;
                    Console.WriteLine(new string(obj->keyStringStart, 0, obj->keyStringLength));
                }
                else
                {
                    Console.WriteLine("false");
                }
            }

            for (int i = 1; i < jsonRender.objectQueueIndex; i++)
            {
                var v = jsonRender.objectQueue[i];
                var next = jsonRender.objectQueue[v.objectNext];
                var key = new string(v.keyStringStart, 0, v.keyStringLength);

                Console.WriteLine();

                if (next.isObject)
                {
                    if (next.keyStringStart != null)
                    {
                        var nextKey = new string(next.keyStringStart, 0, next.keyStringLength);
                        Console.WriteLine(key + "," + nextKey);
                    }
                    else
                    {
                        Console.WriteLine(key + " | " + v.objectNext);
                    }
                }
                else
                {
                    Console.WriteLine(key + "," + next.arrayIndex);
                }
            }

            Console.ReadKey();
        }


        //@自己 ..父类  $根结点 

        public struct A3struct
        {
            public double Num
            {
                get { return num; }
                set { num = value; }
            }
            public double num;
        }
        public class A3
        {
            public double Num
            {
                get { return num; }
                set { num = value; }
            }
            public double num;

            public A3struct Struct
            {
                get { return v; }
                set { v = value; }
            }
            public A3struct v;
        }

        public class TclassACC
        {
            public double num;
            public bool b;
            public double Num
            {
                get { return num; }
                set { num = value; }
            }
        }


        public class AAXA
        {
            private double num;
            public double Num
            {
                get { return num; }
                set { num = value; }
            }

            private B bb;
            public B BB
            {
                get { return bb; }
                set { bb = value; }
            }
        }

        class Ott2
        {
            public object a1;
            public ACE5 a5;
            public object a6;
            public object a7;
            //public int[,,] arrayArray1;
            //public int[,][] arrayArray2;
            //public int[] arrayArray3;
            //public int[][] arrayArray4;
            //public int[,,] arrayArray5;
        }

        public static unsafe void Main(string[] args)
        {
            float a = 6.4f;
            int b = *(int*)(&a);
            int b2  = a.GetHashCode();

            TestJsonClassA inputData = new TestJsonClassA()
            {
                b = true,
                Num = 33.56,
                kk = -15,
                str = "4ERWRds",
                BB = new B
                {
                    str = "Wvs",
                    b = true,
                    num = -9999.232
                },
                //gcc = new C
                //{
                //    k = 12,
                //    bbb = {
                //        str = "cc",
                //        b = true,
                //        num = -8.56
                //    }
                //},
                //p3 = {
                //    x = 2,
                //    y = 1,
                //    z = 33
                //},
                //testOB = {
                //    numI = 13,
                //    num = 3.6,
                //    p3_0 = new P_box_3 {
                //        x = 1.2f,
                //        y = -1.8f,
                //        z = 33
                //    },
                //    p3_2 = {
                //        x = -12,
                //        y = 18,
                //        z = 3.3f
                //    }
                //},
                //gD = new E
                //{
                //    k = -3.14E-12,
                //    str = "3特瑞aV",
                //    b = true
                //},
                //arrayArray3 = new int[] { 1, 2, 3, 4, 5 },
                //arrayArray4 = new int[][] {
                //    new int[] { 1, 2, 3, 4, 5 },
                //    new int[] { 11, 12, 13, 14, 15 },
                //    new int[] { 21, 22, 23, 24, 25 }
                //},
                //arrayArray1 = new int[,,] {
                //    {
                //        { 1, 2, 3, 4, 5 },
                //        { 11, 12, 13, 14, 15 },
                //        { 21, 22, 23, 24, 25 }
                //    },
                //    {
                //        { 101, 102, 103, 104, 105 },
                //        { 1011, 1012, 1013, 1014, 1015 },
                //        { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},
                //arrayArray2 = new int[,][] {
                //    {
                //        new int[] { 1, 2, 3, 4, 5 },
                //        new int[] { 11, 12, 13, 14, 15 },
                //        new int[] { 21, 22, 23, 24, 25 }
                //    },
                //    {
                //        new int[] { 101, 102, 103, 104, 105 },
                //        new int[] { 1011, 1012, 1013, 1014, 1015 },
                //        new int[] { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},
                //arrayArray5 = new int[,,] {
                //{
                //    { 1, 2, 3, 4, 5 },
                //    { 11, 12, 13, 14, 15 },
                //    { 21, 22, 23, 24, 25 }
                //    },
                //{
                //    { 1001, 1002, 1003, 1004, 1005 },
                //    { 1011, 1012, 1013, 1014, 1015 },
                //    { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},

                //ddd = (System.Double)(-3.14E-12),
                //objects = new object[] {
                //    (System.Int32)(12),
                //    (System.Double)(-3.14E-12)
                //},
                //testEnums = new TestEnum[] { TestEnum.Test002, TestEnum.Test004, TestEnum.Test003 },
                //testEnum = TestEnum.Test002,


                //listB = new List<B> {
                //    new B {
                //        str = "00001",
                //        b = true,
                //        num = -8.56
                //    },
                //    new B {
                //        str = "aaaa2",
                //        b = false,
                //        num = 10000888.999
                //    },
                //},

                //arrayLinkedList = new LinkedList<long>(new List<long> { 1L, 2L, 3L, 4L, 7L }),// { 1L, 2l, 3l, 4l, 7l }

                //arrayStack = new Stack<int>(new List<int> { 3, 4, 5 }),

                //arraydouble = new HashSet<double>() {
                //    3.333,
                //    -4.8888,
                //    -5.34E+108
                //},
                //arraystring = new Queue<string>(
                //   new List<string> {
                //       "true",
                //       "null",
                //       "ed false"
                //   }
                //),
                //arraybool = new List<bool>() {
                //    false,
                //    true,
                //    false
                //},

                //arrayint2 = new List<System.Int32> { 14, 24, 34, 44, 54 },

                //fd = new B[] {
                //    new B {
                //        b = true,
                //        num = -3.14E-12,
                //        str = "ddd"
                //    },
                //    new B {
                //        b = false,
                //        num = 34.5
                //    },
                //    new B {
                //        num = 999999
                //    }
                //},

                //dcc = new TclassDCC("3213.#$%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),

                //Iclass0Z = new DogJsonTest.TclassC
                //{
                //    b = 122,
                //    value = 1.444,
                //    bbb = new B
                //    {
                //        b = true,
                //        num = -3.14E-12,
                //        str = "hello world"
                //    }
                //},
                //v3 = new V3(3, 2, 1),
                //testDelegate2 = TClassA.Fool,

                //arrayRekn = new B[,,]
                //{
                //    {
                //        {
                //            new B {
                //                num = 1
                //            },
                //            new B {
                //                num = 2,
                //                str = "ddd"
                //            }
                //        },
                //        {
                //            new B {
                //                num = 3
                //            },
                //            new B {
                //                num = 4
                //            }
                //        }
                //    },
                //    {
                //        {
                //            new B {
                //                num = 5,
                //                str = "5"
                //            },
                //            new B {
                //                num = 6,
                //                str = "6"
                //            }
                //        },
                //        {
                //            new B {
                //                num = 7,
                //                str = "7"
                //            },
                //            new B {
                //                num = 8,
                //                str = "8"
                //            }
                //        }
                //    },
                //    {
                //        {
                //            new B {
                //                num = 9,
                //                str = "9"
                //            },
                //            new B {
                //                num = 10,
                //                str = "10"
                //            }
                //        },
                //        {
                //            new B {
                //                num = 11,
                //                str = "11"
                //            },
                //            new B {
                //                num = 12,
                //                str = "12"
                //            }
                //        }
                //    }
                //},

                //tClass001 = new TClass001
                //{
                //    objects = new object[] {
                //        (System.Int32)12,
                //        -(System.Double)3.14E-12,
                //        (System.String)"3213.#$%^&*()",
                //    },
                //    tClass002s = new TClass002[] {
                //        new TClass002 {
                //            size = 1,
                //            testString = "A@0"
                //        },
                //        new TClass002 {
                //            size = 2,
                //            testString = "A@1",
                //            tClass003 = new TClass003 {
                //                testString = "testClassDD4"
                //            },
                //            tClass003s = new TClass003[] {
                //                new TClass003 {
                //                    testString = "0000"
                //                },
                //                new TClass003 {
                //                    testString = "0001"
                //                },
                //                new TClass003 {
                //                    testString = "testClassDD3"
                //                }
                //            }
                //        },
                //        new TClass002 {
                //            size = 3,
                //            testString = "asdede"
                //        }
                //    },

                //    tClass002 = new TClass002
                //    {
                //        size = 12,
                //        tClass003 = new TClass003
                //        {
                //            testString = "testClassDD"
                //        },
                //        tClass003s = new TClass003[] {
                //            new TClass003 {
                //                testString = "0000"
                //            },
                //            new TClass003 {
                //                testString = "0001"
                //            },
                //            new TClass003 {
                //                testString = "testClassDD2"
                //            },
                //            new TClass003 {
                //                testString = "0003"
                //            }
                //        },

                //        testString = "A@2"
                //    },
                //    TestDuble = -3.14E-12
                //},

                //dictionary3 = new Dictionary<V3, B>()
                //{
                //    {
                //       new V3( 3, 2, 1 ),
                //       new B {
                //            num = 11111,
                //            str= "rradads"
                //        }
                //    },

                //    {
                //       new V3( 1, 4, -9.8f  ),
                //       new B {
                //            num = 888,
                //            str= "热热我"
                //        }
                //    },

                //    {
                //       new V3( -3.2E-13f, 3.0E+13f, 0.99f ),
                //       new B {
                //            num = 999999,
                //            str= "特别强势人物了"
                //        }
                //    }
                //},
                //tstructA = new TstructA
                //{
                //    value = 3.6,
                //    b = new TstructB
                //    {
                //        kk = "FC",
                //        c = new TstructC
                //        {
                //            Id = 21443,
                //        }
                //    }
                //}
            };

            ACE5 a6 = new ACE5()
            {
                kk = 3,
                str = "13214",
                aCE3 = new ACE3()
                {
                    kk = 14,
                    str = "serwte342",
                }
            };
            Ott2 ott2 = new Ott2()
            {
                a1 = typeof(int),
                a5 = new ACE5()
                {
                    kk = 3,
                    str = "13214",
                    aCE3 = new ACE3()
                    {
                        kk = 14,
                        str = "serwte342",
                    }
                },
                a6 = a6,
                a7 = new object[] { typeof(int), a6 },
                //arrayArray3 = new int[] { 1, 2, 3, 4, 5 },
                //arrayArray4 = new int[][] {
                //    new int[] { 1, 2, 3, 4, 5 },
                //    new int[] { 11, 12, 13, 14, 15 },
                //    new int[] { 21, 22, 23, 24, 25 }
                //},
                //arrayArray1 = new int[,,] {
                //    {
                //        { 1, 2, 3, 4, 5 },
                //        { 11, 12, 13, 14, 15 },
                //        { 21, 22, 23, 24, 25 }
                //    },
                //    {
                //        { 101, 102, 103, 104, 105 },
                //        { 1011, 1012, 1013, 1014, 1015 },
                //        { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},
                //arrayArray2 = new int[,][] {
                //    {
                //        new int[] { 1, 2, 3, 4, 5 },
                //        new int[] { 11, 12, 13, 14, 15 },
                //        new int[] { 21, 22, 23, 24, 25 }
                //    },
                //    {
                //        new int[] { 101, 102, 103, 104, 105 },
                //        new int[] { 1011, 1012, 1013, 1014, 1015 },
                //        new int[] { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},
                //arrayArray5 = new int[,,] {
                //{
                //    { 1, 2, 3, 4, 5 },
                //    { 11, 12, 13, 14, 15 },
                //    { 21, 22, 23, 24, 25 }
                //    },
                //{
                //    { 1001, 1002, 1003, 1004, 1005 },
                //    { 1011, 1012, 1013, 1014, 1015 },
                //    { 1021, 1022, 1023, 1024, 1025 }
                //    }
                //},
            };


            JsonWriter jsonWriter = new JsonWriter(new WriterReflection());
            string dataStr = jsonWriter.Writer(ott2);

            Console.WriteLine(dataStr);

            JsonReader jsonRender = new JsonReader();
            var outData = jsonRender.ReadJsonTextCreate<Ott2>(dataStr);

            //bool isOk = Assert.AreEqualObject(outData, inputData);
            //Console.WriteLine(isOk);
            Console.WriteLine();

        }

        static unsafe void Main444(string[] args)
        {
            Stopwatch oTime = new Stopwatch();
            StreamReader streamReader = new StreamReader(@"TextFile1.json", Encoding.UTF32);
            string str = streamReader.ReadToEnd();
            CollectionManager.Start();//ReflectionToObject  AddrToObject2

            JsonReader jsonRender = new JsonReader();

            jsonRender.ReadJsonText(str);
            var wedso = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);

            StreamReader streamReader2 = new StreamReader(@"TextFile2.json", Encoding.UTF32);
            string strD = streamReader2.ReadToEnd();


            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 300000; __1++)
            {
                jsonRender.ReadJsonText(str);
            }
            oTime.Stop();
            double time001 = oTime.Elapsed.TotalMilliseconds;
            Console.WriteLine("1：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
            var o = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);


            Stopwatch oTime2 = new Stopwatch();
            oTime2.Reset(); oTime2.Start();
            for (int __1 = 0; __1 < 300000; __1++)
            {
                var ot = jsonRender.ReadJsonTextCreate<TestJsonClassA>(str);
            }

            //GC.Collect();
            oTime2.Stop();
            Console.WriteLine("C：{0} 毫秒", (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds);
            Console.WriteLine("2：{0} 毫秒", (oTime2.Elapsed).TotalMilliseconds);

            double time002 = (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds;
            Console.WriteLine(time002 / time001);

            Console.ReadKey();

            //AAXA aAXA = new AAXA();
            //var a1_p = GeneralTool.ObjectToVoidPtr(aAXA);
            //var o111 = new TypeAddrReflectionWrapper(typeof(AAXA));
            //string key = "Num\"";
            ////fixed (byte* o11 = &o11)
            //fixed (char* o11 = key)
            //{
            //    TypeAddrFieldAndProperty typeAddr = o111.Find(o11, key.Length - 1);
            //    if (typeAddr.isField)
            //    {

            //    }
            //    else if (typeAddr.isPropertySet)
            //    {
            //        *typeAddr.propertyDelegateItem.setTargetPtr = a1_p;
            //        typeAddr.propertyDelegateItem.setDouble(111.444);
            //    }
            //} 
            Console.ReadKey();
        }

        public struct ACE4
        {
            public int kk;
            public string str;
        }

        public class ACE3
        {
            public int kk;
            public string str;
        }

        public class ACE5
        {
            public int kk;
            public string str;
            public ACE3 aCE3;
            public ACE5(int kk, string str, ACE3 aCE3)
            {
                this.kk = kk;
                this.str = str;
                this.aCE3 = aCE3;
            }
            public ACE5()
            {
            }
            public string Foo3(int kk, string str, ACE3 aCE3)
            {
                return "草泥马";
            }
            public string Foo4(int kk, string str, ACE3 aCE3)
            {
                return "我是傻逼！";
            }
        }


        public unsafe class ACE1
        {
            public Type[] args;
            public enum TestEnum : byte
            {
                ddd,
                kkk,
                ffff
            }
            public TestEnum testEnum;
            public Type testType;
            public ACE3[] aCE3s;
            public object aCE3;
            public ACE5 aCE5;
            public void* dp;

            public ACE3 ACE3_2;

            public Func<int, string, ACE3, string> func;
            public bool b;
            public double num;
            public string str;
            // public ACE3 aCE3;
            public ACE4 aCE4;
            public ACE4[,] ffccc;

            public object ints;
            //public int[] ints;
            public List<ACE4> aCE4Lists;
            public Dictionary<ACE3, ACE4> aCE3Dictionary;
            public Dictionary<int, ACE4> aCE3Dictionary2;
            public Dictionary<string, ACE4> aCE3Dictionary4;
            public List<int> intLists;
            public Queue<int> intQueues;

            public int[,] ints2;
            public ACE3[,] aCE3s2;
            public int[,,] ints3;

            public string Foo1(int kk, string str, ACE3 aCE3)
            {
                return "给我跪下！叫我一身爷！";
            }
            public string Foo2(int kk, string str, ACE3 aCE3)
            {
                return "爷~~~ 我认输了，我是最垃圾的九级野怪！";
            }

        }

        [CollectionWriteAttribute(typeof(ACE5))]
        public unsafe class CreateWriter : IWriterCollectionObject
        {
            public JsonWriteType GetWriteType(object obj) { return JsonWriteType.Object; }
            public IEnumerable<KeyValueStruct> GetValue(object obj)
            {
                var collection = (ACE5)obj;

                if (collection.str != null)
                {
                    yield return new KeyValueStruct()
                    {
                        key = "#create",
                        value = new object[] { collection.kk, collection.str, collection.aCE3 },
                        type = typeof(object[]),
                    };
                }
            }
        }


        //[CollectionWriteAttribute(typeof(ACE5), false)]
        //public unsafe class ACE5Writer : IWriterObject
        //{
        //    public bool IsArray() { return false; }
        //    public IEnumerable<KeyValueStruct> GetValue(object obj)
        //    {
        //        var collection = (ACE5)obj;

        //        if (collection.str != null)
        //        {
        //            yield return new KeyValueStruct()
        //            {
        //                key = "str",
        //                value = collection.str,
        //                type = collection.str.GetType(),
        //            };
        //        }
        //        yield return new KeyValueStruct() {
        //            key = "kk",
        //            value = collection.kk,
        //            type = collection.kk.GetType(),
        //        };
        //        if (collection.aCE3 != null)
        //        {
        //            yield return new KeyValueStruct() {
        //                key = "aCE3",
        //                value = collection.aCE3,
        //                type = collection.aCE3.GetType(),
        //            };
        //        }
        //    }
        //}



        //class DictionString<T> : Dictionary<string, T> { }


        public class DictionString<Zero, T> : SpecialCaseGeneric where Zero : Dictionary<string, T> { }
       
         interface IGenericType
        {
            Type GetGenericType();
        }
        
        class DictionString2<T> : IGenericType {
            public Type GetGenericType() {
                return typeof(Dictionary<string, T>);
            }
        }




        public struct DDDDDccccfff
        {
        }
        static void K(Type type)
        { 
            Console.WriteLine("xxx " + typeof(List<>).MakeGenericType(type).TypeHandle.Value);
        }


        public unsafe delegate void ActionRef2<T>(ref T arg2);
        public unsafe delegate void ActionVoidPtr2<T>(void* arg1, T arg2);
        public unsafe delegate void ActionVoidPtr3(void* arg1);


        interface 接口
        {
            Delegate GetDelegate();
        }

        class 实现类 : 接口
        {
            public void Alex(ref V3 v3)
            {
                v3.x = 111;
            }
            public Delegate GetDelegate() {
                ActionRef2<V3> ac = (ref V3 v3) =>
                {
                    v3.x = 111;
                };
                return ac;
            }
        }

        class 实现类2 : 接口
        {
            public void Alex(ACE5 v3)
            {
                v3.kk = 111;
            }
            public Delegate GetDelegate()
            {
                Action<ACE5> ac = Alex;
                return ac;
            }
        }

        interface 接口2
        {
            void Call();
        }
        class 实现类3 : 接口2
        {
            public void Call()
            {
                //new ACE5();
            }
        }


        static Dictionary<IntPtr, Type> mainType1 = new Dictionary<IntPtr, Type>();
        static Dictionary<Type, Type> mainType2 = new Dictionary<Type, Type>();




        private static unsafe void TestXingNeng(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '{':
                        break;
                    case ' ':
                        break;
                    case '\t':
                        break;
                    case '\r':
                        break;
                    case '\n':
                        break;
                    case '"':
                        break;
                    case ':':
                        break;
                    case '}':
                        break;
                    case '[':
                        break;
                    case ']':
                        break;
                    case ',':
                        break;
                    default:
                        sb.Append(str[i]);
                        break;
                }
            }
        }
        public struct TestOB2
        {
            public P_box_3 p3_0;
            public P_box_3 p3_2;
            public int numI;
            public double num;
        }
        public struct TestOB
        {
            public P_box_3 p3_0;
            public P_box_3 p3_2;
            public int numI;
            public double num;
        }

        public struct P_box_3
        {
            public float x;
            public float y;
            public float z;
        }
        public struct P_3
        {
            public float x;
            public float y;
            public float z;
        }

        ///* 
        [CollectionRead(typeof(Program.TestOB2), false)]
        public unsafe class CollectionTestOB2 : CollectionObjectBase<TestOB2, TestOB2>
        {
            public override bool IsRef()
            {
                return false;
            }
            public override unsafe Type GetItemType(JsonObject* bridge)
            {
                string keystring = new string(bridge->keyStringStart, 0, bridge->keyStringLength);
                switch (keystring)
                {
                    case "p3_0":
                        return typeof(P_box_3);
                    case "p3_2":
                        return typeof(P_box_3);
                    case "numI":
                        return typeof(int);
                    case "num":
                        return typeof(double);
                }
                return typeof(float);
            }

            protected override unsafe void Add(TestOB2 obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
            {
                string keystring = new string(key, 0, keyLength);
                switch (keystring)
                {
                    case "p3_0":
                        obj.p3_0 = (P_box_3)value;
                        break;
                    case "p3_2":
                        obj.p3_2 = (P_box_3)value;
                        break;
                }
            }

            protected override unsafe void AddValue(TestOB2 obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
            {
                string keystring = new string(key, 0, keyLength);
                switch (keystring)
                {
                    case "numI":
                        if (value->type == JsonValueType.Long)
                        {
                            obj.numI = (int)value->valueLong;
                        }
                        else
                        {
                            obj.numI = (int)value->valueDouble;
                        }
                        break;
                    case "num":
                        if (value->type == JsonValueType.Long)
                        {
                            obj.num = value->valueLong;
                        }
                        else
                        {
                            obj.num = value->valueDouble;
                        }
                        break;
                }
            }

            protected override TestOB2 CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
            {
                return new TestOB2();
            }

            protected override TestOB2 End(TestOB2 obj)
            {
                return obj;
            }
        }

        [CollectionRead(typeof(Program.TestOB), false)]
        public unsafe class CollectionTestOB : CollectionObjectStructBase<TestOB>
        {
            public override unsafe Type GetItemType(JsonObject* bridge)
            {
                string keystring = new string(bridge->keyStringStart, 0, bridge->keyStringLength);
                switch (keystring)
                {
                    case "p3_0":
                        return typeof(P_box_3);
                    case "p3_2":
                        return typeof(P_box_3);
                    case "numI":
                        return typeof(int);
                    case "num":
                        return typeof(double);
                }
                return typeof(float);
            }

            protected override unsafe void Add(Box<TestOB> obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
            {
                string keystring = new string(key, 0, keyLength);
                switch (keystring)
                {
                    case "p3_0":
                        obj.value.p3_0 = (P_box_3)value;
                        break;
                    case "p3_2":
                        obj.value.p3_2 = (P_box_3)value;
                        break;
                }
            }

            protected override unsafe void AddValue(Box<TestOB> obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
            {
                string keystring = new string(key, 0, keyLength);
                switch (keystring)
                {
                    case "numI":
                        if (value->type == JsonValueType.Long)
                        {
                            obj.value.numI = (int)value->valueLong;
                        }
                        else
                        {
                            obj.value.numI = (int)value->valueDouble;
                        }
                        break;
                    case "num":
                        if (value->type == JsonValueType.Long)
                        {
                            obj.value.num = value->valueLong;
                        }
                        else
                        {
                            obj.value.num = value->valueDouble;
                        }
                        break;
                }
            }
        }



        //[Collection(typeof(Program.P_3), false)]
        //public unsafe class CollectionP_3X : CollectionObjectBase<P_3>
        //{
        //    public override unsafe Type GetItemType(char* key, int keyLength)
        //    {
        //        return typeof(float);
        //    }

        //    public override unsafe void AddValue(object obj, char* key, int keyLength, char* str, JsonValue* value)
        //    {
        //        P_3* p = (P_3*)GeneralTool.ObjectToVoidPtr(obj);
        //        switch (value->type)
        //        {
        //            case JsonValueType.Long:
        //                switch (*key)
        //                {
        //                    case 'x':
        //                        p->x = (float)value->valueLong;
        //                        break;
        //                    case 'y':
        //                        p->y = (float)value->valueLong;
        //                        break;
        //                    case 'z':
        //                        p->z = (float)value->valueLong;
        //                        break;
        //                }
        //                break;
        //            case JsonValueType.Double:
        //                switch (*key)
        //                {
        //                    case 'x':
        //                        p->x = (float)value->valueDouble;
        //                        break;
        //                    case 'y':
        //                        p->y = (float)value->valueDouble;
        //                        break;
        //                    case 'z':
        //                        p->z = (float)value->valueDouble;
        //                        break;
        //                }
        //                break;
        //        }
        //    }

        //}


        [CollectionRead(typeof(Program.P_box_3), false)]
        public unsafe class CollectionP_3 : CollectionObjectBase<Program.P_box_3, Box<P_box_3>>
        {
            public override unsafe Type GetItemType(JsonObject* bridge)
            {
                return typeof(float);
            }

            protected override unsafe void Add(Box<P_box_3> obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
            {
            }

            protected override unsafe void AddValue(Box<P_box_3> obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
            {
                switch (value->type)
                {
                    case JsonValueType.Long:
                        switch (*key)
                        {
                            case 'x':
                                obj.value.x = (float)value->valueLong;
                                break;
                            case 'y':
                                obj.value.y = (float)value->valueLong;
                                break;
                            case 'z':
                                obj.value.z = (float)value->valueLong;
                                break;
                        }
                        break;
                    case JsonValueType.Double:
                        switch (*key)
                        {
                            case 'x':
                                obj.value.x = (float)value->valueDouble;
                                break;
                            case 'y':
                                obj.value.y = (float)value->valueDouble;
                                break;
                            case 'z':
                                obj.value.z = (float)value->valueDouble;
                                break;
                        }
                        break;
                }
            }

            protected override Box<P_box_3> CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
            {
                var obj2 = new Box<P_box_3>();
                obj2.value = new P_box_3();
                return obj2;
            }

            protected override P_box_3 End(Box<P_box_3> obj)
            {
                return obj.value;
            }

            public class V3_
            {
                public Program.P_box_3 v3;
            }
          
        }



        [ReadCollection(typeof(Program.V3), true)]
        public unsafe class CollectionArrayV3 : CreateTaget<ReadCollectionLink>
        {
            CollectionManager.TypeAllCollection collection;
            public ReadCollectionLink Create()
            {
                //collection = CollectionManager.GetTypeCollection(typeof(float));
                ReadCollectionLink read = new ReadCollectionLink();
                read.isLaze = false;

                read.addValueStructDelegate = (AddValue_)AddValue;
                read.addValueClassDelegate = (AddValueObject_)AddValueObject;
                
                read.createObject = CreateObject;
                read.createStructDelegate = (CreateValue_)CreateValue;
                //read.getItemType = GetItemType;
                return read;
            }


            delegate void AddValue_(ref V3 v3, ReadCollectionLink.AddValue_Args arg);
            void AddValue(ref V3 v3, ReadCollectionLink.AddValue_Args arg)
            {
                switch (arg.value->type)
                {
                    case JsonValueType.Long:
                        switch (arg.value->arrayIndex)
                        {
                            case 0:
                                v3.x = (float)arg.value->valueLong;
                                break;
                            case 1:
                                v3.y = (float)arg.value->valueLong;
                                break;
                            case 2:
                                v3.z = (float)arg.value->valueLong;
                                break;
                        }
                        break;
                    case JsonValueType.Double:
                        switch (arg.value->arrayIndex)
                        {
                            case 0:
                                v3.x = (float)arg.value->valueDouble;
                                break;
                            case 1:
                                v3.y = (float)arg.value->valueDouble;
                                break;
                            case 2:
                                v3.z = (float)arg.value->valueDouble;
                                break;
                        }
                        break;
                }
            }

            delegate void AddValueObject_(Box<V3> v3, ReadCollectionLink.AddValue_Args arg);
            void AddValueObject(Box<V3> v3, ReadCollectionLink.AddValue_Args arg)
            {
                switch (arg.value->type)
                {
                    case JsonValueType.Long:
                        switch (arg.value->arrayIndex)
                        {
                            case 0:
                                v3.value.x = (float)arg.value->valueLong;
                                break;
                            case 1:
                                v3.value.y = (float)arg.value->valueLong;
                                break;
                            case 2:
                                v3.value.z = (float)arg.value->valueLong;
                                break;
                        }
                        break;
                    case JsonValueType.Double:
                        switch (arg.value->arrayIndex)
                        {
                            case 0:
                                v3.value.x = (float)arg.value->valueDouble;
                                break;
                            case 1:
                                v3.value.y = (float)arg.value->valueDouble;
                                break;
                            case 2:
                                v3.value.z = (float)arg.value->valueDouble;
                                break;
                        }
                        break;
                }
            }


            object CreateObject(out object temp, ReadCollectionLink.Create_Args arg)
            {
                temp = null;
                return new V3();
            }

            delegate void CreateValue_(ref V3 v3, out object temp, ReadCollectionLink.Create_Args arg);

            void CreateValue(ref V3 v3, out object temp, ReadCollectionLink.Create_Args arg)
            {
                temp = null;
                v3 = new V3();
            }
            //CollectionManager.TypeAllCollection GetItemType(ReadCollectionLink.GetItemType_Args arg)
            //{
            //    return  collection;
            //}
        }



        //[CollectionRead(typeof(Program.V3), true)]
        //public unsafe class CollectionArrayV3 : CollectionArrayBase<Program.V3, CollectionArrayV3.V3_>
        //{
        //    public class V3_
        //    {
        //        public Program.V3 v3;
        //    }
        //    protected override void AddValue(V3_ obj, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
        //    {
        //        switch (value->type)
        //        {
        //            case JsonValueType.Long:
        //                switch (index)
        //                {
        //                    case 0:
        //                        obj.v3.x = (float)value->valueLong;
        //                        break;
        //                    case 1:
        //                        obj.v3.y = (float)value->valueLong;
        //                        break;
        //                    case 2:
        //                        obj.v3.z = (float)value->valueLong;
        //                        break;
        //                }
        //                break;
        //            case JsonValueType.Double:
        //                switch (index)
        //                {
        //                    case 0:
        //                        obj.v3.x = (float)value->valueDouble;
        //                        break;
        //                    case 1:
        //                        obj.v3.y = (float)value->valueDouble;
        //                        break;
        //                    case 2:
        //                        obj.v3.z = (float)value->valueDouble;
        //                        break;
        //                }
        //                break;
        //        }
        //    }
        //    protected override V3_ CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        //    {
        //        return new V3_();
        //    }
        //    protected override V3 End(V3_ obj)
        //    {
        //        return obj.v3;
        //    }
        //    public override Type GetItemType(int index)
        //    {
        //        return typeof(float);
        //    }

        //    protected override void Add(V3_ obj, int index, object value, ReadCollectionProxy proxy)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //   */



        public class TClass001 {
            public object[] objects;
            public double TestDuble;
            public TClass002 tClass002;
            public TClass002[] tClass002s;
        }
        public class TClass002
        {
            public double size;
            public string testString;
            public TClass003 tClass003;
            public TClass003[] tClass003s;
        }
        public class TClass003
        {
            public string testString;
        }

        public class TclassDCC
        {
            public TclassDCC(string str, IList<int> array, double num) {
                this.str = str;
                this.array = array;
                this.num = num;
            }
            public string str;
            public IList<int> array;
            public double num;
        }

        public class TestJsonClassA
        {
            public Action<int, string> testDelegate2;
            ///*
            private double num;
            public double Num
            {
                get { return num; }
                set { num = value; }
            }

            private B bb;
            public B BB
            {
                get { return bb; }
                set { bb = value; }
            }

            public V3 V32 { get => v32; set => v32 = value; }
            private V3 v32;


            public LinkedList<long> arrayLinkedList;
            public int[,,] arrayArray1;
            public int[,][] arrayArray2;
            public int[] arrayArray3;
            public int[][] arrayArray4;
            public int[,,] arrayArray5;

            public List<B> listB;
            public List<C> listC;
            public List<E> listE;

            public bool b;
            public int kk;
            public string str;
            public C gcc;
            public E gD;
            public V3 v3;
            
            public TestOB testOB;

            public TclassDCC dcc;

            public object ddd;
            public object[] objects;
            public TClass001 tClass001;
            public TClass003 testClassDD;
            public TClass003 testClassDD2;
            public TClass003 testClassDD3;
            public TClass003 testClassDD4;
            public TestEnum testEnum;
            public TestEnum[] testEnums;

            public IList<int> arrayint2;
            public TclassA Iclass0Z;
            public P_box_3 p3;
            public B[,,] arrayRekn;
            public Stack<int> arrayStack;
            public B[] fd;
            public HashSet<double> arraydouble;
            public Queue<string> arraystring;
            public List<bool> arraybool;
            public Dictionary<int, string> dictionary1;
            public Dictionary<string, B> dictionary2;
            public Dictionary<V3, B> dictionary3;
        }



        public class TclassA
        {
            public bool b;
        }
        public class TclassC : TclassA
        {
            public double value;
            public B bbb;
        }


        public struct C
        {
            public double k;
            public B bbb;
        }
        public struct B
        {
            public bool b;
            public double num;
            public string str;
        }

        public struct V3
        {
            public float x;
            public float y;
            public float z;
        }



        [StructLayout(LayoutKind.Explicit)]
        struct D
        {
            [FieldOffset(0)]
            public bool b;

            [FieldOffset(1)]
            public double k;

            [FieldOffset(16)]
            public string str;

            [FieldOffset(40)]
            public E e;
        }


        public class E
        {
            public bool b;
            public double k;
            public string str;
        }

    }
}
