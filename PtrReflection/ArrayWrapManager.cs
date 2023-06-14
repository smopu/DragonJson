using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
{
    public unsafe static class ArrayWrapManager
    {
        public static IArrayWrap GetIArrayWrap(Type type)
        {
            var itemType = type.GetElementType();
            int rank = type.GetArrayRank();
            if (rank == 1)
            {
                ArrayWrapRankOne arrayWrapRank = new ArrayWrapRankOne(1, itemType);
                return arrayWrapRank;
            }
            else
            {
                ArrayWrapRank arrayWrapRank = new ArrayWrapRank(rank, itemType);
                return arrayWrapRank;
            }
        }
        /*
        private static bool isObjectArrayAddOffcet = false;
        public static bool IsObjectArrayAddOffcet
        {
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
        static void Start()
        {
            lock (lockData)
            {
                if (!isStart)
                {
                    object array = new byte[0];
                    object[] array2 = new object[1];
                    object vad = new object();
                    array2[0] = vad;
                    IntPtr* p1 = (IntPtr*)GeneralTool.ObjectToVoidPtr(array2);
                    IntPtr p2 = (IntPtr)GeneralTool.ObjectToVoidPtr(vad);

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
        */
    }

    public unsafe struct ArrayWrapData
    {
        /// <summary>
        /// 一维数组的长度
        /// </summary>
        public int length;
        /// <summary>
        /// 多维数组的长度
        /// </summary>
        public int[] arrayLengths;
        /// <summary>
        /// 第一个元素的内存偏移量
        /// </summary>
        public int startItemOffcet;
        //public GCHandle gCHandle;
    }
    public unsafe abstract class IArrayWrap
    {
        public readonly TypeCode elementTypeCode;
        public readonly IntPtr head;
        public readonly int rank;
        public readonly int elementTypeSize;
        public readonly Type elementType;
        /// <summary>
        /// 是否指类型
        /// </summary>
        public readonly bool isValueType;
        public IntPtr elementTypeHead;
        public int* lengths;
        public int allLength;

        public IArrayWrap(int rank, Type elementType)
        {
            this.rank = rank;
            this.elementType = elementType;
            this.elementTypeSize = UnsafeOperation.SizeOfStack(elementType);
            this.elementTypeCode = Type.GetTypeCode(elementType);
            this.isValueType = elementType.IsValueType;
            if (rank == 1)
            {
                this.head = *(IntPtr*)GeneralTool.ObjectToVoidPtr(Array.CreateInstance(elementType, 0));
            }
            else
            {
                this.head = *(IntPtr*)GeneralTool.ObjectToVoidPtr(Array.CreateInstance(elementType, new int[rank]));
            }
            this.lengths = (int*)Marshal.AllocHGlobal(rank);
            this.elementTypeHead = UnsafeOperation.GetTypeHead(elementType);
        }
        ~IArrayWrap()
        {
            Marshal.FreeHGlobal((IntPtr)lengths);
        }

        public abstract object CreateArray(int arraySize, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle);
        public abstract object CreateArray(ref ArrayWrapData arrayWrapData);
        public abstract object CreateArray();
        public abstract ArrayWrapData GetArrayData(Array array);

        public unsafe object GetValue(void* source, int index)
        {
            byte* field = (byte*)source;
            field += index * this.elementTypeSize;

            if (this.isValueType)
            {
                switch (elementTypeCode)
                {
                    case TypeCode.Boolean:
                        return *(bool*)field;
                    case TypeCode.Byte:
                        return *(Byte*)field;
                    case TypeCode.Char:
                        return *(Char*)field;
                    case TypeCode.DateTime:
                        return *(DateTime*)field;
                    case TypeCode.Decimal:
                        return *(Decimal*)field;
                    case TypeCode.Double:
                        return *(Double*)field;
                    case TypeCode.Int16:
                        return *(Int16*)field;
                    case TypeCode.Int32:
                        return *(Int32*)field;
                    case TypeCode.Int64:
                        return *(Int64*)field;
                    case TypeCode.SByte:
                        return *(Int64*)field;
                    case TypeCode.Single:
                        return *(Single*)field;
                    case TypeCode.UInt16:
                        return *(UInt16*)field;
                    case TypeCode.UInt32:
                        return *(UInt32*)field;
                    case TypeCode.UInt64:
                        return *(UInt64*)field;
                    case TypeCode.Object:
                    default:
                        //GC.Collect(); 
                        //return null;  
                        //ulong gcHandle;
                        object obj = new byte[this.elementTypeSize - 1 * UnsafeOperation.PTR_COUNT];
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
                        //if (typeHead == default(IntPtr))
                        //{
                        //    typeHead = UnsafeOperation.GetTypeHead(elementType);
                        //}
                        *ptr = elementTypeHead;
                        ptr += 1;

                        GeneralTool.MemCpy(ptr, field, this.elementTypeSize);
                        //GC.Collect();
                        return obj;
                }
            }
            else
            {
                return GeneralTool.VoidPtrToObject(*(IntPtr**)field);
            }
        }

        public unsafe void SetValue(void* source, int index, object value)
        {
            byte* field = (byte*)source;
            field += index * this.elementTypeSize;

            if (this.isValueType)
            {
                switch (elementTypeCode)
                {
                    case TypeCode.Boolean:
                        *(bool*)field = (bool)value;
                        break;
                    case TypeCode.Byte:
                        *(Byte*)field = (Byte)value;
                        break;
                    case TypeCode.Char:
                        *(Char*)field = (Char)value;
                        break;
                    case TypeCode.DateTime:
                        *(DateTime*)field = (DateTime)value;
                        break;
                    case TypeCode.Decimal:
                        *(Decimal*)field = (Decimal)value;
                        break;
                    case TypeCode.Double:
                        *(Double*)field = (Double)value;
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Int16:
                        *(Int16*)field = (Int16)value;
                        break;
                    case TypeCode.Int32:
                        *(Int32*)field = (Int32)value;
                        break;
                    case TypeCode.Int64:
                        *(Int64*)field = (Int64)value;
                        break;
                    case TypeCode.SByte:
                        *(SByte*)field = (SByte)value;
                        break;
                    case TypeCode.Single:
                        *(Single*)field = (Single)value;
                        break;
                    case TypeCode.UInt16:
                        *(UInt16*)field = (UInt16)value;
                        break;
                    case TypeCode.UInt32:
                        *(UInt32*)field = (UInt32)value;
                        break;
                    case TypeCode.UInt64:
                        *(UInt64*)field = (UInt64)value;
                        break;
                    case TypeCode.String:
                    case TypeCode.Object:
                        //ulong gcHandle;
                        IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(value);
                        GeneralTool.MemCpy(field, ptr + 1, elementTypeSize);
                        break;
                }
            }
            else
            {
                GeneralTool.SetObject(field, value);
            }
        }
    }

    public unsafe class ArrayWrapRank : IArrayWrap
    {
        public ArrayWrapRank(int rank, Type elementType) : base(rank, elementType) { }
        
        public override object CreateArray(int arraySize, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle)
        {
            int offcet = rank * 8;
            int arrayMsize = offcet + arraySize * this.elementTypeSize;

            object array = new byte[arrayMsize];
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            objPtr = (byte*)p;
            *p = head;
            ++p;
            *p = (IntPtr)arraySize; ++p;
            //*p = type.TypeHandle.Value; ++p;


            GeneralTool.MemCpy(p, arrayLengths, rank * 4);
            startItemOffcet = ((byte*)p);
            startItemOffcet += rank * 8;

            return array;
        }

        public override object CreateArray(ref ArrayWrapData arrayWrapData)
        {
#if msbuild
            if (isValueType)
            {

                int offcet = rank * 8;
                int* lengths = stackalloc int[rank];
                arrayWrapData.length = 1;
                for (int i = 0; i < rank; i++)
                {
                    arrayWrapData.length *= arrayWrapData.arrayLengths[i];
                    lengths[i] = arrayWrapData.arrayLengths[i];
                }
                int arrayMsize = offcet + arrayWrapData.length * this.elementTypeSize;

                object array = new byte[arrayMsize];

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)arrayWrapData.length; ++p;
                GeneralTool.MemCpy(p, lengths, rank * 4);
                arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT + rank * 8;

                return array;
            }
            else
            {
                int offcet = rank * 8;
                int* lengths = stackalloc int[rank];
                arrayWrapData.length = 1;
                for (int i = 0; i < rank; i++)
                {
                    arrayWrapData.length *= arrayWrapData.arrayLengths[i];
                    lengths[i] = arrayWrapData.arrayLengths[i];
                }
                int arrayMsize = offcet + arrayWrapData.length * this.elementTypeSize + UnsafeOperation.PTR_COUNT;

                object array = new byte[arrayMsize];

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)arrayWrapData.length; ++p;
                *p = this.elementTypeHead; ++p;
                GeneralTool.MemCpy(p, lengths, rank * 4);
                arrayWrapData.startItemOffcet = 3 * UnsafeOperation.PTR_COUNT + rank * 8;

                return array;
            }
#else
            int offcet = rank * 8;
            int* lengths = stackalloc int[rank];
            arrayWrapData.length = 1;
            for (int i = 0; i < rank; i++)
            {
                arrayWrapData.length *= arrayWrapData.arrayLengths[i];
                lengths[i] = arrayWrapData.arrayLengths[i];
            }
            int arrayMsize = offcet + arrayWrapData.length * this.elementTypeSize;

            object array = new byte[arrayMsize];

            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            *p = head;
            ++p;
            *p = (IntPtr)arrayWrapData.length; ++p;
            GeneralTool.MemCpy(p, lengths, rank * 4);
            arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT + rank * 8;

            return array;
#endif
        }

        public override object CreateArray()
        {
#if msbuild
            if (isValueType)
            {
                int arrayMsize = rank * 8 + allLength * this.elementTypeSize;

                object array = new byte[arrayMsize];

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)allLength; ++p;
                GeneralTool.MemCpy(p, lengths, rank * 4);
                return array;
            }
            else
            {
                int arrayMsize = rank * 8 + allLength * this.elementTypeSize + UnsafeOperation.PTR_COUNT;

                object array = new byte[arrayMsize];

                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)allLength; ++p;

                *p = this.elementTypeHead; ++p;

                GeneralTool.MemCpy(p, lengths, rank * 4);
                return array;
            }
