using NUnit.Framework;
using PtrReflection;
using System;
using System.Text;


namespace PtrReflection.Test
{
    public unsafe class TimeConsumingUnitTest
    {
        TypeAddrReflectionWrapper warp = TypeAddrReflectionWrapper.GetTypeWarp(typeof(MyClass));

        string[] v1s = new string[] { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" };
        int[] v2s = new int[] { 1, 2, 8, 476, 898, 9 };
        Vector3[] v3s = new Vector3[] {
               new Vector3(3, -4.5f, 97.4f),
               new Vector3(9999f, -43f, 0.019f),
               new Vector3(55.3f, -0.01f, -130),
            };

        string[,] v1ss = new string[,] {
                { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
            };
        int[,,] v2ss = new int[,,]
            {
                { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
            };
        Vector3[,] v3ss = new Vector3[,] {
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

        string v1 = "qwxc21";
        int v2 = 21323;
        Vector3 v3 = new Vector3(1, 565, 77.88f);
        MyClass myClass = null;

        const int testCount = 100000;

        const string fieldName_str = nameof(MyClass.str);
        const string fieldName_one = nameof(MyClass.one);
        const string fieldName_point = nameof(MyClass.point);

        int fieldName_strLength = fieldName_str.Length;
        int fieldName_oneLength = fieldName_one.Length;
        int fieldName_pointLength = fieldName_point.Length;

        const string fieldName_Str = nameof(MyClass.Str);
        const string fieldName_One = nameof(MyClass.One);
        const string fieldName_Point = nameof(MyClass.Point);

        int fieldName_StrLength = fieldName_Str.Length;
        int fieldName_OneLength = fieldName_One.Length;
        int fieldName_PointLength = fieldName_Point.Length;


        [SetUp]
        public void Setup()
        {
            myClass = (MyClass)warp.Create();
            myClass.one = 1;
            myClass.str = "S3ED";
            myClass.point = new Vector3(3, -4.5f, 97.4f);

            myClass.One = -9;
            myClass.Str = "QBN3";
            myClass.Point = new Vector3(45f, -12.00009f, 3f);

            myClass.ones = new int[] { 1, 2, 8, 476, 898, 9 };
            myClass.strs = new string[] { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" };
            myClass.points = new Vector3[] {
               new Vector3(3, -4.5f, 97.4f),
               new Vector3(9999f, -43f, 0.019f),
               new Vector3(55.3f, -0.01f, -130),
            };

            myClass.oness = new int[,,]
            {
                { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
            };
            myClass.strss = new string[,] {
                { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
            };
            myClass.pointss = new Vector3[,] {
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
        public void SystemReflection_FieldInfo_SetValue()
        {
            //原生反射
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetField(fieldName_str).SetValue(myClass, v1);
                typeof(MyClass).GetField(fieldName_one).SetValue(myClass, v2);
                typeof(MyClass).GetField(fieldName_point).SetValue(myClass, v3);
            }
        }


        [Test]
        public void PtrReflection_FieldInfo_SetValue()
        {
            //指针方法 SetValue 使用object类型 string查询
            for (int i = 0; i < testCount; i++)
            {
                warp.nameOfField[fieldName_str].SetValue(myClass, v1);
                warp.nameOfField[fieldName_one].SetValue(myClass, v2);
                warp.nameOfField[fieldName_point].SetValue(myClass, v3);
            }
        }


        [Test]
        public void PtrReflection_FieldInfo_CharPtr_SetValue()
        {
            //指针方法 SetValue 使用object类型 char*查询 忽略fixed
            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    warp.Find(_str, fieldName_strLength).SetValue(myClass, v1);
                    warp.Find(_one, fieldName_oneLength).SetValue(myClass, v2);
                    warp.Find(_point, fieldName_pointLength).SetValue(myClass, v3);
                }
            }
        }


        /// <summary>
        /// 指针方法 SetValue 确定类型的 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_FieldInfo_CharPtr_SetValue_NoObjectBox()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    GeneralTool.SetObject(*handleByte + warp.Find(_str, fieldName_strLength).offset, v1);
                    *(int*)(*handleByte + warp.Find(_one, fieldName_oneLength).offset) = v2;
                    warp.Find(_point, fieldName_pointLength).ClassWriteStruct<Vector3>(handleVoid, v3);
                }
            }
        }

        /// <summary>
        /// 指针方法 SetValue 确定类型的 忽略字符串查询 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_FieldInfo_CharPtr_SetValue_NoObjectBox_NoSelectName()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                TypeAddrFieldAndProperty addr1 = warp.Find(_str, fieldName_strLength);
                TypeAddrFieldAndProperty addr2 = warp.Find(_one, fieldName_oneLength);
                TypeAddrFieldAndProperty addr3 = warp.Find(_point, fieldName_pointLength);
                for (int i = 0; i < testCount; i++)
                {
                    GeneralTool.SetObject(*handleByte + addr1.offset, v1);
                    *(int*)(*handleByte + addr2.offset) = v2;
                    addr3.ClassWriteStruct<Vector3>(handleVoid, v3);
                }
            }
        }

        /// <summary>
        /// 原生
        /// </summary>
        [Test]
        public void Native_FieldInfo_SetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                myClass.str = v1;
                myClass.one = v2;
                myClass.point = v3;
            }
        }




