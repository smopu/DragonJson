using NUnit.Framework;
using PtrReflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PtrReflection.Test
{
    public unsafe class ChakeUnitTestStruct
    {
        void DebugLog(object obj)
        {
            Console.WriteLine(Convert.ToString(obj));
            GC.Collect();
        }

        TypeAddrReflectionWrapper warp = TypeAddrReflectionWrapper.GetTypeWarp(typeof(MyClassStruct));

        MyClassStruct instens = new MyClassStruct();
        [SetUp]
        public void Setup()
        {
            //instens = (MyClassStruct)warp.Create();
            instens.one = 1;
            instens.str = "S3ED";
            instens.point = new Vector3(3, -4.5f, 97.4f);

            instens.One = -9;
            instens.Str = "QBN3";
            instens.Point = new Vector3(45f, -12.00009f, 3f);

            instens.ones = new int[] { 1, 2, 8, 476, 898, 9 };
            instens.strs = new string[] { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" };
            instens.points = new Vector3[] {
               new Vector3(3, -4.5f, 97.4f),
               new Vector3(9999f, -43f, 0.019f),
               new Vector3(55.3f, -0.01f, -130),
            };

            instens.oness = new int[,,]
            {
                { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
            };
            instens.strss = new string[,] {
                { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
            };
            instens.pointss = new Vector3[,] {
                {
                   new Vector3(3, -4.5f, 97.4f),
                   new Vector3(9999f, -43f, 0.019f),
                   new Vector3(55.3f, -0.01f, -130)
                },
                {
                   new Vector3(0, -1.2e+19f, 97.4f),
                   new Vector3(9999f, -100000f, 0.019f),
                   new Vector3(55.3f, -0.01f, -130)
                },
                {
                   new Vector3(3, -4.5f, 5555.5555f),
                   new Vector3(12321.4441f, -0.000001f, 0.019f),
                   new Vector3(1234, -982.3f, -299)
                },
            };

        }



        [Test]
        public void Test_ObjReference()
        {
            MyClassStruct instensT = new MyClassStruct();

            void* handleVoid = GeneralTool.AsPointer(ref instensT);
            byte* handleByte = (byte*)handleVoid;

            TypedReference d = __makeref(instensT);
            void* handleVoid2 = *(void**)(&d);

            Assert.IsTrue(handleVoid == handleVoid2);
        }

        [Test]
        public void Test_CreateClass()
        {
            var instensT = (MyClassStruct)warp.Create();
            Assert.IsTrue(typeof(MyClassStruct) == instensT.GetType());
        }

        [Test]
        public void Test_FieldValue()
        {
            string fieldName = "one";

            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                int value = *(int*)(handleByte + addr.offset);
                //取值后 输出 1
                Assert.IsTrue(value == 1);
                //Assert.AreEqual(value, 1);

                *(int*)(handleByte + addr.offset) = 18;
                //赋值后 输出 18 
                Assert.IsTrue(instens.one == 18);

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 18
                Assert.AreEqual(obj, 18);

                addr.StructSetValue(handleVoid, 444);
                //不指定类型赋值后 输出 444
                Assert.IsTrue(instens.one == 444);
            }
        }

        [Test]
        public void Test_FieldClass()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "str";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                string value = (string)GeneralTool.VoidPtrToObject(*(void**)(handleByte + addr.offset));
                //取值后 输出 S3ED
                Assert.IsTrue(value == "S3ED");

                GeneralTool.SetObject(handleByte + addr.offset, "Acfv");
                //赋值后 输出 Acfv 
                Assert.IsTrue(instens.str == "Acfv");

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 Acfv 
                Assert.AreEqual(obj, "Acfv");

                addr.StructSetValue(handleVoid, "DDcc#2$%@");
                //不指定类型赋值后 输出 DDcc#2$%@
                Assert.AreEqual(instens.str, "DDcc#2$%@");
            }
        }

        [Test]
        public void Test_FieldStruct()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "point";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                Vector3 value = addr.StructReadStruct<Vector3>(handleVoid);
                //取值后 输出 3, -4.5, 97.4
                Assert.IsTrue(value == new Vector3(3, -4.5f, 97.4f));

                var pt = new Vector3(-99.56f, 50.22f, 9f);
                addr.StructWriteStruct<Vector3>(handleVoid, pt);
                Assert.IsTrue(instens.point == new Vector3(-99.56f, 50.22f, 9f));
                //取值后 输出 -99.56, 50.22, 9

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 -99.56, 50.22, 9
                Assert.AreEqual(obj, new Vector3(-99.56f, 50.22f, 9f));


                object obj2 = addr.GetValue(instens);
                //不指定类型取值后 输出 -99.56, 50.22, 9
                Assert.AreEqual(obj2, new Vector3(-99.56f, 50.22f, 9f));


                addr.StructSetValue(handleVoid, new Vector3(0, -9999f, 12.888f));
                //不指定类型赋值后 输出 0, -9999 , 12.888
                Assert.AreEqual(instens.point, new Vector3(0, -9999f, 12.888f));
            }
        }

        [Test]
        public void Test_PropertyValue()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "One";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                int k = addr.propertyDelegateItem.getInt32(handleVoid);
                //取值后 输出 -9
                Assert.IsTrue(k == -9);

                addr.propertyDelegateItem.setInt32(handleVoid, 18);
                //赋值后 输出 18
                Assert.IsTrue(instens.One == 18);

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 18
                Assert.AreEqual(obj, 18);

                addr.StructSetValue(handleVoid, 444);
                //不指定类型赋值后 输出 444
                Assert.AreEqual(instens.One, 444);
            }
        }

        [Test]
        public void Test_PropertyClass()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "Str";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                string value = addr.propertyDelegateItem.getString(handleByte);
                //取值后 输出 QBN3
                Assert.IsTrue(value == "QBN3");

                addr.propertyDelegateItem.setString(handleVoid, "$%^&ui yd7@#");
                //"赋值后 输出 $%^&ui yd7@# 
                Assert.IsTrue(instens.Str == "$%^&ui yd7@#");

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 ADc34gC@#
                Assert.AreEqual(obj, "$%^&ui yd7@#");

                addr.StructSetValue(handleVoid, "DDcc#2$%@ ");
                //不指定类型赋值后 输出 DDcc#2$%@
                Assert.AreEqual(instens.Str, "DDcc#2$%@ ");
            }
        }


        [Test]
        public void Test_PropertyStruct()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "Point";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

                Vector3 value = (Vector3)addr.propertyDelegateItem.getObject(handleVoid);
                //取值后 输出  45,-12.00009f,3
                Assert.IsTrue(value == new Vector3(45f, -12.00009f, 3f));

                var pt = new Vector3(-99.56f, 21423.009f, -993f);
                addr.propertyDelegateItem.setObject(handleVoid, pt);
                //赋值后 输出 -99.56, 21423.009, -993
                Assert.IsTrue(instens.Point == new Vector3(-99.56f, 21423.009f, -993f));

                object obj = addr.StructGetValue(handleVoid);
                //不指定类型取值后 输出 -99.56, 21423.009, -993
                Assert.AreEqual(obj, new Vector3(-99.56f, 21423.009f, -993f));

                addr.StructSetValue(handleVoid, new Vector3(12.888f, -9999f, 0));
                //不指定类型赋值后 输出 0, -9999 , 12.888
                Assert.AreEqual(instens.Point, new Vector3(12.888f, -9999f, 0));
            }
        }


        [Test]
        public void Test_FieldArrayValue()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "ones";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                //int[] array = (int[])addr.GetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);

                int[] chakeArray = new int[] { 1, 2, 8, 476, 898, 9 };
                //取值后 输出 1, 2, 8, 476, 898, 9
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    int value = *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
                    Assert.IsTrue(value == chakeArray[i]);
                }

                //赋值后 输出  0, 1, 2, 3, 4, 5
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.ones[i], i);
                }

                //不指定类型取值后 输出 0, 1, 2, 3, 4, 5
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                    Assert.AreEqual(value, i);
                }

                //不指定类型赋值后 输出 0, 100, 200, 300, 400, 500 : ");
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, 100 * i);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.ones[i], i * 100);
                }
            }
        }



        [Test]
        public void Test_FieldArrayClass()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "strs";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);

                var chakeArray = new string[] { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" };
                //取值后 输出 ass,#$%^&,*SAHASww&(), 兀驦屮鲵傌
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    string value = (string)GeneralTool.VoidPtrToObject(*(void**)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i));
                    Assert.AreEqual(chakeArray[i], value);
                }

                //赋值后 输出  Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    GeneralTool.SetObject(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, "Ac4……*" + i);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.strs[i], "Ac4……*" + i);
                }

                //"不指定类型取值后 输出 Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                    Assert.AreEqual(value, "Ac4……*" + i);
                }

                //不指定类型赋值后 输出 Fc%^0, Fc%^100, Fc%^200, Fc%^300 : ");
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, "Fc%^" + i * 100);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.strs[i], "Fc%^" + i * 100);
                }
            }
        }


        [Test]
        public void Test_FieldArrayStruct()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;
            string fieldName = "points";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);
                var chakeArray = new Vector3[] {
                   new Vector3(3, -4.5f, 97.4f),
                   new Vector3(9999f, -43f, 0.019f),
                   new Vector3(55.3f, -0.01f, -130),
                };
                //取值后 输出 (3, -4.5, 97.4) (9999, -43, 0.019) (55.3, -0.01, -130)
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    Vector3 value = *(Vector3*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
                    Assert.AreEqual(chakeArray[i], value);
                }

                //赋值后 输出  (0,0,0) , (1,1,1) , (2,2,2)
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    var v = new Vector3(i, i, i);
                    GeneralTool.MemCpy(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.points[i], new Vector3(i, i, i));
                }

                //不指定类型取值后 输出  (0,0,0) , (1,1,1) , (2,2,2)
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                    Assert.AreEqual(value, new Vector3(i, i, i));
                }

                //不指定类型赋值后 输出  (0,0,0) , (100,10,1000) , (200,20,2000)
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, new Vector3(i * 100, i * 10, i * 1000));
                }
                for (int i = 0; i < array.Length; i++)
                {
                    Assert.AreEqual(instens.points[i], new Vector3(i * 100, i * 10, i * 1000));
                }
            }
        }


        [Test]
        public void Test_FieldArrayWrapManagerValue()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "oness";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);
                var chakeArray = new int[,,]
                {
                    { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                    { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                    { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
                };

                //取值后 输出 1,2,252,1331,55,66,11,898,13,-19,-1,-999999,4576,8198,0,0,4176,8958
                int yzL = arrayWrapOutData.arrayLengths[1] * arrayWrapOutData.arrayLengths[2];
                int zL = arrayWrapOutData.arrayLengths[2];
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++)
                        {
                            int value = *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize *
                                (x * yzL + y * zL + z)
                                );
                            Assert.AreEqual(chakeArray[x, y, z], value);
                        }
                    }
                }

                //赋值后 输出 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yzL + y * zL + z)) = i;
                        }
                    }
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            Assert.AreEqual(instens.oness[x, y, z], i);
                        }
                    }
                }

                //不指定类型取值后 输出\n0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z));
                            Assert.AreEqual(value, i);
                        }
                    }
                }

                //不指定类型取值后 输出 0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z), -i);
                        }
                    }
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            Assert.AreEqual(instens.oness[x, y, z], -i);
                        }
                    }
                }

            }
        }


        [Test]
        public void Test_FieldArrayWrapManagerClass()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "strss";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);
                var chakeArray = new string[,] {
                    { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                    { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
                };

                //取值后 输出 ass,#$%^&,*SAHASww&(),兀驦屮鲵傌,adsadad,⑤驦屮尼傌,fun(a*(b+c)),FSD手动阀手动阀
                int yL = arrayWrapOutData.arrayLengths[1];
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        string value = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y)));
                        Assert.AreEqual(chakeArray[x, y], value);
                    }
                }


                //赋值后 输出 Ac4……*0,Ac4……*1,Ac4……*2,Ac4……*3,Ac4……*4,Ac4……*5,Ac4……*6,Ac4……*7
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        GeneralTool.SetObject(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), "Ac4……*" + i);
                    }
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        Assert.AreEqual(instens.strss[x, y], "Ac4……*" + i);
                    }
                }

                //"不指定类型取值后 输出 Ac4……*0,Ac4……*1,Ac4……*2,Ac4……*3,Ac4……*4,Ac4……*5,Ac4……*6,Ac4……*7
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y));
                        Assert.AreEqual(value, "Ac4……*" + i);
                    }
                }

                //不指定类型赋值后 输出 Fc%^0,Fc%^100,Fc%^200,Fc%^300,Fc%^400,Fc%^500,Fc%^600,Fc%^700: ");
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, "Fc%^" + (x * yL + y) * 100);
                    }
                }
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        Assert.AreEqual(instens.strss[x, y], "Fc%^" + (x * yL + y) * 100);
                    }
                }
            }
        }


        [Test]
        public void Test_FieldArrayWrapManagerStruct()
        {
            void* handleVoid = GeneralTool.AsPointer(ref instens);
            byte* handleByte = (byte*)handleVoid;

            string fieldName = "pointss";
            fixed (char* fieldNamePtr = fieldName)
            {
                TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
                var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

                Array array = (Array)addr.StructGetValue(handleVoid);
                ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

                ObjReference objReferenceArray = new ObjReference(array);
                byte** pp = (byte**)GeneralTool.AsPointer<ObjReference>(ref objReferenceArray);
                var chakeArray = new Vector3[,] {
                    {
                       new Vector3(3, -4.5f, 97.4f),
                       new Vector3(9999f, -43f, 0.019f),
                       new Vector3(55.3f, -0.01f, -130)
                    },
                    {
                       new Vector3(0, -1.2e+19f, 97.4f),
                       new Vector3(9999f, -100000f, 0.019f),
                       new Vector3(55.3f, -0.01f, -130)
                    },
                    {
                       new Vector3(3, -4.5f, 5555.5555f),
                       new Vector3(12321.4441f, -0.000001f, 0.019f),
                       new Vector3(1234, -982.3f, -299)
                    },
                };

                //取值后 输出 (3,-4.5,97.4),(9999,-43,0.019),(55.3,-0.01,-130),(0,-1.2E+19,97.4),(9999,-100000,0.019),(55.3,-0.01,-130),(3,-4.5,5555.5557),(12321.444,-1E-06,0.019),(1234,-982.3,-299)
                int yL = arrayWrapOutData.arrayLengths[1];
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        Vector3 value = *(Vector3*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y));
                        Assert.AreEqual(value, chakeArray[x, y]);
                    }
                }

                //赋值后 输出 (0,0,0),(1,1,1),(2,2,2),(3,3,3),(4,4,4),(5,5,5),(6,6,6),(7,7,7),(8,8,8)
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        var v = new Vector3(i, i, i);
                        GeneralTool.MemCpy(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
                    }
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        Assert.AreEqual(instens.pointss[x, y], new Vector3(i, i, i));
                    }
                }


                //不指定类型取值后 输出 (0,0,0),(1,1,1),(2,2,2),(3,3,3),(4,4,4),(5,5,5),(6,6,6),(7,7,7),(8,8,8)
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        //object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
                        object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y));
                        Assert.AreEqual(value, new Vector3(i, i, i));
                    }
                }

                //不指定类型赋值后 输出 (0,0,0),(100,10,1000),(200,20,2000),(300,30,3000),(400,40,4000),(500,50,5000),(600,60,6000),(700,70,7000),(800,80,8000)
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y), new Vector3(i * 100, i * 10, i * 1000));
                    }
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        Assert.AreEqual(instens.pointss[x, y], new Vector3(i * 100, i * 10, i * 1000));
                    }
                }
            }
        }





    }
}