using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogJsonTest.Read;
using DogJson;
using System.Diagnostics;

namespace DogJsonTest
{
    public class Program
    {

        public static unsafe void Main(string[] args)
        {
            //object vv = Enum.Parse(typeof(DCC2), "n3");
            //DCC2 cc = DCC2.n3 | DCC2.n2;
            //object inEnum = cc;
            //var ty = inEnum.GetType();
            //var typeCode = Type.GetTypeCode(ty);
            //long data = 0;
            //switch (typeCode)
            //{
            //    case TypeCode.SByte:
            //        data = (long)(SByte)inEnum;
            //        break;
            //    case TypeCode.Byte:
            //        data = (long)(Byte)inEnum;
            //        break;
            //    case TypeCode.Int16:
            //        data = (long)(Int16)inEnum;
            //        break;
            //    case TypeCode.UInt16:
            //        data = (long)(UInt16)inEnum;
            //        break;
            //    case TypeCode.Int32:
            //        data = (long)(Int32)inEnum;
            //        break;
            //    case TypeCode.UInt32:
            //        data = (long)(UInt32)inEnum;
            //        break;
            //    case TypeCode.Int64:
            //        data = (long)(Int64)inEnum;
            //        break;
            //    case TypeCode.UInt64:
            //        data = (long)(UInt64)inEnum;
            //        break;
            //}

            //List<string> enumToArray = new List<string>();
            //if (data == 0)
            //{
            //    enumToArray.Add(inEnum.ToString());
            //}
            //else
            //{
            //    Array array = Enum.GetValues(ty);
            //    for (int i = 0, length = array.Length; i < length; i++)
            //    {
            //        object value = array.GetValue(i);
            //        long id = 0;
            //        switch (typeCode)
            //        {
            //            case TypeCode.SByte:
            //                id = (long)(SByte)value;
            //                break;
            //            case TypeCode.Byte:
            //                id = (long)(Byte)value;
            //                break;
            //            case TypeCode.Int16:
            //                id = (long)(Int16)value;
            //                break;
            //            case TypeCode.UInt16:
            //                id = (long)(UInt16)value;
            //                break;
            //            case TypeCode.Int32:
            //                id = (long)(Int32)value;
            //                break;
            //            case TypeCode.UInt32:
            //                id = (long)(UInt32)value;
            //                break;
            //            case TypeCode.Int64:
            //                id = (long)(Int64)value;
            //                break;
            //            case TypeCode.UInt64:
            //                id = (long)(UInt64)value;
            //                break;
            //        }
            //        if ((data & id) == id)
            //        {
            //            data &= ~id;
            //            enumToArray.Add(value.ToString());
            //            if (data == 0)
            //            {
            //                break;
            //            }
            //        }
            //    }

            //}


            //Console.WriteLine(vv);

            //Console.WriteLine(vv);

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
                gDs = new E[] {
                    new E(){
                        k = -6466,
                        str = "FF",
                        b = true
                    },
                    new E(){
                        k = -3.14E-12,
                        str = "RRRR",
                        b = true
                    },
                    new E(){
                        k = 55.999,
                        str = "3特瑞aV",
                        b = true
                    }
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
                    (System.Double)(-3.14E-12),
                    new List<int>{1, 2, 4 }
                },
                //testEnums = new TestEnum[] { TestEnum.Test002, TestEnum.Test004, TestEnum.Test003 },
                //testEnum = TestEnum.Test008 | TestEnum.Test002 | TestEnum.Test003,


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
                listE = new List<E>() {
                   new E(){
                    k = -3E-12,
                    str = "ff出错fvv",
                    b = true
                   },
                   new E(){
                    k = 124,
                    str = "3特瑞aV",
                    b = false
                   },
                   new E(){
                    k = -999.45,
                    str = "_@W#W$%^&",
                    b = true
                   }
                },

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
                dcc3 = new TclassDCC3("3213.#$%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),
                dcc2 = new TclassDCC("3213.#$%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),

                Iclass0Z = new DogJsonTest.TclassC
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
                v3 = new Vector3(3, 2, 1),
                //testDelegate2 = TClassA.Fool,

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
                tClass001 = new TClass001
                {
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

                dictionary3 = new Dictionary<Vector3, B>()
                {
                    {
                       new Vector3( 3, 2, 1 ),
                       new B {
                            num = 11111,
                            str= "rradads"
                        }
                    },

                    {
                       new Vector3( 1, 4, -9.8f  ),
                       new B {
                            num = 888,
                            str= "热热我"
                        }
                    },

                    {
                       new Vector3( -3.2E-13f, 3.0E+13f, 0.99f ),
                       new B {
                            num = 999999,
                            str= "特别强势人物了"
                        }
                    }
                },

                tstructA = new TstructA
                {
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

            inputData.testClassDD = inputData.tClass001.tClass002.tClass003;
            inputData.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;
            inputData.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
            inputData.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];

            JsonWriter jsonWriter = new JsonWriter(new WriterReflection());

            string dataStr = jsonWriter.Writer(inputData);
            Console.WriteLine(dataStr);


            //            dataStr = @"
            //{
            //""dcc"": {
            //                ""$type"": ""DogJsonTest,DogJsonTest.TclassDCC"",
            //                ""$value"": {
            //                        ""$create"": [
            //                                {
            //                                        ""$type"": ""mscorlib,System.String"",
            //                                        ""$value"": ""3213.#$%^&*()""
            //                                },
            //                                {
            //                                        ""$type"": ""mscorlib,System.Collections.Generic.List`1[System.Int32]"",
            //                                        ""$value"": [
            //                                                14, 24, 34, 44, 54
            //                                        ]
            //                                },
            //                                {
            //                                        ""$type"": ""mscorlib,System.Double"",
            //                                        ""$value"": -3.14E-12
            //                                },
            //                        ]
            //                }
            //        }
            //}
            //";
            //dataStr = File.ReadAllText("TextFile3.json");
            JsonRender jsonRender = new JsonRender();


            var outData = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(dataStr);

            bool isOk =  Assert.AreEqualObject(outData, inputData);
            Console.WriteLine(isOk);
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
            JsonRender jsonRender = new JsonRender();

            string testPath = Path.GetDirectoryName(typeof(JsonRender).Assembly.Location) + @"\JsonFile\" + nameof(JsonReadTestClassA.ReadClassTestJsonClassA) + ".json";
            string str = File.ReadAllText(testPath, Encoding.Unicode);

            TestJsonClassA o = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);

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
            o = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);

            //string strd = File.ReadAllText("TextFile1.json");

            Stopwatch oTime2 = new Stopwatch();
            oTime2.Reset(); oTime2.Start();
            for (int __1 = 0; __1 < testConst; __1++)
            {
                var ot = jsonRender.ReadJsonTextCreateObject<TestJsonClassA>(str);
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
