using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DogJson;
using DogJson.RenderToObject;
using NUnit.Framework;
namespace DogJsonTest
{
    [TestFixture]
    public class BaseTest
    {
        public class AAA<T, B>
        {
            public class BBB { }
        }

        public struct TestStruct
        {
            public double num2;
            public int num;
        }

        public class TestClass
        {
            public double num2;
            public int num;
        }


        public class CCC
        {
            public class DDD<T, B> { }
        }

        [Test]
        public unsafe void GetTypeTest()
        {
            string typeName = typeof(DogJsonTest.E[]).ToString();
            Type dc = UnsafeOperation.GetType(typeName);
            Assert.Equal(typeName, dc.ToString());

            typeName = typeof(AAA<int, string>.BBB).ToString();
            dc = UnsafeOperation.GetType(typeName);
            Assert.Equal(typeName, dc.ToString());

            typeName = typeof(Dictionary<Tuple<string, int>, List<string[][,,,]>>[,][]).ToString();
            dc = UnsafeOperation.GetType(typeName);
            Assert.Equal(typeName, dc.ToString());

            typeName = typeof(Dictionary<AAA<int, List<string>>, Tuple<int, Dictionary<int, List<string>>, AAA<string, Tuple<int, double>>.BBB>>).ToString();
            dc = UnsafeOperation.GetType(typeName);
            Assert.Equal(typeName, dc.ToString());

            typeName = typeof(Dictionary<AAA<int[][,,,], List<CCC.DDD<List<string>, List<string[][,,,]>[,][]>>[][,]>[,][], Tuple<int, Dictionary<int, List<string>[]>, AAA<string, Tuple<int, double>[]>.BBB>[]>).ToString();
            dc = UnsafeOperation.GetType(typeName);
            Assert.Equal(typeName, dc.ToString());

        }


        [Test]
        public unsafe void StringTableTest()
        {
            string[] strss = new string[] {
                "testDelegate",
                "testDelegate2",
                "testDelegate3",
                "arrayLinkedList",
                "arrayArray1",
                "arrayArray2",
                "arrayArray3",
                "arrayArray4",
                "arrayArray5",
                "listB",
                "listE",
                "b",
                "kk",
                "str",
                "gcc",
                "gD",
                "gDs",
                "v3",
                "testOB",
                "dcc",
                "dcc3",
                "dcc2",
                "ddd",
                "objects",
                "tClass001",
                "testClassDD",
                "testClassDD2",
                "testClassDD3",
                "testClassDD4",
                "testEnum",
                "testEnums",
                "arrayint2",
                "Iclass0Z",
                "p3",
                "arrayRekn",
                "arrayStack",
                "fd",
                "arraydouble",
                "arraystring",
                "arraybool",
                "dictionary3",
                "tstructA",
                "Num",
                "BB",
                "testEnum2"
            };

            Dictionary<string, int> strs = new Dictionary<string, int>();
            for (int i = 0; i < strss.Length; i++)
            {
                strs.Add(strss[i], i);
            }

            var varStringTable = new StringTable(strs);

            for (int i = 0; i < strss.Length; i++)
            {
                string str = strss[i] + "\"";
                fixed (char* c = str)
                {
                    int id = varStringTable.Find(c, str.Length - 1);
                    Assert.Equal(id, i);
                }

            }

            strss = new string[]
            {
                "a23",// 1 2
                "b23",// 2 3
                "c23",// 3 4
                //21 18
                "d24",// 4 2
                "e24",// 5 3
                "f24",// 6 4
            };

            strs = new Dictionary<string, int>();
            for (int i = 0; i < strss.Length; i++)
            {
                strs.Add(strss[i], i);
            }
            varStringTable = new StringTable(strs);
            for (int i = 0; i < strss.Length; i++)
            {
                string str = strss[i] + "\"";
                fixed (char* c = str)
                {
                    int id = varStringTable.Find(c, str.Length - 1);
                    Assert.Equal(id, i);
                }
            }
        }


        [Test]
        public unsafe void ArrayWrapTest()
        {
            int* array_lengths = stackalloc int[10];
            int size = 1;

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(TestStruct[,,]));
                array_lengths[0] = 2;
                array_lengths[1] = 3;
                array_lengths[2] = 4;
                size = 2 * 3 * 4;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                int itemTypeSize;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                TestStruct[,,] outData = new TestStruct[2, 3, 4];
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            outData[x, y, z].num = x * y - z;
                            outData[x, y, z].num2 = x * y + z;
                            fixed (TestStruct* p = &outData[x, y, z])
                            {
                                GeneralTool.Memcpy(
                                     startItemOffcet + (x * 3 * 4 + y * 4 + z) * arrayWrap.elementTypeSize, p
                                     , arrayWrap.elementTypeSize);
                            }
                        }
                    }
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(TestClass[,,]));
                array_lengths[0] = 2;
                array_lengths[1] = 3;
                array_lengths[2] = 4;
                size = 2 * 3 * 4;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                TestClass[,,] outData = new TestClass[2, 3, 4];
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            TestClass testClass = outData[x, y, z] = new TestClass();
                            outData[x, y, z].num = x * y - z;
                            outData[x, y, z].num2 = x * y + z;

                            GeneralTool.SetObject(startItemOffcet + (x * 3 * 4 + y * 4 + z) * arrayWrap.elementTypeSize,
                               testClass);
                        }
                    }
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(int[,,]));
                array_lengths[0] = 2;
                array_lengths[1] = 3;
                array_lengths[2] = 4;
                size = 2 * 3 * 4;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                int[,,] outData = new int[2, 3, 4];
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            outData[x, y, z] = x * y - z;

                            fixed (int* p = &outData[x, y, z])
                            {
                                GeneralTool.Memcpy(
                                     startItemOffcet + (x * 3 * 4 + y * 4 + z) * arrayWrap.elementTypeSize, p
                                     , arrayWrap.elementTypeSize);
                            }
                        }
                    }
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(TestStruct[]));
                size = 12;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                int itemTypeSize;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                TestStruct[] outData = new TestStruct[size];
                for (int i = 0; i < size; i++)
                {
                    outData[i].num = i;
                    outData[i].num2 = i * i;
                    fixed (TestStruct* p = &outData[i])
                    {
                        GeneralTool.Memcpy(
                             startItemOffcet + i * arrayWrap.elementTypeSize, p
                             , arrayWrap.elementTypeSize);
                    }
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(TestClass[]));
                size = 12;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                TestClass[] outData = new TestClass[size];
                for (int i = 0; i < size; i++)
                {
                    TestClass testClass = outData[i] = new TestClass();
                    outData[i].num = i;
                    outData[i].num2 = i * 2;

                    GeneralTool.SetObject(startItemOffcet + (i) * arrayWrap.elementTypeSize,
                       testClass);
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }

            {
                IArrayWrap arrayWrap = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
                size = 12;

                byte* objPtr;
                byte* startItemOffcet;
                GCHandle gCHandle;
                object inData = arrayWrap.CreateArray(size, array_lengths, out objPtr, out startItemOffcet, out gCHandle);

                Assert.Equal(inData, GeneralTool.VoidToObject(objPtr));

                int[] outData = new int[size];
                for (int i = 0; i < size; i++)
                {
                    outData[i] = i;

                    fixed (int* p = &outData[i])
                    {
                        GeneralTool.Memcpy(
                             startItemOffcet + (i) * arrayWrap.elementTypeSize, p
                             , arrayWrap.elementTypeSize);
                    }
                }
                gCHandle.Free();

                Assert.AreEqualObject(inData, outData);
            }
        }












    }
}