#else
            int arrayMsize = rank * 8 + allLength * this.elementTypeSize;

            object array = new byte[arrayMsize];

            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            *p = head;
            ++p;
            *p = (IntPtr)allLength; ++p;
            GeneralTool.MemCpy(p, lengths, rank * 4);
            return array;
#endif

        }

        public override ArrayWrapData GetArrayData(Array array)
        {
#if msbuild
            if (isValueType)
            {
                ArrayWrapData arrayWrapData = new ArrayWrapData();
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                ++p;
                arrayWrapData.length = (int)*(long*)p; ++p;

                int* lengthP = (int*)p;

                arrayWrapData.arrayLengths = new int[rank];
                for (int i = 0; i < rank; i++)
                {
                    arrayWrapData.arrayLengths[i] = lengthP[i];
                }

                arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT + rank * 8;
                return arrayWrapData;
            }
            else
            {

                ArrayWrapData arrayWrapData = new ArrayWrapData();
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                ++p;
                arrayWrapData.length = (int)*(long*)p;
                p += 2;
                int* lengthP = (int*)p;

                arrayWrapData.arrayLengths = new int[rank];
                for (int i = 0; i < rank; i++)
                {
                    arrayWrapData.arrayLengths[i] = lengthP[i];
                }

                arrayWrapData.startItemOffcet = 3 * UnsafeOperation.PTR_COUNT + rank * 8;
                return arrayWrapData;
            
            }
#else

            ArrayWrapData arrayWrapData = new ArrayWrapData();
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            ++p;
            arrayWrapData.length = (int)*(long*)p; ++p;
            int* lengthP = (int*)p;

            arrayWrapData.arrayLengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                arrayWrapData.arrayLengths[i] = lengthP[i];
            }

            arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT + rank * 8;
            return arrayWrapData;
