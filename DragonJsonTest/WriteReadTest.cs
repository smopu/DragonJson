using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DragonJson;
using DragonJson.RenderToObject;
using NUnit.Framework;
namespace DragonJsonTest
{
    [TestFixture]
    public class WriteReadTest
    {
        JsonWriter _jsonWriter;
        JsonWriter jsonWriter {
            get {
                if (_jsonWriter == null)
                {
                    _jsonWriter = new JsonWriter(new WriterReflection());
                }
                return _jsonWriter; }
        
        }

        JsonReader _jsonReader;
        JsonReader jsonReader
        {
            get
            {
                if (_jsonReader == null)
                {
                    _jsonReader = new JsonReader();
                }
                return _jsonReader;
            }
        }

        class TestNullClass1
        {
            public TestClass testClass;
        }
        struct TestNullStruct1
        {
            public TestClass testClass;
        }


        public struct TStructNull
        {
            public TStructNull1 var1;
        }
        public struct TStructNull1
        {
            public TStructNull2 var2;
        }
        public struct TStructNull2
        {
            public TStructNull3 var3;
        }
        public struct TStructNull3
        {
            public TStructNull4 var4;
        }

        public struct TStructNull4
        {
            public TClass001 tClass001;
            public TClass003 testClassDD;
            public TClass003 testClassDD2;
            public TClass003 testClassDD3;
            public TClass003 testClassDD4;
        }


