using System;
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

namespace DogJson
{

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


        static unsafe bool Find(char* path, int pathLength, JsonRender jsonRender, ref JsonObject* obj)
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
            JsonRender jsonRender = new JsonRender();
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

        static unsafe void Main44(string[] args)
        {
            StreamReader streamReader = new StreamReader(@"TextFile1.json", Encoding.UTF32);
            string str = streamReader.ReadToEnd();
            JsonManager.Start(new AddrToObject2());//ReflectionToObject  AddrToObject2

            JsonRender jsonRender = new JsonRender();

            jsonRender.ReadJsonText(str);
            var o = jsonRender.ReadJsonTextCreateObject<AAXA>(str);

            Console.ReadKey();


            //AAXA aAXA = new AAXA();
            //var a1_p = GeneralTool.ObjectToVoid(aAXA);
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


        public class ACE3
        {
            public int kk;
            public string str;
        }

        public class ACE1
        {
            public ACE3[] aCE3s;
            public bool b;
            public double num;
            public string str;
            public ACE3 aCE3;
            public int[] ints;
            public List<int> intLists;
            public int[,] ints2;
            public ACE3[,] aCE3s2;
            public int[,,] ints3;
        }


        static unsafe void Main(string[] args)
        {
            StreamReader streamReader = new StreamReader(@"TextFile1.json", Encoding.UTF32);
            string str = streamReader.ReadToEnd();
            JsonManager.Start(new AddrToObject2());//ReflectionToObject  AddrToObject2

            JsonRender jsonRender = new JsonRender();

            //jsonRender.ReadJsonText(str);
            //var oCCCCCCCCCCC = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);


            JsonWriter jsonWriter = new JsonWriter();
            ACE1 aCE = new ACE1();
            aCE.aCE3 = new ACE3
            {
                kk = 44,
                str = "E4",
            };

            aCE.intLists = new List<int>() { 3, 4, 5 };

            aCE.ints = new int[] {
                1,2,3,9
            };
            
            aCE.ints2 = new int[,]
            {
                {1, 2, 3, 4},
                {11, 12, 13, 14},
                {101, 102, 103, 104},
            };
            aCE.ints3 = new int[,,]
            {
                 {
                    {1, 2, 3, 4},
                    {11, 12, 13, 14},
                    {101, 102, 103, 104},
                },
                 {
                    {2001, 2002, 2003, 2004},
                    {2011, 2012, 2013, 2014},
                    {2101, 2102, 2103, 2104},
                },
            };
            aCE.aCE3s2 = new ACE3[,]
            {
                 {
                   new ACE3{
                kk = 4,
                str = "1", },
                   new ACE3{
                kk = 3,
                str = "2", },
                },
                 {
                   new ACE3{
                kk = 2,
                str = "3", },
                   new ACE3{
                kk = 1,
                str = "4", },
                },
            };

            aCE.aCE3s = new ACE3[] {
                new ACE3{
                kk = 4,
                str = "eewqr", },
                new ACE3{
                kk = 2,
                str = "DE4", },
                new ACE3{
                kk = 1,
                str = "F3", }
            };


            aCE.b = false;
            aCE.num = 4.5;
            aCE.str = "CCCCD";

            List<JsonWriteValue> writers = new List<JsonWriteValue>();
            writers = jsonWriter.Wirter(aCE);


            JsonWriteValue root = new JsonWriteValue();
            root.type = JsonWriteType.Object;
            root.isLast = true;

            JsonWriteValue item = new JsonWriteValue();

            StringBuilder sb = new StringBuilder();
            Stack<JsonWriteValue> objStack = new Stack<JsonWriteValue>();
            sb.AppendLine("{");

            JsonWriteValue parent = writers[0];
            objStack.Push(parent);


            //for (int i = 1; i < writers.Count; i++)
            //{
            //    item = parent.back;
            //    parent = item;

            //    if (item.key != null)
            //    {
            //        sb.Append("\"" + item.key + "\": ");
            //    }
            //    sb.AppendLine(item.value);

            //}

            //Console.WriteLine(sb.ToString());
            //Console.ReadKey();



            for (int i = 1; i < writers.Count; i++)
            {
                sb.Append('\t', objStack.Count);
                item = parent.back;
                parent = item;

                if (item.key != null)
                {
                    sb.Append("\"" + item.key + "\": ");
                }

                switch (item.type)
                {
                    case JsonWriteType.String:
                        sb.Append(item.value);
                        if (item.isLast)
                        {
                            JsonWriteValue parentStack;
                            sb.AppendLine();
                            if (item.back == null)
                            {
                                while (objStack.Count > 0)//item.back != null &&  
                                {
                                    parentStack = objStack.Pop();
                                    sb.Append('\t', objStack.Count);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("}");
                                        }
                                        else
                                        {
                                            sb.AppendLine("]");
                                        }
                                    }
                                    else
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("},");
                                        }
                                        else
                                        {
                                            sb.AppendLine("],");
                                        }
                                    }
                                }
                                break;
                            }
                            else
                            {

                                while (objStack.Peek().parent != item.back.parent)//item.back != null &&  
                                {
                                    parentStack = objStack.Pop();
                                    sb.Append('\t', objStack.Count);

                                    if (parentStack.isLast)
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("}");
                                        }
                                        else
                                        {
                                            sb.AppendLine("]");
                                        }
                                    }
                                    else
                                    {
                                        if (parentStack.type == JsonWriteType.Object)
                                        {
                                            sb.AppendLine("},");
                                        }
                                        else
                                        {
                                            sb.AppendLine("],");
                                        }
                                    }
                                }
                                parentStack = objStack.Pop();
                                sb.Append('\t', objStack.Count);

