using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.RenderToObject
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
    }

    public unsafe abstract class IArrayWrap
    {
        public readonly TypeCode elementTypeCode;
        public readonly IntPtr head;
        public readonly int rank;
        public readonly int elementTypeSize;
        public readonly Type elementType;
        CollectionManager.TypeAllCollection typeAllCollection;

        public IArrayWrap(int rank, Type elementType)
        {
            this.rank = rank;
            this.elementType = elementType;
            this.elementTypeSize = UnsafeOperation.SizeOfStack(elementType);
            this.elementTypeCode = Type.GetTypeCode(elementType);
            if (rank == 1)
            {
                this.head = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(elementType, 0));
            }
            else
            {
                this.head = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(elementType, new int[rank]));
            }
        }

        public CollectionManager.TypeAllCollection GetTypeAllCollection()
        {
            if (typeAllCollection == null)
            {
                typeAllCollection = CollectionManager.GetTypeCollection(elementType);
            }
            return typeAllCollection;
        }


        public abstract object CreateArray(int arraySize, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle);
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

            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
            objPtr = (byte*)p;
            *p = head;
            ++p;
            *p = (IntPtr)arraySize; ++p;
            //*p = type.TypeHandle.Value; ++p;


            GeneralTool.Memcpy(p, arrayLengths, rank * 4);
            startItemOffcet = ((byte*)p);
            startItemOffcet += rank * 8;

            return array;
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
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
            objPtr = (byte*)p;
            *p = head;
            ++p;
            *p = (IntPtr)length; ++p;
            //*p = type.TypeHandle.Value; ++p;
            startItemOffcet = (byte*)p;
            return array;
        }
    }

}
