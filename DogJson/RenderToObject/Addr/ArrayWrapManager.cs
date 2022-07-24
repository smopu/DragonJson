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
                ArrayWrapRankOne arrayWrapRank = new ArrayWrapRankOne();
                arrayWrapRank.elementType = itemType;
                arrayWrapRank.elementTypeSize = UnsafeOperation.SizeOfStack(itemType);
                arrayWrapRank.elementTypeCode = Type.GetTypeCode(itemType); 
                arrayWrapRank.rank = 1; 
                arrayWrapRank.head = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(itemType, 0));
                return arrayWrapRank;
            }
            else
            {
                ArrayWrapRank arrayWrapRank = new ArrayWrapRank();
                arrayWrapRank.elementType = itemType;
                arrayWrapRank.elementTypeSize = UnsafeOperation.SizeOfStack(itemType);
                arrayWrapRank.elementTypeCode = Type.GetTypeCode(itemType); 
                arrayWrapRank.rank = rank;
                arrayWrapRank.head = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(itemType, new int[rank]));
                return arrayWrapRank;
            }
        }
    }

    public unsafe abstract class IArrayWrap
    {
        public TypeCode elementTypeCode;
        public IntPtr head;
        public int rank;
        public int elementTypeSize;
        public Type elementType;
        CollectionManager.TypeAllCollection typeAllCollection;
        public CollectionManager.TypeAllCollection GetTypeAllCollection()
        {
            if (typeAllCollection == null)
            {
                typeAllCollection = CollectionManager.GetTypeCollection(elementType);
            }
            return typeAllCollection;
        }

        public abstract object CreateArray(int arraySize, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle,
             out int itemTypeSize);
    }

    public unsafe class ArrayWrapRank : IArrayWrap
    {
        public override object CreateArray(int arraySize, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle,
             out int itemTypeSize)
        {
            itemTypeSize = this.elementTypeSize;

            int offcet = (rank * 2 - 1) * 4;
            int arrayMsize = offcet + arraySize * itemTypeSize;

            object array = new byte[arrayMsize + UnsafeOperation.PTR_COUNT];
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
        public override object CreateArray(int length, int* arrayLengths, out byte* objPtr, out byte* startItemOffcet, out GCHandle gCHandle,
             out int itemTypeSize)
        {
            itemTypeSize = this.elementTypeSize;
            int arrayMsize = length * itemTypeSize;
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
