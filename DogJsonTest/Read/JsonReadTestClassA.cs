using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DogJson;
using NUnit.Framework;

namespace DogJsonTest.Read
{
    public class JsonReadTestClassA
    {
        [Test]
        public void ReadClassTestJsonClassA()
        {
            TestJsonClassA test1 = new TestJsonClassA()
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
                gcc = new C
                {
                    k = 12,
                    bbb = {
                        str = "cc",
                        b = true,
                        num = -8.56
                    }
                },
                p3 = {
                    x = 2,
                    y = 1,
                    z = 33
                },
                testOB = {
                    numI = 13,
                    num = 3.6,
                    p3_0 = new P_box_3 {
                        x = 1.2f,
                        y = -1.8f,
                        z = 33
                    },
                    p3_2 = {
                        x = -12,
                        y = 18,
                        z = 3.3f
                    }
                },
                gD = new E
                {
                    k = -3.14E-12,
                    str = "3特瑞aV",
                    b = true
                },
                arrayArray3 = new int[] { 1, 2, 3, 4, 5 },
                arrayArray4 = new int[][] {
                    new int[] { 1, 2, 3, 4, 5 },
                    new int[] { 11, 12, 13, 14, 15 },
                    new int[] { 21, 22, 23, 24, 25 }
                },
                arrayArray1 = new int[,,] {
                    {
                        { 1, 2, 3, 4, 5 },
                        { 11, 12, 13, 14, 15 },
                        { 21, 22, 23, 24, 25 }
                    },
                    {
                        { 101, 102, 103, 104, 105 },
                        { 1011, 1012, 1013, 1014, 1015 },
                        { 1021, 1022, 1023, 1024, 1025 }
                    }
                },
                arrayArray2 = new int[,][] {
                    {
                        new int[] { 1, 2, 3, 4, 5 },
                        new int[] { 11, 12, 13, 14, 15 },
                        new int[] { 21, 22, 23, 24, 25 }
                    },
                    {
                        new int[] { 101, 102, 103, 104, 105 },
                        new int[] { 1011, 1012, 1013, 1014, 1015 },
                        new int[] { 1021, 1022, 1023, 1024, 1025 }
                    }
                },
                arrayArray5 = new int[,,] {
                {
                    { 1, 2, 3, 4, 5 },
                    { 11, 12, 13, 14, 15 },
                    { 21, 22, 23, 24, 25 }
                    },
                {
                    { 1001, 1002, 1003, 1004, 1005 },
                    { 1011, 1012, 1013, 1014, 1015 },
                    { 1021, 1022, 1023, 1024, 1025 }
                    }
                },

                ddd = (System.Double)(-3.14E-12),
                objects = new object[] {
                    (System.Int32)(12),
                    (System.Double)(-3.14E-12)
                },
                testEnums = new TestEnum[] { TestEnum.Test002, TestEnum.Test004, TestEnum.Test003 },
                testEnum = TestEnum.Test002,


                listB = new List<B> {
                    new B {
                        str = "00001",
                        b = true,
                        num = -8.56
                    },
                    new B {
                        str = "aaaa2",
                        b = false,
                        num = 10000888.999
                    },
                },

                arrayLinkedList = new LinkedList<long>(new List<long> { 1L, 2L, 3L, 4L, 7L }),// { 1L, 2l, 3l, 4l, 7l }

                arrayStack = new Stack<int>(new List<int> { 3, 4, 5 }),

                arraydouble = new HashSet<double>() {
                    3.333,
                    -4.8888,
                    -5.34E+108
                },
                arraystring = new Queue<string>(
                   new List<string> {
                       "true",
                       "null",
                       "ed false"
                   }
                ),
                arraybool = new List<bool>() {
                    false,
                    true,
                    false
                },

                arrayint2 = new List<System.Int32> { 14, 24, 34, 44, 54 },

                fd = new B[] {
                    new B {
                        b = true,
                        num = -3.14E-12,
                        str = "ddd"
                    },
                    new B {
                        b = false,
                        num = 34.5
                    },
                    new B {
                        num = 999999
                    }
                },

                dcc = new TclassDCC("3213.#$%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),

                Iclass0Z = new DogJsonTest.Read.JsonReadTestClassA.TclassC
                {
                    b = 122,
                    value = 1.444,
                    bbb = new B
                    {
                        b = true,
                        num = -3.14E-12,
                        str = "hello world"
                    }
                },
                v3 = new V3(3, 2, 1),
                testDelegate2 = TClassA.Fool,

                arrayRekn = new B[,,]
                {
                    {
                        {
                            new B {
                                num = 1
                            },
                            new B {
                                num = 2,
                                str = "ddd"
                            }
                        },
                        {
                            new B {
                                num = 3
                            },
                            new B {
                                num = 4
                            }
                        }
                    },
                    {
                        {
                            new B {
                                num = 5,
                                str = "5"
                            },
                            new B {
                                num = 6,
                                str = "6"
                            }
                        },
                        {
                            new B {
                                num = 7,
                                str = "7"
                            },
                            new B {
                                num = 8,
                                str = "8"
                            }
                        }
                    },
                    {
                        {
                            new B {
                                num = 9,
                                str = "9"
                            },
                            new B {
                                num = 10,
                                str = "10"
                            }
                        },
                        {
                            new B {
                                num = 11,
                                str = "11"
                            },
                            new B {
                                num = 12,
                                str = "12"
                            }
                        }
                    }
                },

                tClass001 = new TClass001 {
                    objects = new object[] {
                        (System.Int32)12,
                        -(System.Double)3.14E-12,
                        (System.String)"3213.#$%^&*()",
                    },
                    tClass002s = new TClass002[] {
                        new TClass002 {
                            size = 1,
                            testString = "A@0"
                        },
                        new TClass002 {
                            size = 2,
                            testString = "A@1",
                            tClass003 = new TClass003 {
                                testString = "testClassDD4"
                            },
                            tClass003s = new TClass003[] {
                                new TClass003 {
                                    testString = "0000"
                                },
                                new TClass003 {
                                    testString = "0001"
                                },
                                new TClass003 {
                                    testString = "testClassDD3"
                                }
                            }
                        },
                        new TClass002 {
                            size = 3,
                            testString = "asdede"
                        }
                    },

                    tClass002 = new TClass002
                    {
                        size = 12,
                        tClass003 = new TClass003
                        {
                            testString = "testClassDD"
                        },
                        tClass003s = new TClass003[] {
                            new TClass003 {
                                testString = "0000"
                            },
                            new TClass003 {
                                testString = "0001"
                            },
                            new TClass003 {
                                testString = "testClassDD2"
                            },
                            new TClass003 {
                                testString = "0003"
                            }
                        },

                        testString = "A@2"
                    },
                    TestDuble = -3.14E-12
                },

                dictionary3 = new Dictionary<V3, B>()
                {
                    {
                       new V3( 3, 2, 1 ),
                       new B {
                            num = 11111,
                            str= "rradads"
                        }
                    },

                    {
                       new V3( 1, 4, -9.8f  ),
                       new B {
                            num = 888,
                            str= "热热我"
                        }
                    },

                    {
                       new V3( -3.2E-13f, 3.0E+13f, 0.99f ),
                       new B {
                            num = 999999,
                            str= "特别强势人物了"
                        }
                    }
                },
                tstructA = new TstructA {
                    value = 3.6,
                    b = new TstructB
                    {
                        kk = "FC",
                        c = new TstructC
                        {
                            Id = 21443,
                        }
                    }
                }


            };

            test1.testClassDD = test1.tClass001.tClass002.tClass003;
            test1.testClassDD4 = test1.tClass001.tClass002s[1].tClass003;
            test1.testClassDD2 = test1.tClass001.tClass002.tClass003s[2];
            test1.testClassDD3 = test1.tClass001.tClass002s[1].tClass003s[2];

            var kkk = Assembly.GetAssembly(typeof(TclassC));
            //
            test1.testDelegate2 += TClassA.Foo2;


            JsonManager.Start(new AddrToObject2());//ReflectionToObject
            JsonRender jsonRender = new JsonRender();

            string testPath = Path.GetDirectoryName(typeof(JsonRender).Assembly.Location) + @"\JsonFile\" + nameof(ReadClassTestJsonClassA) + ".json";
            string data = File.ReadAllText(testPath, Encoding.Unicode);

            TestJsonClassA o = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(data);

            Assert.AreEqualObject(o, test1);
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
            public Dictionary<int, B> dictionary2;
            public Dictionary<V3, B> dictionary3;
            //*/
            public TstructA tstructA;
        }