        /// <summary>
        /// 原生反射
        /// </summary>
        [Test]
        public void SystemReflection_FieldInfo_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetField(fieldName_str).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetField(fieldName_one).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetField(fieldName_point).GetValue(myClass);
            }
        }


        [Test]
        public void PtrReflection_FieldInfo_GetValue()
        {
            //指针方法 GetValue 使用object类型 string查询
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.nameOfField[fieldName_str].ClassGetFieldValue(handleVoid);
                v2 = (int)warp.nameOfField[fieldName_one].ClassGetFieldValue(handleVoid);
                v3 = (Vector3)warp.nameOfField[fieldName_point].ClassGetFieldValue(handleVoid);
            }
        }


        [Test]
        public void PtrReflection_FieldInfo_CharPtr_GetValue()
        {
            //指针方法 GetValue 使用object类型 char*查询 忽略fixed
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);

            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    v1 = (string)warp.Find(_str, fieldName_strLength).ClassGetFieldValue(handleVoid);
                    v2 = (int)warp.Find(_one, fieldName_oneLength).ClassGetFieldValue(handleVoid);
                    v3 = (Vector3)warp.Find(_point, fieldName_pointLength).ClassGetFieldValue(handleVoid);
                }
            }
        }


        /// <summary>
        /// 指针方法 GetValue 确定类型的 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_FieldInfo_CharPtr_GetValue_NoObjectBox()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + warp.Find(_str, fieldName_strLength).offset));
                    v2 = *(int*)(*handleByte + warp.Find(_one, fieldName_oneLength).offset);
                    v3 = warp.Find(_point, fieldName_pointLength).ClassReadStruct<Vector3>(handleVoid);
                }
            }
        }

        /// <summary>
        /// 指针方法 GetValue 确定类型的 忽略字符串查询 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_FieldInfo_CharPtr_GetValue_NoObjectBox_NoSelectName()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _str = fieldName_str)
            fixed (char* _one = fieldName_one)
            fixed (char* _point = fieldName_point)
            {
                TypeAddrFieldAndProperty addr1 = warp.Find(_str, fieldName_strLength);
                TypeAddrFieldAndProperty addr2 = warp.Find(_one, fieldName_oneLength);
                TypeAddrFieldAndProperty addr3 = warp.Find(_point, fieldName_pointLength);
                for (int i = 0; i < testCount; i++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + addr1.offset));
                    v2 = *(int*)(*handleByte + addr2.offset);
                    v3 = addr3.ClassReadStruct<Vector3>(handleVoid); 
                }
            }
        }

        /// <summary>
        /// 原生 GetValue
        /// </summary>
        [Test]
        public void Native_FieldInfo_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1 = myClass.str;
                v2 = myClass.one;
                v3 = myClass.point;
            }
        }





        [Test]
        public void SystemReflection_Property_SetValue()
        {
            //原生反射
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetProperty(fieldName_Str).SetValue(myClass, v1);
                typeof(MyClass).GetProperty(fieldName_One).SetValue(myClass, v2);
                typeof(MyClass).GetProperty(fieldName_Point).SetValue(myClass, v3); ;
            }
        }


        [Test]
        public void PtrReflection_Property_SetValue()
        {
            //指针方法 SetValue 使用object类型 string查询
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            for (int i = 0; i < testCount; i++)
            {
                warp.nameOfField[fieldName_Str].ClassSetPropertyValue(handleVoid, v1);
                warp.nameOfField[fieldName_One].ClassSetPropertyValue(handleVoid, v2);
                warp.nameOfField[fieldName_Point].ClassSetPropertyValue(handleVoid, v3);
            }
        }


        [Test]
        public void PtrReflection_Property_CharPtr_SetValue()
        {
            //指针方法 SetValue 使用object类型 char*查询 忽略fixed
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    warp.Find(_Str, fieldName_StrLength).ClassSetPropertyValue(handleVoid, v1);
                    warp.Find(_One, fieldName_OneLength).ClassSetPropertyValue(handleVoid, v2);
                    warp.Find(_Point, fieldName_PointLength).ClassSetPropertyValue(handleVoid, v3);
                }
            }
        }


        /// <summary>
        /// 指针方法 SetValue 确定类型的 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_Property_CharPtr_SetValue_NoObjectBox()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.setString(*handleVoid, v1);
                    warp.Find(_One, fieldName_OneLength).propertyDelegateItem.setInt32(*handleVoid, v2);
                    warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.setObject(*handleVoid, v3);
                }
            }
        }

        /// <summary>
        /// 指针方法 SetValue 确定类型的 忽略字符串查询 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_Property_CharPtr_SetValue_NoObjectBox_NoSelectName()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                TypeAddrFieldAndProperty addr1 = warp.Find(_Str, fieldName_StrLength);
                TypeAddrFieldAndProperty addr2 = warp.Find(_One, fieldName_OneLength);
                TypeAddrFieldAndProperty addr3 = warp.Find(_Point, fieldName_PointLength);
                for (int i = 0; i < testCount; i++)
                {
                    addr1.propertyDelegateItem.setString(*handleVoid, v1);
                    addr2.propertyDelegateItem.setInt32(*handleVoid, v2);
                    addr3.propertyDelegateItem.setObject(*handleVoid, v3);
                }
            }
        }

        /// <summary>
        /// 原生
        /// </summary>
        [Test]
        public void Native_Property_SetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1 = myClass.Str;
                v2 = myClass.One;
                v3 = myClass.Point;
            }
        }




        /// <summary>
        /// 原生反射
        /// </summary>
        [Test]
        public void SystemReflection_Property_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetProperty(fieldName_Str).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetProperty(fieldName_One).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetProperty(fieldName_Point).GetValue(myClass);
            }
        }


        [Test]
        public void PtrReflection_Property_GetValue()
        {
            //指针方法 GetValue 使用object类型 string查询
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.nameOfField[fieldName_Str].ClassGetPropertyValue(handleVoid);
                v2 = (int)warp.nameOfField[fieldName_One].ClassGetPropertyValue(handleVoid);
                v3 = (Vector3)warp.nameOfField[fieldName_Point].ClassGetPropertyValue(handleVoid);
            }
        }


        [Test]
        public void PtrReflection_Property_CharPtr_GetValue()
        {
            //指针方法 GetValue 使用object类型 char*查询 忽略fixed
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    v1 = (string)warp.Find(_Str, fieldName_StrLength).ClassGetPropertyValue(handleVoid);
                    v2 = (int)warp.Find(_One, fieldName_OneLength).ClassGetPropertyValue(handleVoid);
                    v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).ClassGetPropertyValue(handleVoid);
                }
            }
        }


        /// <summary>
        /// 指针方法 GetValue 确定类型的 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_Property_CharPtr_GetValue_NoObjectBox()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                for (int i = 0; i < testCount; i++)
                {
                    v1 = warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.getString(*handleVoid);
                    v2 = warp.Find(_One, fieldName_OneLength).propertyDelegateItem.getInt32(*handleVoid);
                    v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.getObject(*handleVoid);
                }
            }
        }

        /// <summary>
        /// 指针方法 GetValue 确定类型的 忽略字符串查询 char*查询, 没有装箱操作，忽略fixed时间。
        /// </summary>
        [Test]
        public void PtrReflection_Property_CharPtr_GetValue_NoObjectBox_NoSelectName()
        {
            ObjReference objReference = new ObjReference(myClass);
            void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
            byte** handleByte = (byte**)handleVoid;

            fixed (char* _Str = fieldName_Str)
            fixed (char* _One = fieldName_One)
            fixed (char* _Point = fieldName_Point)
            {
                TypeAddrFieldAndProperty addr1 = warp.Find(_Str, fieldName_StrLength);
                TypeAddrFieldAndProperty addr2 = warp.Find(_One, fieldName_OneLength);
                TypeAddrFieldAndProperty addr3 = warp.Find(_Point, fieldName_PointLength);
                for (int i = 0; i < testCount; i++)
                {
                    v1 = (string)addr1.propertyDelegateItem.getObject(*handleVoid);
                    v2 = addr2.propertyDelegateItem.getInt32(*handleVoid);
                    v3 = (Vector3)addr3.propertyDelegateItem.getObject(*handleVoid);
                }
            }
        }

        /// <summary>
        /// 原生 GetValue
        /// </summary>
        [Test]
        public void Native_Property_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1 = myClass.Str;
                v2 = myClass.One;
                v3 = myClass.Point;
            }
        }







        /// <summary>
        /// 原生反射
        /// </summary>
        [Test]
        public void SystemReflection_Array_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1s;
                for (int j = 0; j < arrayV1.Length; j++)
                {
                    v1 = (string)arrayV1.GetValue(j);
                }
                Array arrayV2 = v2s;
                for (int j = 0; j < arrayV2.Length; j++)
                {
                    v2 = (int)arrayV2.GetValue(j);
                }
                Array arrayV3 = v3s;
                for (int j = 0; j < arrayV3.Length; j++)
                {
                    v3 = (Vector3)arrayV3.GetValue(j);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager GetValue
        /// </summary>
        [Test]
        public void PtrReflection_Array_GetValue()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)arrayWrapV1.GetValue(*v1sP + data1.startItemOffcet, j);
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = (int)arrayWrapV2.GetValue(*v2sP + data2.startItemOffcet, j);
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = (Vector3)arrayWrapV3.GetValue(*v3sP + data3.startItemOffcet, j);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager GetValue 确定类型的
        /// </summary>
        [Test]
        public void PtrReflection_Array_GetValue_NoObjectBox()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));


            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = *(Vector3*)(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager GetValue 确定类型的 忽略Data查询
        /// </summary>
        [Test]
        public void PtrReflection_Array_GetValue_NoObjectBox_NoSelectData()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
            ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
            ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);

            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                }
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                }
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = *(Vector3*)(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                }
            }
        }

        /// <summary>
        /// 原生 GetValue
        /// </summary>
        [Test]
        public void Native_Array_GetValue()
        {
            for (int j = 0; j < v1s.Length; j++)
            {
                v1 = v1s[j];
            }
            for (int j = 0; j < v2s.Length; j++)
            {
                v2 = v2s[j];
            }
            for (int j = 0; j < v3s.Length; j++)
            {
                v3 = v3s[j];
            }
        }

        /// <summary>
        /// 原生反射 Array SetValue
        /// </summary>
        [Test]
        public void SystemReflection_Array_SetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1s;
                for (int j = 0; j < arrayV1.Length; j++)
                {
                    arrayV1.SetValue(v1, j);
                }
                Array arrayV2 = v2s;
                for (int j = 0; j < arrayV2.Length; j++)
                {
                    arrayV2.SetValue(v2, j);
                }
                Array arrayV3 = v3s;
                for (int j = 0; j < arrayV3.Length; j++)
                {
                    arrayV3.SetValue(v3, j);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager SetValue
        /// </summary>
        [Test]
        public void PtrReflection_Array_SetValue()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    arrayWrapV1.SetValue(*v1sP + data1.startItemOffcet, j, v1);
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    arrayWrapV2.SetValue(*v2sP + data2.startItemOffcet, j, v2);
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    arrayWrapV3.SetValue(*v3sP + data3.startItemOffcet, j, v3);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager SetValue 确定类型的：
        /// </summary>
        [Test]
        public void PtrReflection_Array_SetValue_NoObjectBox()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    GeneralTool.SetObject(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    var v = new Vector3(j, j, j);
                    GeneralTool.MemCpy(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager SetValue 确定类型的 忽略Data查询
        /// </summary>
        [Test]
        public void PtrReflection_Array_SetValue_NoObjectBox_NoSelectData()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
            ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
            ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);

            ObjReference v1sReference = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sReference);

            ObjReference v2sReference = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sReference);

            ObjReference v3sReference = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sReference);

            for (int i = 0; i < testCount; i++)
            {
                for (int j = 0; j < data1.length; j++)
                {
                    GeneralTool.SetObject(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                }

                for (int j = 0; j < data2.length; j++)
                {
                    *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                }

                for (int j = 0; j < data3.length; j++)
                {
                    var v = new Vector3(i, i, i);
                    GeneralTool.MemCpy(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                }
            }
        }

        /// <summary>
        /// 原生 SetValue
        /// </summary>
        [Test]
        public void Native_Array_SetValue()
        {
            for (int j = 0; j < v1s.Length; j++)
            {
                v1s[j] = v1;
            }
            for (int j = 0; j < v2s.Length; j++)
            {
                v2s[j] = v2;
            }
            for (int j = 0; j < v3s.Length; j++)
            {
                v3s[j] = v3;
            }
        }








        /// <summary>
        /// 原生反射 Multidimensional Array GetValue
        /// </summary>
        [Test]
        public void SystemReflection_MultidimensionalArray_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1ss;
                for (int x = 0; x < arrayV1.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV1.GetLength(1); y++)
                    {
                        v1 = (string)arrayV1.GetValue(x, y);
                    }
                }

                Array arrayV2 = v2ss;
                for (int x = 0; x < arrayV2.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV2.GetLength(1); y++)
                    {
                        for (int z = 0; z < arrayV2.GetLength(2); z++)
                        {
                            v2 = (int)arrayV2.GetValue(x, y, z);
                        }
                    }
                }

                Array arrayV3 = v3ss;
                for (int x = 0; x < arrayV3.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV3.GetLength(1); y++)
                    {
                        v3 = (Vector3)arrayV3.GetValue(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// 指针方法 Multidimensional ArrayWrapManager GetValue
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_GetValue()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        v1 = (string)arrayWrapV1.GetValue(*v1ssP + data1.startItemOffcet, x * data1.arrayLengths[1] + y);
                    }
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            v2 = (int)arrayWrapV2.GetValue(*v2ssP + data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                        }
                    }
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        v3 = (Vector3)arrayWrapV3.GetValue(*v3ssP + data3.startItemOffcet, x * data3.arrayLengths[1] + y);
                    }
                }
            }
        }

        /// <summary>
        /// 指针方法 Multidimensional ArrayWrapManager GetValue 确定类型的
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_GetValue_NoObjectBox()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                    }
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            v2 = *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                        }
                    }
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        v3 = *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                    }
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager GetValue 确定类型的 忽略Data查询
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_GetValue_NoObjectBox_NoSelectData()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
            ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
            ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                    }
                }
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            v2 = *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                        }
                    }
                }
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        v3 = *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                    }
                }
            }
        }

        /// <summary>
        /// 原生 GetValue
        /// </summary>
        [Test]
        public void Native_MultidimensionalArray_GetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < v1ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v1ss.GetLength(1); y++)
                    {
                        v1 = v1ss[x, y];
                    }
                }
                for (int x = 0; x < v2ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v2ss.GetLength(1); y++)
                    {
                        for (int z = 0; z < v2ss.GetLength(2); z++)
                        {
                            v2 = v2ss[x, y, z];
                        }
                    }
                }
                for (int x = 0; x < v3ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v3ss.GetLength(1); y++)
                    {
                        v3 = v3ss[x, y];
                    }
                }
            }
        }










        /// <summary>
        /// 原生反射 Multidimensional Array SetValue
        /// </summary>
        [Test]
        public void SystemReflection_MultidimensionalArray_SetValue()
        {
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1ss;
                for (int x = 0; x < arrayV1.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV1.GetLength(1); y++)
                    {
                        arrayV1.SetValue(v1, x, y);
                    }
                }
                Array arrayV2 = v2ss;
                for (int x = 0; x < arrayV2.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV2.GetLength(1); y++)
                    {
                        for (int z = 0; z < arrayV2.GetLength(2); z++)
                        {
                            arrayV2.SetValue(v2, x, y, z);
                        }
                    }
                }
                Array arrayV3 = v3ss;
                for (int x = 0; x < arrayV3.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV3.GetLength(1); y++)
                    {
                        arrayV3.SetValue(v3, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// 指针方法 Multidimensional ArrayWrapManager SetValue
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_SetValue()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        //v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                        arrayWrapV1.SetValue(*v1ssP + data1.startItemOffcet, x * data1.arrayLengths[1] + y, v1);
                    }
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            arrayWrapV2.SetValue(*v2ssP + data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z), v2);
                        }
                    }
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        arrayWrapV3.SetValue(*v3ssP + data3.startItemOffcet, x * data3.arrayLengths[1] + y, v3);
                    }
                }
            }
        }

        /// <summary>
        /// 指针方法 ArrayWrapManager SetValue 确定类型的：
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_SetValue_NoObjectBox()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        GeneralTool.SetObject(
                            (*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)),
                            v1);
                    }
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z))
                                = v2;
                        }
                    }
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        // GeneralTool.MemCpy((data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)), GeneralTool.AsPointer(ref v3), arrayWrapV3.elementTypeSize);
                        *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)) = v3;
                    }
                }
            }
        }

        /// <summary>
        ///指针方法 Multidimensional ArrayWrapManager SetValue 忽略Data查询
        /// </summary>
        [Test]
        public void PtrReflection_MultidimensionalArray_SetValue_NoObjectBox_NoSelectData()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());

            ObjReference v1ssReference = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssReference);

            ObjReference v2ssReference = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssReference);

            ObjReference v3ssReference = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssReference);

            ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
            ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
            ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        GeneralTool.SetObject(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y), v1);
                    }
                }
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z)) = v2;
                        }
                    }
                }
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)) = v3;
                    }
                }
            }
        }

        /// <summary>
        /// 原生 SetValue
        /// </summary>
        [Test]
        public void Native_MultidimensionalArray_SetValue()
        {
            for (int x = 0; x < v1ss.GetLength(0); x++)
            {
                for (int y = 0; y < v1ss.GetLength(1); y++)
                {
                    v1ss[x, y] = v1;
                }
            }
            for (int x = 0; x < v2ss.GetLength(0); x++)
            {
                for (int y = 0; y < v2ss.GetLength(1); y++)
                {
                    for (int z = 0; z < v2ss.GetLength(2); z++)
                    {
                        v2ss[x, y, z] = v2;
                    }
                }
            }
            for (int x = 0; x < v3ss.GetLength(0); x++)
            {
                for (int y = 0; y < v3ss.GetLength(1); y++)
                {
                    v3ss[x, y] = v3;
                }
            }
        }






        /// <summary>
        /// 原生反射 Activator CreateInstance
        /// </summary>
        [Test]
        public void Activator_CreateInstance()
        {
            for (int i = 0; i < testCount; i++)
            {
                MyClass a = (MyClass)System.Activator.CreateInstance(typeof(MyClass));
            }
        }

        /// <summary>
        /// 指针方法 Create Class
        /// </summary>
        [Test]
        public void CreateClass()
        {
            for (int i = 0; i < testCount; i++)
            {
                MyClass a = (MyClass)warp.Create();
            }
        }

        /// <summary>
        /// 原生 new Class
        /// </summary>
        [Test]
        public void Native_NewClass()
        {
            for (int i = 0; i < testCount; i++)
            {
                MyClass a = new MyClass();
            }
        }


        /// <summary>
        /// 指针方法 创建数组
        /// </summary>
        [Test]
        public void CreateArray()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData arrayWrapData1 = new ArrayWrapData
                {
                    length = 5
                };
                v1s = (string[])arrayWrapV1.CreateArray(ref arrayWrapData1);

                ArrayWrapData arrayWrapData2 = new ArrayWrapData
                {
                    length = 10
                };
                v2s = (int[])arrayWrapV2.CreateArray(ref arrayWrapData2);

                ArrayWrapData arrayWrapData3 = new ArrayWrapData
                {
                    length = 4
                };
                v3s = (Vector3[])arrayWrapV3.CreateArray(ref arrayWrapData3);

            }
        }

        /// <summary>
        /// 指针方法 创建数组 (不需要多余返回值)
        /// </summary>
        [Test]
        public void CreateArray_DontReturn()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));
            for (int i = 0; i < testCount; i++)
            {
                arrayWrapV1.allLength = 5;
                v1s = (string[])arrayWrapV1.CreateArray();
                arrayWrapV2.allLength = 10;
                v2s = (int[])arrayWrapV2.CreateArray();
                arrayWrapV3.allLength = 4;
                v3s = (Vector3[])arrayWrapV3.CreateArray();
            }
        }

        /// <summary>
        /// 原生反射 创建数组
        /// </summary>
        [Test]
        public void System_ArrayCreateInstance_CreateArray()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1s = (string[])Array.CreateInstance(typeof(string), 5);
                v2s = (int[])Array.CreateInstance(typeof(int), 10);
                v3s = (Vector3[])Array.CreateInstance(typeof(Vector3), 4);
            }
        }

        /// <summary>
        /// 原生 new 数组
        /// </summary>
        [Test]
        public void Native_CreateArray()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1s = new string[5];
                v2s = new int[10];
                v3s = new Vector3[4];
            }
        }



        /// <summary>
        /// 指针方法 创建多维数组
        /// </summary>
        [Test]
        public void CreateMultidimensionalArray()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData arrayWrapData1 = new ArrayWrapData();
                arrayWrapData1.arrayLengths = new int[] { 2, 4 };
                v1ss = (string[,])arrayWrapV1.CreateArray(ref arrayWrapData1);

                ArrayWrapData arrayWrapData2 = new ArrayWrapData();
                arrayWrapData2.arrayLengths = new int[] { 3, 3, 2 };
                v2ss = (int[,,])arrayWrapV2.CreateArray(ref arrayWrapData2);

                ArrayWrapData arrayWrapData3 = new ArrayWrapData();
                arrayWrapData3.arrayLengths = new int[] { 3, 3 };
                v3ss = (Vector3[,])arrayWrapV3.CreateArray(ref arrayWrapData3);
            }
        }

        /// <summary>
        /// 指针方法 创建多维数组(不需要多余返回值)
        /// </summary>
        [Test]
        public void CreateMultidimensionalArray_DontReturn()
        {
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());
            for (int i = 0; i < testCount; i++)
            {
                arrayWrapV1.lengths[0] = 2;
                arrayWrapV1.lengths[1] = 4;
                arrayWrapV1.allLength = 2 * 4;
                v1ss = (string[,])arrayWrapV1.CreateArray();

                arrayWrapV2.lengths[0] = 3;
                arrayWrapV2.lengths[1] = 3;
                arrayWrapV2.lengths[2] = 2;
                arrayWrapV2.allLength = 3 * 3 * 2;
                v2ss = (int[,,])arrayWrapV2.CreateArray();

                arrayWrapV3.lengths[0] = 3;
                arrayWrapV3.lengths[1] = 3;
                arrayWrapV3.allLength = 3 * 3;
                v3ss = (Vector3[,])arrayWrapV3.CreateArray();
            }
        }

        /// <summary>
        /// 原生反射 创建多维数组 
        /// </summary>
        [Test]
        public void System_ArrayCreateInstance_CreateMultidimensionalArray()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1ss = (string[,])Array.CreateInstance(typeof(string), new int[] { 2, 4 });
                v2ss = (int[,,])Array.CreateInstance(typeof(int), new int[] { 3, 3, 2 });
                v3ss = (Vector3[,])Array.CreateInstance(typeof(Vector3), new int[] { 3, 3 });
            }
        }


        /// <summary>
        /// 原生 new 多维数组 
        /// </summary>
        [Test]
        public void Native_CreateMultidimensionalArray()
        {
            for (int i = 0; i < testCount; i++)
            {
                v1ss = new string[2, 4];
                v2ss = new int[3, 3, 2];
                v3ss = new Vector3[3, 3];
            }
        }



    }
}