#endif

        }
    }

    public unsafe class ArrayWrapRankOne : IArrayWrap
    {
        public ArrayWrapRankOne(int rank, Type elementType) : base(rank, elementType) { }
        public override object CreateArray(int length, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle)
        {
            int arrayMsize = length * this.elementTypeSize;
            object array = new byte[arrayMsize];
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            objPtr = (byte*)p;
            *p = head;
            ++p;
            *p = (IntPtr)length; ++p;
            //*p = type.TypeHandle.Value; ++p;
            startItemOffcet = (byte*)p;
            return array;
        }
        public override object CreateArray(ref ArrayWrapData arrayWrapData)
        {
#if msbuild
            if (isValueType)
            {
                int arrayMsize = arrayWrapData.length * this.elementTypeSize;
                object array = new byte[arrayMsize];
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)arrayWrapData.length; ++p;
                arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT;
                return array;
            }
            else
            {
                int arrayMsize = arrayWrapData.length * this.elementTypeSize + UnsafeOperation.PTR_COUNT;
                object array = new byte[arrayMsize];
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)arrayWrapData.length; ++p;
                *p = this.elementTypeHead;
                ++p;
                arrayWrapData.startItemOffcet = 3 * UnsafeOperation.PTR_COUNT;
                return array;
            }
#else
            int arrayMsize = arrayWrapData.length * this.elementTypeSize;
            object array = new byte[arrayMsize];
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            *p = head;
            ++p;
            *p = (IntPtr)arrayWrapData.length; ++p;
            arrayWrapData.startItemOffcet = 2 * UnsafeOperation.PTR_COUNT;
            return array;
#endif
        }

        public override object CreateArray()
        {
#if msbuild
            if (isValueType)
            {
                int arrayMsize = allLength * this.elementTypeSize;
                object array = new byte[arrayMsize];
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;
                *p = (IntPtr)allLength; ++p;
                return array;
            }
            else
            {
                int arrayMsize = allLength * this.elementTypeSize + UnsafeOperation.PTR_COUNT * 1;
                object array = new byte[arrayMsize];
                IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
                *p = head;
                ++p;

                *p = (IntPtr)allLength; ++p;
                *p = elementTypeHead; ++p;


                return array;
            }
#else
            int arrayMsize = allLength * this.elementTypeSize;
            object array = new byte[arrayMsize];
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoidPtr(array);
            *p = head;
            ++p;
            *p = (IntPtr)allLength; ++p;
            return array;
#endif
        }

        public override ArrayWrapData GetArrayData(Array array)
        {

#if msbuild
            if (isValueType)
            {
                return new ArrayWrapData
                {
                    length = array.Length,
                    startItemOffcet = 2 * UnsafeOperation.PTR_COUNT
                };
            }
            else
            {
                return new ArrayWrapData
                {
                    length = array.Length,
                    startItemOffcet = 3 * UnsafeOperation.PTR_COUNT
                };
            }
#else

            return new ArrayWrapData
            {
                length = array.Length,
                startItemOffcet = 2 * UnsafeOperation.PTR_COUNT
            };
#endif

        }

    }


}
