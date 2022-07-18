using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DogJson
{
    public unsafe class ArrayWrapper
    {
        private static IntPtr byteArrayHead = default(IntPtr);
        public static IntPtr ByteArrayHead
        {
            get
            {
                if (!isStart)
                {
                    Start();
                }
                return byteArrayHead;
            }
        }

        private static bool isObjectArrayAddOffcet = false;
        public static bool IsObjectArrayAddOffcet { 
            get
            {
                if (!isStart)
                {
                    Start();
                }
                return isObjectArrayAddOffcet;
            } 
        }



        public static int objectArray1StartOffcet = UnsafeOperation.PTR_COUNT * 2;
        public static int objectArray1StartOffcetAdd = 2;
        readonly static int array1StartOffcet = UnsafeOperation.PTR_COUNT * 2;
        int maxRank = 10;

        static void Start()
        {
            lock (lockData)
            {
                if (!isStart)
                {
                    object array = new byte[0];
                    byteArrayHead = *(IntPtr*)GeneralTool.ObjectToVoid(array);
                    object[] array2 = new object[1];
                    object vad = new object();
                    array2[0] = vad;
                    IntPtr* p1 = (IntPtr*)GeneralTool.ObjectToVoid(array2);
                    IntPtr p2 = (IntPtr)GeneralTool.ObjectToVoid(vad);

                    //IntPtr* longp2 = (IntPtr*)GeneralTool.ObjectToVoid(array2);
                    //List<IntPtr> dt2 = new List<IntPtr>();
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    dt2.Add(longp2[i]);
                    //}
                    for (int i = 1; i < 10; i++)
                    {
                        if (p1[i] == p2)
                        {
                            objectArray1StartOffcet = UnsafeOperation.PTR_COUNT * i;
                            objectArray1StartOffcetAdd = i;
                            if (i != 2)
                            {
                                isObjectArrayAddOffcet = true;
                            }
                            break;
                        }
                    }
                    isStart = false;
                }
            }
        }

        static bool isStart = false;
        static object lockData = new object();
        public ArrayWrapper(int Max_Rank = 10)
        {
            this.maxRank = Max_Rank;
            intPtr = Marshal.AllocHGlobal(100);
            array_lengths = (int*)intPtr.ToPointer();

            if (!isStart)
            {
                Start();
            }
        }



        ~ArrayWrapper()
        {
            Marshal.FreeHGlobal(intPtr);
        }

        IntPtr intPtr;
        int* array_lengths;
        int rank = 0;
        public int arraySize = 1;

        Dictionary<Type, ArrayWrapperItem> allArrayTypeHeadAddr = new Dictionary<Type, ArrayWrapperItem>();


        public void SetSize(int length)
        {
            array_lengths[rank] = length;
            ++rank;
            arraySize *= length;
            if (maxRank < rank)
            {
                maxRank = rank;
            }
        }
        //public void ResetSize()
        //{
        //    rank = 0;
        //    array_size = 1;
        //}


        public object CreateArrayOne(Type type, int length, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle, out int itemSize)
        {
            if (isObjectArrayAddOffcet && !type.IsValueType)
            {

                object array = Array.CreateInstance(type, length);
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
                objPtr = (byte*)p;

                IntPtr head = *p;
                *p = byteArrayHead;
                gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                *p = head;
                ++p;
                *p = (IntPtr)length;
                ++p;
                startItemOffcet = (byte*)p;
                itemSize = UnsafeOperation.PTR_COUNT;
                return array;
            }
            else
            {
                ArrayWrapperItem arrayWrapperItem;
                if (allArrayTypeHeadAddr.TryGetValue(type, out arrayWrapperItem))
                {
                }
                else
                {
                    allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
                }
                itemSize = arrayWrapperItem.typeSize;
                int arrayMsize = length * arrayWrapperItem.typeSize;
                object array = new byte[arrayMsize];
                gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
                objPtr = (byte*)p;
                *p = arrayWrapperItem.heads[1];
                ++p;
                *p = (IntPtr)length;
                ++p;
                startItemOffcet = (byte*)p;
                return array;
            }
        }

        public object CreateArray(Type type, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle, 
             out int itemTypeSize)
        {
            ArrayWrapperItem arrayWrapperItem;
            if (allArrayTypeHeadAddr.TryGetValue(type, out arrayWrapperItem))
            {
                if (arrayWrapperItem.maxRank < maxRank)
                {
                    allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
                }
            }
            else
            {
                allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
            }
            itemTypeSize = arrayWrapperItem.typeSize;

            int offcet = (rank * 2 - 1) * 4;
            int arrayMsize = offcet + arraySize * itemTypeSize;
            if (isObjectArrayAddOffcet && !type.IsValueType)
            {
                //object array2 = Array.CreateInstance(type, new int[] { 2, 3 });
                object array = new byte[arrayMsize + UnsafeOperation.PTR_COUNT];
                gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
                objPtr = (byte*)p;
                *p = arrayWrapperItem.heads[rank]; ++p;
                *p = (IntPtr)arraySize; ++p;
                *p = type.TypeHandle.Value;
                ++p;

                GeneralTool.Memcpy(p, array_lengths, rank * 4);
                startItemOffcet = ((byte*)p);
                startItemOffcet += rank * 8;
                rank = 0;
                arraySize = 1;

                //long* longp = (long*)GeneralTool.ObjectToVoid(array);
                //List<long> dt = new List<long>();
                //for (int i = 0; i < 10; i++)
                //{
                //    dt.Add(longp[i]);
                //}

                //long* longp2 = (long*)GeneralTool.ObjectToVoid(array2);
                //List<long> dt2 = new List<long>();
                //for (int i = 0; i < 10; i++)
                //{
                //    dt2.Add(longp2[i]);
                //}

                return array;
            }
            else
            {
                object array = new byte[arrayMsize];
                gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
                objPtr = (byte*)p;
                *p = arrayWrapperItem.heads[rank];
                ++p;

                *p = (IntPtr)arraySize; ++p;
                //startOffcet = (byte*)GeneralTool.ObjectToVoid(array) + offcet + array1StartOffcet + 4;
                GeneralTool.Memcpy(p, array_lengths, rank * 4);
                startItemOffcet = ((byte*)p);
                startItemOffcet += rank * 8;
                rank = 0;
                arraySize = 1;

                return array;
            }
        }

        public class ArrayWrapperItem
        {
            public ArrayWrapperItem(Type itemType, int maxRank)
            {
                this.itemType = itemType;
                this.maxRank = maxRank;
                this.typeSize = UnsafeOperation.SizeOfStack(itemType);
                heads = new IntPtr[maxRank + 1];
                heads[1] = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(itemType, 0));
                for (int i = 2; i < maxRank + 1; i++)
                {
                    object objarr = Array.CreateInstance(itemType, new int[i]);
                    heads[i] = *(IntPtr*)GeneralTool.ObjectToVoid(objarr);
                }

            }
            public int maxRank;
            public Type itemType;
            public IntPtr[] heads;
            public int typeSize;
        }

    }

}