        [Test]
        public unsafe void T001_Null()
        {
            {
                TestNullClass1 inputData = new TestNullClass1();
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }
            {
                TestNullStruct1 inputData = new TestNullStruct1();
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }
            {
                TStructNull inputData = new TStructNull();
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            
        }

        [Test]
        public unsafe void T002_ListTest()
        {
            {
                List<int> inputData = new List<int>() { 1, 2, 3, 4, 5, 6 };
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<short> inputData = new List<short>() { 1, -2, 1233, 422, -5, 6444 };
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreate<List<short>>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<long> inputData = new List<long>() { -1, 2, -334146798, 4, -5, 6 };
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreate<List<long>>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<double> inputData = new List<double>() { 1.2, 2.455, 3.0E-13, 4.888e+15, -9.456E-13, -3.888E+15, -3.0e+14, 3.0e-14, -3.0e-14 };
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreate<List<double>>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<float> inputData = new List<float>() { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f };
                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreate<List<float>>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<string> inputData = new List<string>() { "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" };
                List<string> inputData2 = new List<string>() { "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" };
                string dataStr = jsonWriter.Writer(inputData2);

                var outData = jsonReader.ReadJsonTextCreate<List<string>>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<TestStruct> inputData = new List<TestStruct>() { new TestStruct() { num = 1, str = "ccc￥%……&&G"}, new TestStruct() { num = -155, str = "ccG" } };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<List<TestStruct>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                List<TestClass> inputData = new List<TestClass>() { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<List<TestClass>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            {
                List<HashSet<TestClass>> inputData = new List<HashSet<TestClass>>();
                inputData.Add(new HashSet<TestClass>() { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -1355, str = "ccG" } });
                inputData.Add(new HashSet<TestClass>() { new TestClass() { num = 2, str = "cccrG" }, new TestClass() { num = -15544, str = "ccG" } });
                inputData.Add(new HashSet<TestClass>() { new TestClass() { num = 3, str = "cccfG" }, new TestClass() { num = -135445, str = "ccG" } });

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<List<HashSet<TestClass>>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

        }


        [Test]
        public unsafe void T003_JsonClassATest()
        {
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
                testEnums = new TestEnum[] { TestEnum.Test002, TestEnum.Test004, TestEnum.Test003 },
                testEnum = TestEnum.Test008 | TestEnum.Test002 | TestEnum.Test003,
                testEnum2 = TestEnum.Test001 | TestEnum.Test002 | TestEnum.Test003,

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
                dcc = new TclassDCC("3213.#%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),
                dcc3 = new TclassDCC3("3213.#%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),
                dcc2 = new TclassDCC("3213.#%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12),

                Iclass0Z = new DragonJsonTest.TclassC
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
                tClass001 = new TClass001
                {
                    objects = new object[] {
                        (System.Int32)12,
                        -(System.Double)3.14E-12,
                        (System.String)"3213.#%^&*()",
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
            inputData.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
            inputData.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];
            inputData.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;

            //"testClassDD": "$/tClass001/tClass002/tClass003",
            //"testClassDD2": "$/tClass001/tClass002/tClass003s/2",
            //"testClassDD3": "$/tClass001/tClass002s/1/tClass003s/2",
            //"testClassDD4": "$/tClass001/tClass002s/1/tClass003"

            inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD = inputData.tClass001.tClass002.tClass003;
            inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;
            inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
            inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];

            inputData.testDelegate2 += TClassA.Foo2;

            inputData.testDelegate3 = inputData.Iclass0Z.Fool;
            inputData.testDelegate3 += inputData.Iclass0Z.Foo2;
            inputData.testDelegate = inputData.testDelegate2;

            string dataStr = jsonWriter.Writer(inputData);

            var outData = jsonReader.ReadJsonTextCreate<TestJsonClassA>(dataStr);

            Assert.AreEqualObject(outData, inputData);
        }

        [Test]
        public void T004_ClassTest()
        {
            {
                TestClass2 inputData = new TestClass2()
                {
                    varDouble = 1.2,
                    varFloat = 1.23f,
                    varInt = 566,
                    varLong = 18,
                    varBool = true,
                    varChar = 'c',
                    varString = "BNUY*Fhi89#$^&",

                    VarDouble2 = 4.9E+18f,
                    VarFloat2 = 1.2E-18f,
                    VarInt2 = -5435,
                    VarLong2 = -18,
                    VarBool2 = true,
                    VarChar2 = 'c',
                    VarString2 = "BNUY*Fhi89#$^&",

                    testStruct = new TestStruct3()
                    {
                        varDouble = 1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    testClass = new TestClass3()
                    {
                        varDouble = -1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    VarStruct = new TestStruct3()
                    {
                        varDouble = 1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    VarClass = new TestClass3()
                    {
                        varDouble = -1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestClass2>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        [Test]
        public unsafe void T005_StructTest()
        {
            {
                TestStruct2 inputData = new TestStruct2() {
                    varDouble = 1.2,
                    varFloat = 1.23f,
                    varInt = 566,
                    varLong = 18,
                    varBool = true,
                    varChar = 'c',
                    varString = "BNUY*Fhi89#$^&",

                    VarDouble2 = 4.9E+18f,
                    VarFloat2 = 1.2E-18f,
                    VarInt2 = -5435,
                    VarLong2 = -18,
                    VarBool2 = true,
                    VarChar2 = 'c',
                    VarString2 = "BNUY*Fhi89#$^&",

                    testStruct = new TestStruct3()
                    {
                        varDouble = 1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    testClass = new TestClass3()
                    {
                        varDouble = -1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    VarStruct = new TestStruct3()
                    {
                        varDouble = 1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    },
                    VarClass = new TestClass3()
                    {
                        varDouble = -1.2,
                        varFloat = 1.23f,
                        varInt = 566,
                        varLong = 18,
                        varBool = true,
                        varChar = 'c',
                        varString = "BNUY*Fhi89#$^&",
                        VarDouble2 = 4.9E+18f,
                        VarFloat2 = 1.2E-18f,
                        VarInt2 = -5435,
                        VarLong2 = -18,
                        VarBool2 = true,
                        VarChar2 = 'c',
                        VarString2 = "BNUY*Fhi89#$^&",
                    }
                };

                string dataStr = jsonWriter.Writer(inputData);

                var outData = jsonReader.ReadJsonTextCreate<TestStruct2>(dataStr);

                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 数组
        /// </summary>
        [Test]
        public void T006_ArrayTest()
        {
            {
                int[] inputData = new int[] { 1, 2, 3, 4, 5, 6 };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<int[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                short[] inputData = new short[] { 1, -2, 1233, 422, -5, 6444 };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<short[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                long[] inputData = new long[] { -1, 2, -334146798, 4, -5, 6 };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<long[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                double[] inputData = new double[] { 1.2, 2.455, 3.0E-13, 4.888e+15, -9.456E-13, -3.888E+15, -3.0e+14, 3.0e-14, -3.0e-14 };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<double[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                float[] inputData = new float[] { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<float[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                string[] inputData = new string[] { "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" };
                string[] inputData2 = new string[] { "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" };
                string dataStr = jsonWriter.Writer(inputData2);
                var outData = jsonReader.ReadJsonTextCreate<string[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            {
                TestStruct[] inputData = new TestStruct[] { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestStruct[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                TestClass[] inputData = new TestClass[] { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestClass[]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

        }

        /// <summary>
        /// 多维数组
        /// </summary>
        [Test]
        public void T007_MultidimensionalArrayTest()
        {
            {
                string[,,] inputData = new string[,,] {
                    {
                      { "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" }
                    },
                    {
                      { "FFFF", "#ED", "*&*($DD", "大家好！\u6211\u662f\u4f60\u7238\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" }
                    },
                    {
                      { "SS", "fff", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\u6211\u662f\u4f60\u7238\u7238！" }
                    }
                };
                string[,,] inputData2 = new string[,,] {
                    {
                      { "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" }
                    },
                    {
                      { "FFFF", "#ED", "*&*($DD", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" }
                    },
                    {
                      { "SS", "fff", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" },{ "asdas", "d23re", "*&*(HJ", "大家好！\\u6211\\u662f\\u4f60\\u7238\\u7238！" }
                    }
                };
                string dataStr = jsonWriter.Writer(inputData2);
                var outData = jsonReader.ReadJsonTextCreate<string[,,]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                int[,,] inputData = new int[,,] {
                    {
                        { 1, 2, 3, 4, 5, 6 }, { 11, 12, 13, 14, 15, 16 }, { 101, 102, 103, 104, 105, 106 }
                    },
                    {
                        { 91, 92, 93, 94, 95, 96 }, { 911, 912, 913, 914, 915, 916 }, { 9101, 9102, 9103, 9104, 9105, 9106 }
                    },
                    {
                        { 61, 62, 63, 64, 65, 66 }, { 611, 612, 613, 14, 15, 16 }, { 6101, 6102, 6103, 6104, 6105, 6106 }
                    }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<int[,,]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                double[,,] inputData = new double[,,] {
                    {
                        { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f}, { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f},
                    },
                    {
                        { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f}, { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f},
                    },
                    {
                        { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f}, { 1.2f, 2.455f, 3.0E-13f, 4.888e+15f, -9.456E-13f, -3.888E+15f, -3.0e+14f, 3.0e-14f, -3.0e-14f},
                }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<double[,,]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                TestStruct[,,] inputData = new TestStruct[,,] {
                    {
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } },
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } }
                    }
                    ,
                    {
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } },
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } }
                    },
                    {
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } },
                      { new TestStruct() { num = 1, str = "ccc￥%……&&G" }, new TestStruct() { num = -155, str = "ccG" } }
                    }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestStruct[,,]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                TestClass[,,] inputData = new TestClass[,,] {
                    {
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } },
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } }
                    }
                    ,
                    {
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } },
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } }
                    },
                    {
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } },
                      { new TestClass() { num = 1, str = "ccc￥%……&&G" }, new TestClass() { num = -155, str = "ccG" } }
                    }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestClass[,,]>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 字典
        /// </summary>
        [Test]
        public void T008_DictionaryTest()
        {
            {
                Dictionary<int, float> inputData = new Dictionary<int, float>
                {
                     { 1, 2.4f },
                     { 2, -6.774f },
                     { 3, 0.4f },
                };

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<int, float>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<TestStruct, float> inputData = new Dictionary<TestStruct, float>
                {
                     { new TestStruct() { num = 1, str = "cccrrrG"}, 2.4f },
                     { new TestStruct() { num = 3, str = "cccsdfsfG"}, -6.774f },
                     { new TestStruct() { num = 4, str = "cccgggG"}, 0.4f },
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<TestStruct, float>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            {
                Dictionary<float, TestStruct> inputData = new Dictionary<float, TestStruct>
                {
                     { 2.4f,  new TestStruct() { num = 1, str = "cccrrrG"} },
                     {-6.774f, new TestStruct() { num = 3, str = "cccsdfsfG"} },
                     {0.4f, new TestStruct() { num = 4, str = "cccgggG"} }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<float, TestStruct>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<TestStruct, TestStruct4> inputData = new Dictionary<TestStruct, TestStruct4>
                {
                     { new TestStruct() { num = 1, str = "cccrrrG"}, new TestStruct4() { num = 1, str = "cccrrrG"} },
                     { new TestStruct() { num = 3, str = "cccsdfsfG"},new TestStruct4() { num = 34, str = "dd"} },
                     { new TestStruct() { num = 4, str = "cccgggG"}, new TestStruct4() { num = 5, str = "rr444"} },
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<TestStruct, TestStruct4>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<string, TestStruct> inputData = new Dictionary<string, TestStruct>
                {
                     { "2.4f",  new TestStruct() { num = 1, str = "cccrrrG"} },
                     {"-6.774f", new TestStruct() { num = 3, str = "cccsdfsfG"} },
                     {"0.4f", new TestStruct() { num = 4, str = "cccgggG"} }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<string, TestStruct>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<TestClass, float> inputData = new Dictionary<TestClass, float>
                {
                     { new TestClass() { num = 1, str = "cccrrrG"}, 2.4f },
                     { new TestClass() { num = 3, str = "cccsdfsfG"}, -6.774f },
                     { new TestClass() { num = 4, str = "cccgggG"}, 0.4f },
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<TestClass, float>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<float, TestClass> inputData = new Dictionary<float, TestClass>
                {
                     { 2.4f,  new TestClass() { num = 1, str = "cccrrrG"} },
                     {-6.774f, new TestClass() { num = 3, str = "cccsdfsfG"} },
                     {0.4f, new TestClass() { num = 4, str = "cccgggG"} }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<float, TestClass>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            {
                Dictionary<TestClass, TestClass> inputData = new Dictionary<TestClass, TestClass>
                {
                     { new TestClass() { num = 1, str = "cccrrrG"}, new TestClass() { num = 1, str = "cccrrrG"} },
                     { new TestClass() { num = 3, str = "cccsdfsfG"},new TestClass() { num = 34, str = "dd"} },
                     { new TestClass() { num = 4, str = "cccgggG"}, new TestClass() { num = 5, str = "rr444"} },
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<TestClass, TestClass>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<TestClass, TestClass4> inputData = new Dictionary<TestClass, TestClass4>
                {
                     { new TestClass() { num = 1, str = "cccrrrG"}, new TestClass4() { num = 1, str = "cccrrrG"} },
                     { new TestClass() { num = 3, str = "cccsdfsfG"},new TestClass4() { num = 34, str = "dd"} },
                     { new TestClass() { num = 4, str = "cccgggG"}, new TestClass4() { num = 5, str = "rr444"} },
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<TestClass, TestClass4>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<string, TestClass> inputData = new Dictionary<string, TestClass>
                {
                     { "2.4f",  new TestClass() { num = 1, str = "cccrrrG"} },
                     {"-6.774f", new TestClass() { num = 3, str = "cccsdfsfG"} },
                     {"0.4f", new TestClass() { num = 4, str = "cccgggG"} }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<Dictionary<string, TestClass>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 协变参数
        /// </summary>
        [Test]
        public void T009_CovariantParameterTest()
        {
            {
                TestClassA inputData = new TestClassC()
                {
                    b = false,
                    num2 = 3.8899,
                    str = "@DSAZS",
                    num = 2
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestClassA>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                Dictionary<string, TestClass> inputData = new Dictionary<string, TestClass>
                {
                     { "2.4f",  new TestClass() { num = 1, str = "cccrrrG"} },
                     {"-6.774f", new TestClass() { num = 3, str = "cccsdfsfG"} },
                     {"0.4f", new TestClass() { num = 4, str = "cccgggG"} }
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<IDictionary<string, TestClass>>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }


        /// <summary>
        /// 构造参数 Class
        /// </summary>
        [Test]
        public void T010_ConstructionClassTest()
        {
            {
                TclassDCC inputData = new TclassDCC("3213.#%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12);
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TclassDCC>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 构造参数 Struct
        /// </summary>
        [Test]
        public void T011_ConstructionStructTest()
        {
            {
                TclassDCC3 inputData = new TclassDCC3("3213.#%^&*()", new List<int> { 14, 24, 34, 44, 54 }, -3.14E-12);
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TclassDCC3>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 循环引用
        /// </summary>
        [Test]
        public void T012_LoopReferenceTest()
        {
            {
                LoopReferenceClass2 inputData = new LoopReferenceClass2();
                inputData.class3 = new LoopReferenceClass3();
                inputData.class3.class2 = inputData;
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            //回环
            {
                LoopReferenceClass4 inputData = new LoopReferenceClass4(1);
                var now = inputData;
                now.next = new LoopReferenceClass4(2); now = now.next;
                now.next = new LoopReferenceClass4(3); now = now.next;
                now.next = new LoopReferenceClass4(4); now = now.next;
                now.next = new LoopReferenceClass4(5); now = now.next;
                now.next = new LoopReferenceClass4(6); now = now.next;
                now.next = inputData;

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            //回环有尾
            {
                LoopReferenceClass4 inputData = new LoopReferenceClass4(1);
                var now = inputData;
                now.next = new LoopReferenceClass4(2); now = now.next;
                now.next = new LoopReferenceClass4(3); now = now.next;
                now.next = new LoopReferenceClass4(4); now = now.next;
                var k = now;
                now.next = new LoopReferenceClass4(5); now = now.next;
                now.next = new LoopReferenceClass4(6); now = now.next;
                now.next = k;

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                LoopReferenceClass1 inputData = new LoopReferenceClass1() {
                    tClass001 = new TClass001
                    {
                        objects = new object[] {
                        (System.Int32)12,
                        -(System.Double)3.14E-12,
                        (System.String)"3213.#%^&*()",
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
                };
                //"testClassDD": "$/tClass001/tClass002/tClass003",
                //"testClassDD2": "$/tClass001/tClass002/tClass003s/2",
                //"testClassDD3": "$/tClass001/tClass002s/1/tClass003s/2",
                //"testClassDD4": "$/tClass001/tClass002s/1/tClass003"

                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD = inputData.tClass001.tClass002.tClass003;
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }

            {
                LoopReferenceClass1 inputData = new LoopReferenceClass1()
                {
                    tClass001 = new TClass001
                    {
                        objects = new object[] {
                        (System.Int32)12,
                        -(System.Double)3.14E-12,
                        (System.String)"3213.#%^&*()",
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
                };
                inputData.testClassDD = inputData.tClass001.tClass002.tClass003;
                inputData.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
                inputData.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];
                inputData.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
            {
                LoopReferenceClass1 inputData = new LoopReferenceClass1()
                {
                    tClass001 = new TClass001
                    {
                        objects = new object[] {
                        (System.Int32)12,
                        -(System.Double)3.14E-12,
                        (System.String)"3213.#%^&*()",
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
                };
                inputData.testClassDD = inputData.tClass001.tClass002.tClass003;
                inputData.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
                inputData.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];
                inputData.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;

                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD = inputData.tClass001.tClass002.tClass003;
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD4 = inputData.tClass001.tClass002s[1].tClass003;
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD2 = inputData.tClass001.tClass002.tClass003s[2];
                inputData.classPath.classPath1.classPath2.classPath3.classPath4.testClassDD3 = inputData.tClass001.tClass002s[1].tClass003s[2];

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }


        }



        /// <summary>
        /// 枚举
        /// </summary>
        [Test]
        public void T013_EnumTest()
        {
            {
                EnumClass1 inputData = new EnumClass1
                {
                    testEnums = new TestEnum1[] { TestEnum1.Test002, TestEnum1.Test004, TestEnum1.Test003 },
                    testEnum = TestEnum1.Test008 | TestEnum1.Test002 | TestEnum1.Test003,
                    testEnum2 = TestEnum1.Test001 | TestEnum1.Test002 | TestEnum1.Test003,
                    testEnum3 = TestEnum1.Test005,
                    testEnum4 = TestEnum1.Test007,
                };
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<EnumClass1>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 委托
        /// </summary>
        [Test]
        public void T014_DelegateTest()
        {
            {
                TestDelegateClass1 inputData = new TestDelegateClass1();
                inputData.testDelegate = inputData.Foo1;

                inputData.testDelegate2 = inputData.Foo1;
                inputData.testDelegate2 += inputData.Foo2;

                inputData.testDelegate3 = inputData.Foo1;
                inputData.testDelegate3 += inputData.Foo2;
                inputData.testDelegate3 += TestDelegateClass1.Foo1static;
                inputData.testDelegate3 += TestDelegateClass1.Foo2static;

                inputData.testDelegate4 = inputData.Foo1;
                inputData.testDelegate4 += inputData.Foo2;
                inputData.testDelegate4 += inputData.testDelegateClass2.Foo3;
                inputData.testDelegate4 += inputData.testDelegateClass2.Foo4;

                inputData.testDelegate5 = inputData.Foo1;
                inputData.testDelegate5 += inputData.Foo2;
                inputData.testDelegate5 += inputData.testDelegateClass2.Foo3;
                inputData.testDelegate5 += inputData.testDelegateClass2.Foo4;
                inputData.testDelegate5 += TestDelegateClass1.Foo1static;
                inputData.testDelegate5 += TestDelegateClass2.Foo3static;

                inputData.testDelegate6 = inputData.Foo1;
                inputData.testDelegate6 += inputData.Foo2;
                inputData.testDelegate6 += inputData.testDelegateClass2.Foo3;
                inputData.testDelegate6 += inputData.testDelegateClass2.Foo4;
                inputData.testDelegate6 += TestDelegateClass1.Foo1static;
                inputData.testDelegate6 += TestDelegateClass1.Foo2static;
                inputData.testDelegate6 += TestDelegateClass2.Foo3static;
                inputData.testDelegate6 += TestDelegateClass2.Foo4static;

                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreate<TestDelegateClass1>(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        [Test]
        public void T015_TypeTest()
        {
            {
                TypeClass inputData = new TypeClass();
                string dataStr = jsonWriter.Writer(inputData);
                var outData = jsonReader.ReadJsonTextCreateObject(dataStr);
                Assert.AreEqualObject(outData, inputData);
            }
        }

            
        class LoopReferenceClass1
        {
            public TClassPath classPath;
            public TClass001 tClass001;
            public TClass003 testClassDD;
            public TClass003 testClassDD2;
            public TClass003 testClassDD3;
            public TClass003 testClassDD4;
        }
        class LoopReferenceClass2
        {
            public int num = 444; 
            public LoopReferenceClass3 class3;
        }

        class LoopReferenceClass3
        {
            public int ff = 3; 
            public LoopReferenceClass2 class2;
        }

        class LoopReferenceClass4
        {
            public LoopReferenceClass4(int node)
            {
                this.node = node;
            }
            public int node = 1; 
            public LoopReferenceClass4 next;
        }



        enum TestEnum1
        {
            Test001 = 1,
            Test002 = 2,
            Test003 = 4,
            Test004 = 8,
            Test005 = 16,
            Test006 = 32,
            Test007 = 64,
            Test008 = 128,
        }
        class EnumClass1
        {
            public TestEnum1 testEnum;
            public TestEnum1 testEnum2 { get; set; }
            public TestEnum1[] testEnums;
            public TestEnum1 testEnum3;
            public TestEnum1 testEnum4;
        }


        public class TestDelegateClass2
        {
            public void Foo3(int a, string b)
            {
            }
            public void Foo4(int a, string b)
            {
            }
            public static void Foo3static(int a, string b)
            {
            }
            public static void Foo4static(int a, string b)
            {
            }
        }

        public class TestDelegateClass1
        {
            public TestDelegateClass2 testDelegateClass2 = new TestDelegateClass2();
            public Action<int, string> testDelegate;
            public Action<int, string> testDelegate2;
            public Action<int, string> testDelegate3;
            public Action<int, string> testDelegate4;
            public Action<int, string> testDelegate5;
            public Action<int, string> testDelegate6;
            public void Foo1(int a, string b)
            {
            }
            public void Foo2(int a, string b)
            {

            }
            public static void Foo1static(int a, string b)
            {
            }
            public static void Foo2static(int a, string b)
            {

            }
        }

        class TestClass2
        {
            public bool varBool;
            public char varChar;
            public double varDouble;
            public float varFloat;
            public int varInt;
            public long varLong;
            public string varString;
            public TestStruct3 testStruct;
            public TestClass3 testClass;

            private float varFloat2;
            private double varDouble2;
            private int varIn2t;
            private long varLong2;

            private string varString2;
            private bool varBool2;
            private char varChar2;
            private TestStruct3 testStruct2;
            private TestClass3 testClass2;


            public float VarFloat2 { get => varFloat2; set => varFloat2 = value; }
            public double VarDouble2 { get => varDouble2; set => varDouble2 = value; }
            public int VarInt2 { get => varIn2t; set => varIn2t = value; }
            public long VarLong2 { get => varLong2; set => varLong2 = value; }
            public string VarString2 { get => varString2; set => varString2 = value; }
            public bool VarBool2 { get => varBool2; set => varBool2 = value; }
            public char VarChar2 { get => varChar2; set => varChar2 = value; }
            public TestStruct3 VarStruct { get => testStruct2; set => testStruct2 = value; }
            public TestClass3 VarClass { get => testClass2; set => testClass2 = value; }

        }
        struct TestStruct2
        {
            public bool varBool;
            public char varChar;
            public double varDouble;
            public float varFloat;
            public int varInt;
            public long varLong;
            public string varString;
            public TestStruct3 testStruct;
            public TestClass3 testClass;

            private float varFloat2;
            private double varDouble2;
            private int varIn2t;
            private long varLong2;

            private string varString2;
            private bool varBool2;
            private char varChar2;
            private TestStruct3 testStruct2;
            private TestClass3 testClass2;


            public float VarFloat2 { get => varFloat2; set => varFloat2 = value; }
            public double VarDouble2 { get => varDouble2; set => varDouble2 = value; }
            public int VarInt2 { get => varIn2t; set => varIn2t = value; }
            public long VarLong2 { get => varLong2; set => varLong2 = value; }
            public string VarString2 { get => varString2; set => varString2 = value; }
            public bool VarBool2 { get => varBool2; set => varBool2 = value; }
            public char VarChar2 { get => varChar2; set => varChar2 = value; }
            public TestStruct3 VarStruct { get => testStruct2; set => testStruct2 = value; }
            public TestClass3 VarClass { get => testClass2; set => testClass2 = value; }

        }

        struct TestStruct3
        {
            public bool varBool;
            public char varChar;
            public double varDouble;
            public float varFloat;
            public int varInt;
            public long varLong;
            public string varString;


            private float varFloat2;
            private double varDouble2;
            private int varIn2t;
            private long varLong2;

            private string varString2;
            private bool varBool2;
            private char varChar2;

            public float VarFloat2 { get => varFloat2; set => varFloat2 = value; }
            public double VarDouble2 { get => varDouble2; set => varDouble2 = value; }
            public int VarInt2 { get => varIn2t; set => varIn2t = value; }
            public long VarLong2 { get => varLong2; set => varLong2 = value; }
            public string VarString2 { get => varString2; set => varString2 = value; }
            public bool VarBool2 { get => varBool2; set => varBool2 = value; }
            public char VarChar2 { get => varChar2; set => varChar2 = value; }
        }

        class TestClass3
        {
            public bool varBool;
            public char varChar;
            public double varDouble;
            public float varFloat;
            public int varInt;
            public long varLong;
            public string varString;


            private float varFloat2;
            private double varDouble2;
            private int varIn2t;
            private long varLong2;

            private string varString2;
            private bool varBool2;
            private char varChar2;

            public float VarFloat2 { get => varFloat2; set => varFloat2 = value; }
            public double VarDouble2 { get => varDouble2; set => varDouble2 = value; }
            public int VarInt2 { get => varIn2t; set => varIn2t = value; }
            public long VarLong2 { get => varLong2; set => varLong2 = value; }
            public string VarString2 { get => varString2; set => varString2 = value; }
            public bool VarBool2 { get => varBool2; set => varBool2 = value; }
            public char VarChar2 { get => varChar2; set => varChar2 = value; }
        }

        struct TestStruct
        {
            public string str;
            public int num;
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is TestStruct)
                {
                    TestStruct testStruct = (TestStruct)obj;
                    if (testStruct.num == this.num && testStruct.str == this.str)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return num;
            }
        }
        
        struct TestStruct4
        {
            public string str;
            public int num;
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is TestStruct4)
                {
                    TestStruct4 testStruct = (TestStruct4)obj;
                    if (testStruct.num == this.num && testStruct.str == this.str)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return num;
            }
        }

        class TestClass4
        {
            public string str;
            public int num;
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is TestClass4)
                {
                    TestClass4 testStruct = (TestClass4)obj;
                    if (testStruct.num == this.num && testStruct.str == this.str)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return num;
            }
        }

        class TestClassA
        {
            public int num;
        }
        class TestClassB : TestClassA
        {
            public string str;
        }
        class TestClassC : TestClassB
        {
            public bool b;
            public double num2;
        }


        class TestClass
        {
            public string str;
            public int num;
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is TestClass)
                {
                    TestClass testStruct = (TestClass)obj;
                    if (testStruct.num == this.num && testStruct.str == this.str)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return num;
            }
        }

        public class AAA<T, B>
        {
            public class BBB { }
        }
        public class CCC
        {
            public class DDD<T, B> { }
        }


        class TypeClass
        {
            public TypeClass()
            {
                var types = new[] { type1, type3, type2, type3, type4, type5, type6, type7, type8, type9, type10, type11 };
                listTypes = new List<Type>() { type1, type2, type3, type4, type5, type6, type7, type8, type9, type10, type11 };
                foreach (var item in types)
                {
                    dictionary1[item.ToString()] = item;
                    dictionary2[item] = item.ToString();
                }
            }

            public Dictionary<Type, string> dictionary2 = new Dictionary<Type, string>();
            public Dictionary<string, Type> dictionary1 = new Dictionary<string, Type>();

            public Type[] types;
            public List<Type> listTypes;


            public Type type1 = typeof(DragonJsonTest.E[]);
            public Type type2 = typeof(AAA<int, string>.BBB);

            public Type type3 = typeof(Dictionary<Tuple<string, int>, List<string[][,,,]>>[,][]);
            public Type type4 = typeof(Dictionary<AAA<int, List<string>>, Tuple<int, Dictionary<int, List<string>>, AAA<string, Tuple<int, double>>.BBB>>);
            public Type type5 = typeof(Dictionary<AAA<int[][,,,], List<CCC.DDD<List<string>, List<string[][,,,]>[,][]>>[][,]>[,][], Tuple<int, Dictionary<int, List<string>[]>, AAA<string, Tuple<int, double>[]>.BBB>[]>);

            public Type type6 = typeof(int);
            public Type type7 = typeof(float);
            public Type type8 = typeof(string);
            public Type type9 = typeof(int[]);
            public Type type10 = typeof(float[,]);
            public Type type11 = typeof(string[,]);

            private Type _type1 = typeof(DragonJsonTest.E[]);
            private Type _type2 = typeof(AAA<int, string>.BBB);
            private Type _type3 = typeof(Dictionary<Tuple<string, int>, List<string[][,,,]>>[,][]);
            private Type _type4 = typeof(Dictionary<AAA<int, List<string>>, Tuple<int, Dictionary<int, List<string>>, AAA<string, Tuple<int, double>>.BBB>>);
            private Type _type5 = typeof(Dictionary<AAA<int[][,,,], List<CCC.DDD<List<string>, List<string[][,,,]>[,][]>>[][,]>[,][], Tuple<int, Dictionary<int, List<string>[]>, AAA<string, Tuple<int, double>[]>.BBB>[]>);
            public Type _type6 = typeof(int);
            public Type _type7 = typeof(float);
            public Type _type8 = typeof(string);
            public Type _type9 = typeof(int[]);
            public Type _type10 = typeof(float[,]);
            public Type _type11 = typeof(string[,]);

            public Type Type1 { get => _type1; set => _type1 = value; }
            public Type Type2 { get => _type2; set => _type2 = value; }
            public Type Type3 { get => _type3; set => _type3 = value; }
            public Type Type4 { get => _type4; set => _type4 = value; }
            public Type Type5 { get => _type5; set => _type5 = value; }
            public Type Type6 { get => _type6; set => _type6 = value; }
            public Type Type7 { get => _type7; set => _type7 = value; }
            public Type Type8 { get => _type8; set => _type8 = value; }
            public Type Type9 { get => _type9; set => _type9 = value; }
            public Type Type10 { get => _type10; set => _type10 = value; }
            public Type Type11 { get => _type11; set => _type11 = value; }

        }



    }
}
