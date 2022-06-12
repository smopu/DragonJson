using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    //[StructLayout(LayoutKind.Explicit)]
    public unsafe struct JsonObject
    {
        public char* keyStringStart;
        public int keyStringLength;

        public int objectQueueIndex;
        public int parentObjectIndex;

        public bool isObject;

        /// <summary>
        /// #create
        /// </summary>
        public bool isConstructor;

        /// <summary>
        /// #value
        /// </summary>
        public bool isCommandValue;


        public int objectNext;

        public int arrayCount;

        public int arrayIndex;

        public int typeStartIndex;
        public int typeLength;
    }


    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct JsonValue
    {
        [FieldOffset(0)]
        public int keyStringStart;
        [FieldOffset(4)]
        public int keyStringLength;

        [FieldOffset(8)]
        public long valueLong;

        [FieldOffset(8)]
        public double valueDouble;

        [FieldOffset(8)]
        public bool valueBool;

        [FieldOffset(8)]
        public int vStringStart;
        [FieldOffset(12)]
        public int vStringLength;

        [FieldOffset(16)]
        public JsonObject* objectQueue;

        [FieldOffset(24)]
        public int arrayIndex;

        [FieldOffset(28)]
        public JsonValueType type;

        [FieldOffset(32)]
        public int typeStartIndex;

        [FieldOffset(36)]
        public int typeLength;

    }

}