        public class TclassA
        {
            public int b;
        }

        public class TclassC : TclassA
        {
            public double value;
            B _bbb;
            public B bbb { get => _bbb; set => _bbb = value; }
        }


        public struct TstructA 
        {
            public double value;
            TstructB _b;
            public TstructB b { get => _b; set => _b = value; }
        }

        public struct TstructB
        {
            public string kk;
            TstructC _c;
            public TstructC c { get => _c; set => _c = value; }
        }


        public struct TstructC
        {
            private int id;

            public int Id { get => id; set => id = value; }
        }


        public enum TestEnum
        {
            Test001,
            Test002,
            Test003,
            Test004,
        }
        public class TClass001
        {
            public object[] objects;
            private double testDuble;
            public double TestDuble {
                get { return testDuble; }
                set { testDuble = value; }
            }
            public TClass002 tClass002;
            private TClass002[] _tClass002s;
            public TClass002[] tClass002s { get => _tClass002s; set => _tClass002s = value; }
        }
        public class TClass002
        {
            public double size;
            public string testString;
            private TClass003 _tClass003;
            public TClass003 tClass003 { get => _tClass003; set => _tClass003 = value; }
            //public TClass003 tClass003;
            public TClass003[] tClass003s;

        }
        public class TClass003
        {
            public string testString;
        }
        public class TclassDCC
        {
            public TclassDCC(string str, IList<int> array, double num)
            {
                this.str = str;
                this.array = array;
                this.num = num;
            }
            public string str;
            public IList<int> array;
            public double num;
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
            private P_box_3 _p3_0;
            public P_box_3 p3_2;
            public int numI;
            public double num;

            public P_box_3 p3_0 { get => _p3_0; set => _p3_0 = value; }
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

        public class TClassA
        {
            public static void Fool(int a, string b)
            {

            }
            public static void Foo2(int a, string b)
            {

            }

        }
        public struct V3
        {
            public V3 (float x, float y, float z) 
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public float x;
            public float y;
            public float z;
            public override bool Equals(object obj)
            {
                if (obj is V3)
                {
                    V3 v = (V3)obj;
                    return v.x == x && v.y == y && v.z == z;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return (x + y + z).GetHashCode();
            }
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

        [Collection(typeof(V3), true)]
        public unsafe class CollectionArrayV3 : CollectionArrayBase<V3, CollectionArrayV3.V3_>
        {
            public class V3_
            {
                public V3 v3;
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

        [Collection(typeof(TestOB), false)]
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


    }
}
