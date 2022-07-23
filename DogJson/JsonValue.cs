using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct JsonObject
    {
        [FieldOffset(0)]
        public char* keyStringStart;

        [FieldOffset(8)]
        public int keyStringLength;
        [FieldOffset(8)]
        public int arrayIndex;

        [FieldOffset(12)]
        public int objectNext;

        [FieldOffset(16)]
        public int objectQueueIndex;
        [FieldOffset(20)]
        public int parentObjectIndex;


        [FieldOffset(24)]
        public long _startCommand;
        /// <summary>
        /// #create
        /// </summary>
        [FieldOffset(24)]
        public bool isConstructor;
        /// <summary>
        /// #value
        /// </summary>
        [FieldOffset(25)]
        public bool isCommandValue;
        [FieldOffset(28)]
        public int arrayCount;

        [FieldOffset(32)]
        public bool isObject;

        [FieldOffset(36)]
        public long _startType;
        [FieldOffset(36)]
        public int typeStartIndex;
        [FieldOffset(40)]
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
        public int valueStringStart;
        [FieldOffset(12)]
        public int valueStringLength;

        [FieldOffset(16)]
        //public JsonObject* objectQueue; objectQueueIndex
        public int objectQueueIndex;

        [FieldOffset(24)]
        public int arrayIndex;

        [FieldOffset(28)]
        public JsonValueType type;



        [FieldOffset(32)]
        public long _startType;
        [FieldOffset(32)]
        public int typeStartIndex;
        [FieldOffset(36)]
        public int typeLength;



    }

}
