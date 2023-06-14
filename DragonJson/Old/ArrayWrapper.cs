//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Runtime.InteropServices;
//using PtrReflection;

//namespace DragonJson
//{
//    public unsafe class ArrayWrapper
//    {
//        private static bool isObjectArrayAddOffcet = false;
//        public static bool IsObjectArrayAddOffcet
//        {
//            get
//            {
//                if (!isStart)
//                {
//                    Start();
//                }
//                return isObjectArrayAddOffcet;
//            }
//        }

//        public static int objectArray1StartOffcet = UnsafeOperation.PTR_COUNT * 2;
//        public static int objectArray1StartOffcetAdd = 2;
//        static void Start()
//        {
//            lock (lockData)
//            {
//                if (!isStart)
//                {
//                    object array = new byte[0];
//                    object[] array2 = new object[1];
//                    object vad = new object();
//                    array2[0] = vad;
//                    IntPtr* p1 = (IntPtr*)GeneralTool.ObjectToVoidPtr(array2);
//                    IntPtr p2 = (IntPtr)GeneralTool.ObjectToVoidPtr(vad);

//                    for (int i = 1; i < 10; i++)
//                    {
//                        if (p1[i] == p2)
//                        {
//                            objectArray1StartOffcet = UnsafeOperation.PTR_COUNT * i;
//                            objectArray1StartOffcetAdd = i;
//                            if (i != 2)
//                            {
//                                isObjectArrayAddOffcet = true;
//                            }
//                            break;
//                        }
//                    }
//                    isStart = false;
//                }
//            }
//        }

//        static bool isStart = false;
//        static object lockData = new object();
//    }

//}