                                if (parentStack.isLast)
                                {
                                    if (parentStack.type == JsonWriteType.Object)
                                    {
                                        sb.AppendLine("}");
                                    }
                                    else
                                    {
                                        sb.AppendLine("]");
                                    }
                                }
                                else
                                {
                                    if (parentStack.type == JsonWriteType.Object)
                                    {
                                        sb.AppendLine("},");
                                    }
                                    else
                                    {
                                        sb.AppendLine("],");
                                    }
                                }
                            }


                        }
                        else
                        {
                            sb.AppendLine(",");
                        }
                        break;
                    case JsonWriteType.Object:
                        sb.AppendLine("{");
                        objStack.Push(item);
                        break;
                    case JsonWriteType.Array:
                        sb.AppendLine("[");
                        objStack.Push(item);
                        break;
                    default:
                        break;
                }
            }

            //sb.AppendLine("}");
            Console.WriteLine(sb.ToString());
            Console.ReadKey();
        }



        static unsafe void MainZ(string[] args)
        {
            Stopwatch oTime = new Stopwatch();

            int tst = sizeof(MulticastDelegateValue);
            //Action<string[]> action2 = Main;
            //action2 += Main7;
            //Action<string[]> action3 = Main;
            //int* action2P = (int*)GeneralTool.ObjectToVoid(action2);
            //List<int> testaction2P = new List<int>();
            //for (int i = 0; i < 20; i++)
            //{
            //    testaction2P.Add(action2P[i]);
            //}
            //int size11 = UnsafeOperation.SizeOf(typeof(Action<string[]>));
            //int size112 = UnsafeOperation.SizeOf(typeof(Func<string[]>));
            //int size1123 = UnsafeOperation.SizeOf(typeof(Func<object, string[]>));
            //int size11234 = UnsafeOperation.SizeOf(typeof(MulticastDelegate));
            //int size12 = UnsafeOperation.SizeOfStack(typeof(Action<string[]>));

            A3 a1 = new A3();
            a1.Num = 999;

            PropertyInfo propertyInfo = typeof(A3).GetProperty("Num");
            PropertyInfo propertyInfo2 = typeof(A3).GetProperty("Struct");

            Action<A3, A3struct> setValue22 = Delegate.CreateDelegate(typeof(Action<A3, A3struct>),
                propertyInfo2.GetSetMethod()) as Action<A3, A3struct>;
            A3struct a3Struct1 = new A3struct();
            a3Struct1.num = 111;
            //setValue22(a1, a3Struct1);

            object box;
            if (typeof(A3).IsValueType)
            {
                box = new Box<A3>(a1);
            }
            else
            {
                box = a1;
            }

            IPropertyWrapper wrapper = PropertyWrapper.Create(typeof(A3), typeof(A3).GetProperty("Num"));
            wrapper.Set(box, 777.0);

            Console.WriteLine(wrapper.Get(box));

            Action<A3, double> setValue = Delegate.CreateDelegate(typeof(Action<A3, double>),
                typeof(A3).GetProperty("Num").GetSetMethod()) as Action<A3, double>;

            var getValue = Delegate.CreateDelegate(typeof(Action<A3, double>),
                typeof(A3).GetProperty("Num").GetSetMethod()) as Action<A3, double>;

            Action<double> setValue2 = Delegate.CreateDelegate(typeof(Action<double>),
                null, propertyInfo.GetSetMethod()) as Action<double>;

            

            var set2 = (Action<double>)
                propertyInfo.GetSetMethod().CreateDelegate(typeof(Action<double>), a1);

            A3struct a3Struct = new A3struct();
            a3Struct.num = 1999;
            var setValue24 = Delegate.CreateDelegate(typeof(Action<double>),
                a3Struct, typeof(A3struct).GetProperty("Num").GetSetMethod()) as Action<double>;
            a3Struct.num = 11;

            object objccc = a3Struct;
            var logp = (long*)GeneralTool.ObjectToVoid(setValue24);
            logp++;
            var logpO = GeneralTool.VoidToObject((long*)*logp);

            PropertyDelegateItem propertyWrapperAction2 = new PropertyDelegateItem();
            propertyWrapperAction2.Set = setValue24;
            *propertyWrapperAction2.setTarget = (IntPtr)GeneralTool.ObjectToVoid(objccc);
            propertyWrapperAction2.setDouble(777.0);


            var @this = 0;
            PropertyDelegateItem propertyWrapperAction = new PropertyDelegateItem();
            propertyWrapperAction.Set = setValue2;
            //propertyWrapperAction.get2 = setValue;


            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                // set2(777.0);
                setValue(a1, 777.0);
            }
            oTime.Stop();
            Console.WriteLine("属性委托赋值（不能做成通用反射，只做参考）：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

            var a1_p = GeneralTool.ObjectToVoid(a1);
            //bool isClass = true;
            //oTime.Reset(); oTime.Start();
            //for (int __1 = 0; __1 < 1000000; __1++)
            //{
            //    if (isClass)
            //    {
            //        propertyWrapperAction.get_double2(a1, 777.0);
            //    }
            //    else
            //    {
            //        *propertyWrapperAction.target = (IntPtr)a1_p;
            //        propertyWrapperAction.getDouble(777.0);
            //    }
            //}
            //oTime.Stop();
            //Console.WriteLine("属性指针赋值（引用类型和值类型分开处理）：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

            *propertyWrapperAction.setTargetPtr = a1_p;
            propertyWrapperAction.setDouble(424.9991);

            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                *propertyWrapperAction.setTargetPtr = a1_p;
                propertyWrapperAction.setDouble(777.0);
            }
            oTime.Stop();
            Console.WriteLine("属性指针赋值（引用类型和值类型同一处理）：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);


            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                *propertyWrapperAction.setTarget = (IntPtr)a1_p;
                propertyWrapperAction.setDouble(777.0);
            }
            oTime.Stop();
            Console.WriteLine("属性指针赋值（引用类型和值类型同一处理）2：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);



            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                wrapper.Set(box, 777.0); 
            }
            oTime.Stop();
            Console.WriteLine("属性接口赋值：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);


            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                a1.Num = 777.0;
            }
            oTime.Stop();
            Console.WriteLine("属性原生赋值：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 1000000; __1++)
            {
                propertyInfo.SetValue(a1, 777.0);
            }
            oTime.Stop();
            Console.WriteLine("属性反射赋值：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);




            //var setValue3 = Delegate.CreateDelegate(typeof(Action),
            //   777,
            //   setValue2.Method) as Action;




            //Console.WriteLine(getValue(a1));

            //Console.ReadKey();
            setValue(a1, 111);
            Console.WriteLine(a1.Num);


            var varddd = TestEnum.Test001.ToString();
            string nnnn = typeof(TestEnum).ToString();

            StreamReader streamReader = new StreamReader(@"TextFile1.json", Encoding.UTF32);
            string str = streamReader.ReadToEnd();
            JsonManager.Start(new AddrToObject2());//ReflectionToObject  AddrToObject2

            JsonRender jsonRender = new JsonRender();

            jsonRender.ReadJsonText(str);
            var o = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>( str);


            StreamReader streamReader2 = new StreamReader(@"TextFile2.json", Encoding.UTF32);
            string strD = streamReader2.ReadToEnd();


            oTime.Reset(); oTime.Start();
            for (int __1 = 0; __1 < 30000; __1++)
            {
                jsonRender.ReadJsonText(str);
            }
            oTime.Stop();
            double time001 = oTime.Elapsed.TotalMilliseconds;
            Console.WriteLine("1：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
            o = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);

            //string strd = File.ReadAllText("TextFile1.json");

            byte[] data = Encoding.Unicode.GetBytes(str);
            Stopwatch oTime2 = new Stopwatch();
            oTime2.Reset(); oTime2.Start();
            for (int __1 = 0; __1 < 30000; __1++)
            {
                var ot = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);
                //AddrToObject2.indexDbug++;
            }


            //GC.Collect();
            oTime2.Stop();
            Console.WriteLine("C：{0} 毫秒", (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds);
            Console.WriteLine("2：{0} 毫秒", (oTime2.Elapsed).TotalMilliseconds);
            //Console.WriteLine("2：{0} 毫秒", (oTime2.Elapsed).TotalMilliseconds);

            double time002 = (oTime2.Elapsed - oTime.Elapsed).TotalMilliseconds;
            Console.WriteLine(time002 / time001);


            Console.ReadKey();
            //System.Collections.CollectionBase

            /*
           //MulticastDelegate
            Type type1 = Type.GetType( "System.Int32");
            Type type2 = Type.GetType("DogJson.Program");

            Action<object> action = Foo1;
            action += Foo2;
            action += Foo3;

            Type type_action = action.GetType();
            var type_action2 = type_action.IsSubclassOf(typeof(Delegate));
            TypeCode typeCode = Type.GetTypeCode(type_action);


            MulticastDelegate multicast = action;
            var ds = action.GetMethodInfo(); 
            var invocationList = action.GetInvocationList(); 

            ParameterModifier parameterModifier = new ParameterModifier(1);
            parameterModifier[0] = false;

            MethodInfo methodInfo = typeof(Program).GetMethod("Foo1",
                BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(string) },
               null
             );


            //Console.WriteLine(methodInfo);


            //MethodInfo methodInfo = typeof(Program).GetMethod("Foo1", BindingFlags.NonPublic | BindingFlags.Instance
            //    | BindingFlags.Public | BindingFlags.Static);


            //var parameters = methodInfo.ReturnParameter;      
            ////Void Main(System.String[])

            Console.WriteLine(ds);//Void Main(System.String[])
            //LinkedList<int> ts = new LinkedList<int>();
            //ts.AddLast
            Console.ReadKey();

           B[]  bs = new B[10];
           bs[0].num = 133.5555; 
            bs[0].str = "qeqdasd";
            bs[1].str = "55555";
           bs[1].num = 133.5555;
           //Test001class.Run();
           IntPtr intPtr = MeasureArrayAdjustment<B>();
           Pinnable<B> _pinnable = Unsafe.As<Pinnable<B>>(bs);
           B v1143234 =  GetPinnableReference<B>(_pinnable, new IntPtr(intPtr.ToInt32()  ));
           Console.WriteLine(v1143234.str);
           //Console.WriteLine(((char)96));//49
           //Test001class.Run();

           str = str.Replace("\t", "");
           //ReadJsonData readJsonData = new ReadJsonData(); 
            Stopwatch oTime = new Stopwatch();

           fixed (char* ptr = str)
           {
               oTime.Reset(); oTime.Start();
               for (int i = 0; i < 1000000; i++)
               {
                   SpanString spanString = new SpanString(ptr, 10,9);
               }
               oTime.Stop();
               Console.WriteLine("spanString ：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

               oTime.Reset(); oTime.Start();
               for (int i = 0; i < 1000000; i++)
               {
                   string sstring = new string(ptr, 10, 9);
               }
               oTime.Stop();
               Console.WriteLine(" string：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

               oTime.Reset(); oTime.Start();
               for (int i = 0; i < 1000000; i++)
               {
                   string sstring = new string(ptr, 10, 9);
               }
               oTime.Stop();
               Console.WriteLine(" string：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
           }

        //   */



            //oTime.Reset(); oTime.Start();
            //for (int i = 0; i < testCount; i++)
            //{
            //    jsonRender.ReadJsonText(str);
            //}
            //oTime.Stop();

            //Console.WriteLine("ReadJsonText：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);

            //A obj;
            //oTime.Reset(); oTime.Start();
            //for (int i = 0; i < testCount; i++)
            //{
            //    //obj = (A)jsonRender.ToObject(typeof(A), str);
            //}
            //oTime.Stop();
            //Console.WriteLine("ToObject：{0} 毫秒", oTime.Elapsed.TotalMilliseconds);
            Console.ReadKey();
        }

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
        [Collection(typeof(Program.TestOB2), false)]
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

            protected override unsafe void Add(TestOB2 obj, char* key, int keyLength, object value)
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

            protected override unsafe void AddValue(TestOB2 obj, char* key, int keyLength, char* str, JsonValue* value)
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

        [Collection(typeof(Program.TestOB), false)]
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

            protected override unsafe void Add(Box<TestOB> obj, char* key, int keyLength, object value)
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

            protected override unsafe void AddValue(Box<TestOB> obj, char* key, int keyLength, char* str, JsonValue* value)
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
        //        P_3* p = (P_3*)GeneralTool.ObjectToVoid(obj);
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


        [Collection(typeof(Program.P_box_3), false)]
        public unsafe class CollectionP_3 : CollectionObjectBase<Program.P_box_3, Box<P_box_3>>
        {
            public override unsafe Type GetItemType(JsonObject* bridge)
            {
                return typeof(float);
            }

            protected override unsafe void Add(Box<P_box_3> obj, char* key, int keyLength, object value)
            {
            }

            protected override unsafe void AddValue(Box<P_box_3> obj, char* key, int keyLength, char* str, JsonValue* value)
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


        [Collection(typeof(Program.V3), true)]
        public unsafe class CollectionArrayV3 : CollectionArrayBase<Program.V3, CollectionArrayV3.V3_>
        {
            public class V3_
            {
                public Program.V3 v3;
            }
            protected override void AddValue(V3_ obj, int index, char* str, JsonValue* value)
            {
                switch (value->type)
                {
                    case JsonValueType.Long:
                        switch (index)
                        {
                            case 0:
                                obj.v3.x = (float)value->valueLong;
                                break;
                            case 1:
                                obj.v3.y = (float)value->valueLong;
                                break;
                            case 2:
                                obj.v3.z = (float)value->valueLong;
                                break;
                        }
                        break;
                    case JsonValueType.Double:
                        switch (index)
                        {
                            case 0:
                                obj.v3.x = (float)value->valueDouble;
                                break;
                            case 1:
                                obj.v3.y = (float)value->valueDouble;
                                break;
                            case 2:
                                obj.v3.z = (float)value->valueDouble;
                                break;
                        }
                        break;
                }
            }
            protected override V3_ CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
            {
                return new V3_();
            }
            protected override V3 End(V3_ obj)
            {
                return obj.v3;
            }
            public override Type GetItemType(int index)
            {
                return typeof(float);
            }

            protected override void Add(V3_ obj, int index, object value)
            {
                throw new NotImplementedException();
            }
        }
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
            public double Num
            {
                get { return num; }
                set { num = value; }
            }
            public double num;

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
            public Action<int, string> testDelegate2;
            public Stack<int> arrayStack;
            public B[] fd;
            public HashSet<double> arraydouble;
            public Queue<string> arraystring;
            public List<bool> arraybool;
            public Dictionary<int, string> dictionary1;
            public Dictionary<int, B> dictionary2;
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
